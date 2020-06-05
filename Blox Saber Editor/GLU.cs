using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Sound_Space_Editor
{
	static class Glu
	{
		public static void RenderQuad(double x, double y, double sx, double sy)
		{
			GL.Translate(x, y, 0);
			GL.Scale(sx, sy, 1);
			GL.Begin(PrimitiveType.Quads);
			GL.Vertex2(0, 0);
			GL.Vertex2(0, 1);
			GL.Vertex2(1, 1);
			GL.Vertex2(1, 0);
			GL.End();
			GL.Scale(1f / sx, 1f / sy, 1);
			GL.Translate(-x, -y, 0);
		}

		public static void RenderQuad(RectangleF rect)
		{
			RenderQuad(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static void RenderTexturedQuad(float x, float y, float sx, float sy, float us, float vs, float ue, float ve, int texture)
		{
			GL.BindTexture(TextureTarget.Texture2D, texture);
			GL.Translate(x, y, 0);
			GL.Scale(sx, sy, 1);
			GL.Begin(PrimitiveType.Quads);
			GL.TexCoord2(us, vs);
			GL.Vertex2(0, 0);
			GL.TexCoord2(us, ve);
			GL.Vertex2(0, 1);
			GL.TexCoord2(ue, ve);
			GL.Vertex2(1, 1);
			GL.TexCoord2(ue, vs);
			GL.Vertex2(1, 0);
			GL.End();
			GL.Scale(1f / sx, 1f / sy, 1);
			GL.Translate(-x, -y, 0);
			GL.BindTexture(TextureTarget.Texture2D, 0);
		}

		public static void RenderTexturedQuad(RectangleF rect, float us, float vs, float ue, float ve, int texture)
		{
			RenderTexturedQuad(rect.X, rect.Y, rect.Width, rect.Height, us, vs, ue, ve, texture);
		}

		public static void RenderOutline(float x, float y, float sx, float sy)
		{
			GL.PolygonMode(MaterialFace.Front, PolygonMode.Line);

			x += 0.5f;
			y += 0.5f;

			sx--;
			sy--;

			GL.Translate(x, y, 0);
			GL.Scale(sx, sy, 1);
			GL.Begin(PrimitiveType.Polygon);
			GL.Vertex2(0, 0);
			GL.Vertex2(0, 1);
			GL.Vertex2(1, 1);
			GL.Vertex2(1, 0);
			GL.End();
			GL.Scale(1 / sx, 1 / sy, 1);
			GL.Translate(-x, -y, 0);

			GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
		}

		public static void RenderOutline(RectangleF rect)
		{
			RenderOutline(rect.X, rect.Y, rect.Width, rect.Height);
		}

		public static void RenderCircle(float x, float y, float r, int pts = 6)
		{
			GL.Begin(PrimitiveType.Polygon);

			for (int i = 0; i < pts; i++)
			{
				var a = i / (float)pts * MathHelper.TwoPi;

				var cx = Math.Cos(a) * r;
				var cy = -Math.Sin(a) * r;

				GL.Vertex2(x + cx, y + cy);
			}

			GL.End();
		}
	}
}