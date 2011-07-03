using System;
using MonoTouch.UIKit;
using osum.Helpers;

#if iOS
using OpenTK.Graphics.ES11;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.OpenGLES;

using TextureTarget = OpenTK.Graphics.ES11.All;
using TextureParameterName = OpenTK.Graphics.ES11.All;
using EnableCap = OpenTK.Graphics.ES11.All;
using BlendingFactorSrc = OpenTK.Graphics.ES11.All;
using BlendingFactorDest = OpenTK.Graphics.ES11.All;
using PixelStoreParameter = OpenTK.Graphics.ES11.All;
using VertexPointerType = OpenTK.Graphics.ES11.All;
using ColorPointerType = OpenTK.Graphics.ES11.All;
using ClearBufferMask = OpenTK.Graphics.ES11.All;
using TexCoordPointerType = OpenTK.Graphics.ES11.All;
using BeginMode = OpenTK.Graphics.ES11.All;
using MatrixMode = OpenTK.Graphics.ES11.All;
using PixelInternalFormat = OpenTK.Graphics.ES11.All;
using PixelFormat = OpenTK.Graphics.ES11.All;
using PixelType = OpenTK.Graphics.ES11.All;
using ShaderType = OpenTK.Graphics.ES11.All;
using VertexAttribPointerType = OpenTK.Graphics.ES11.All;
using ProgramParameter = OpenTK.Graphics.ES11.All;
using ShaderParameter = OpenTK.Graphics.ES11.All;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
#else
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;
using osum.Input;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;
#endif

using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.OpenGLES;
using osum.Graphics.Skins;
using System.Globalization;
using System.Threading;

namespace osum
{
    public class GameBaseIphone : GameBase
    {
        GameWindowIphone gameWindow;

        public GameBaseIphone()
        {
        }

        override public void MainLoop()
        {
            MonoTouch.UIKit.UIApplication.Main(new string[]{});
        }

        bool disableDimming = false;
        internal override bool DisableDimming {
            get {
                return disableDimming;
            }
            set {
                if (value == disableDimming) return;
                disableDimming = value;
                UIApplication.SharedApplication.IdleTimerDisabled = value;
            }
        }

        public override void Initialize()
        {
            string culture = "en-US";
            switch (NSLocale.PreferredLanguages[0])
            {
                case "ja":
                    culture = "ja-JP";
                    break;
            }

            System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

            gameWindow = GameWindowIphone.Instance;
            base.Initialize();
        }

        public override void SetViewport()
        {
            base.SetViewport();

            float width = GameBase.NativeSize.Height;
            float height = GameBase.NativeSize.Width;
            GL.Translate(width / 2, height / 2, 0);
            GL.Rotate(90, 0, 0, 1);
            GL.Translate(-height / 2, -width / 2, 0);
        }

        protected override BackgroundAudioPlayer InitializeBackgroundAudio()
        {
            //only initialise the first time (we may be here from a resume operation)
            return new BackgroundAudioPlayerIphone();
        }

        protected override void InitializeInput()
        {
            InputSource source = new InputSourceIphone (gameWindow);
            gameWindow.SetInputHandler(source);
            InputManager.AddSource(source);
        }

        public override string PathConfig {
            get {
                return Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "/../Library/";
            }
        }

        public override Thread RunInBackground(VoidDelegate task)
        {
            ParameterizedThreadStart pts = delegate {
                int ourTask = 0;
                UIApplication application = UIApplication.SharedApplication;

                if (UIDevice.CurrentDevice.IsMultitaskingSupported)
                {
                    application.BeginBackgroundTask(delegate {
                        //expired
                        if (ourTask != 0)
                        {
                            application.EndBackgroundTask(ourTask);
                            ourTask = 0;
                        }
                    });
                }

                task();

                if (ourTask != 0)
                {
                    application.BeginInvokeOnMainThread(delegate
                    {
                        if (ourTask != 0) //same as above
                        {
                            application.EndBackgroundTask(ourTask);
                            ourTask = 0;
                        }
                    });
                }
            };

            Thread t = new Thread (pts);
            t.Priority = ThreadPriority.Highest;
            t.IsBackground = true;
            t.Start();
            return t;
        }
    }
}

