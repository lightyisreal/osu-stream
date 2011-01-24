using System;
using osum.Graphics.Sprites;
using osum.Graphics.Drawables;
using osum.Helpers;
using OpenTK.Graphics;
using OpenTK;
#if IPHONE
using OpenTK.Graphics.ES11;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.OpenGLES;
using TextureTarget = OpenTK.Graphics.ES11.All;
using TextureParameterName = OpenTK.Graphics.ES11.All;
using EnableCap = OpenTK.Graphics.ES11.All;
using ArrayCap = OpenTK.Graphics.ES11.All;
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
using ErrorCode = OpenTK.Graphics.ES11.All;
using TextureEnvParameter = OpenTK.Graphics.ES11.All;
using TextureEnvTarget =  OpenTK.Graphics.ES11.All;
#else
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;
#endif


namespace osum.Graphics.Drawables
{
	internal class CircularProgress : pDrawable
	{
        internal float Progress;
        internal float Radius;
		internal bool EvenShading;

        public CircularProgress(Vector2 position, float radius, bool alwaysDraw, float drawDepth, Color4 colour)
		{
            AlwaysDraw = alwaysDraw;
            DrawDepth = drawDepth;
            StartPosition = position;
            Position = position;
            Radius = radius;
            Colour = colour;
            Field = FieldTypes.Standard;
		}
		
		public override void Dispose()
		{
		}
		
		public override bool Draw()
		{
            if (base.Draw())
            {

                Color4 c = AlphaAppliedColour;

                float resolution = 0.1f;
                float startAngle = (float)(-Math.PI / 2);
                float cappedProgress = pMathHelper.ClampToOne(Progress);

                float endAngle = (float)(cappedProgress * Math.PI * 2f + startAngle);

                int parts = (int)((endAngle - startAngle) / resolution);

                float da = (endAngle - startAngle) / parts;

                parts++;

                float[] vertices = new float[parts * 2 + 2];
                float[] colours = new float[parts * 4 + 4];

                float radius = Radius * GameBase.WindowRatio * ScaleScalar;
                Vector2 pos = FieldPosition;

                vertices[0] = pos.X;
                vertices[1] = pos.Y;

                colours[0] = c.R;
                colours[1] = c.G;
                colours[2] = c.B;
                colours[3] = c.A * Progress;

                float a = startAngle;
                for (int v = 1; v <= parts; v++)
                {
                    vertices[v * 2] = (float)(pos.X + Math.Cos(a) * radius);
                    vertices[v * 2 + 1] = (float)(pos.Y + Math.Sin(a) * radius);
                    a += da;

                    colours[v * 4] = c.R;
                    colours[v * 4 + 1] = c.G;
                    colours[v * 4 + 2] = c.B;
                    colours[v * 4 + 3] = c.A * (EvenShading ? 0.5f :  (0.1f + 0.4f * ((float)v/parts)));
                }

                GL.EnableClientState(ArrayCap.ColorArray);

                GL.VertexPointer(2, VertexPointerType.Float, 0, vertices);
                GL.ColorPointer(4, ColorPointerType.Float, 0, colours);
                GL.DrawArrays(BeginMode.TriangleFan, 0, parts + 1);

                GL.DisableClientState(ArrayCap.ColorArray);

                return true;
            }

            return false;

		}
	}
}

