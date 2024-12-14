using OpenTK.Graphics.OpenGL;
using SkiaSharp;
using System.Drawing;

namespace New_SSQE.GUI
{
    internal class GuiSquare : WindowControl
    {
        public Color Color;
        private Color prevColor;

        private readonly bool IsTextured;
        private readonly string FileName = "";
        private readonly string TextureName = "";

        private readonly bool Outline;

        public GuiSquare(float x, float y, float w, float h, Color color, bool outline = false, string fileName = "", string textureName = "", bool moveWithOffset = false) : base(x, y, w, h)
        {
            Color = color;
            prevColor = Color;
            Outline = outline;

            if (fileName != "" && File.Exists(fileName))
            {
                IsTextured = true;
                FileName = fileName;
                TextureName = textureName;

                using FileStream fs = File.OpenRead(FileName);
                tHandle = TextureManager.GetOrRegister(TextureName, SKBitmap.Decode(fs), false);

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2d, tHandle);
            }
            else if (fileName == "" && textureName != "")
            {
                IsTextured = true;
                TextureName = textureName;

                tHandle = TextureManager.GetOrRegister(TextureName);
            }

            MoveWithOffset = moveWithOffset;

            Init();
        }

        public GuiSquare(Color color, bool outline = false, string fileName = "", string textureName = "") : this(0, 0, 0, 0, color, outline, fileName, textureName, false) { }

        public override void Render(float mousex, float mousey, float frametime)
        {
            if (!IsTextured && Visible)
            {
                if (prevColor != Color)
                {
                    Update();

                    prevColor = Color;
                }

                PrimitiveType type = Outline ? PrimitiveType.TriangleStrip : PrimitiveType.Triangles;
                int indexCount = Outline ? 10 : 6;

                GL.BindVertexArray(VaO);
                GL.DrawArrays(type, 0, indexCount);
            }
        }

        public override void RenderTexture()
        {
            if (IsTextured && Visible)
            {
                TextureManager.SetActive(0);
                GL.BindTexture(TextureTarget.Texture2d, tHandle);

                GL.BindVertexArray(tVaO);
                GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
            }
        }

        public override Tuple<float[], float[]> GetVertices()
        {
            float[] c = new float[] { Color.R / 255f, Color.G / 255f, Color.B / 255f, Color.A / 255f };

            float[] fill = Outline ? GLU.Outline(Rect, 2, c) : GLU.Rect(Rect, c);
            float[] texture = Array.Empty<float>();

            if (IsTextured)
                texture = GLU.TexturedRect(Rect, Color.A / 255f);

            return new(fill, texture);
        }
    }
}
