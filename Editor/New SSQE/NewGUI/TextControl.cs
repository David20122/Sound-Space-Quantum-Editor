using New_SSQE.GUI.Font;
using New_SSQE.GUI.Shaders;
using OpenTK.Mathematics;
using System.Drawing;

namespace New_SSQE.NewGUI
{
    internal abstract class TextControl : Control
    {
        protected string text;
        protected int textSize;
        protected string font;
        protected float alpha;
        protected Color textColor = Color.White;

        public bool Centered = true;
        private Vector4[] verts = [];

        private float textX;
        private float textY;

        public TextControl(float x, float y, float w, float h, string text = "", int textSize = 0, string font = "main", bool centered = true) : base(x, y, w, h)
        {
            this.text = text;
            this.textSize = textSize;
            this.font = font;

            Centered = centered;
            Update();
        }

        public override void Update()
        {
            base.Update();

            if (Centered)
            {
                float width = FontRenderer.GetWidth(text, textSize, font);
                float height = FontRenderer.GetHeight(textSize, font) * text.Split('\n').Length;

                textX = rect.X + rect.Width / 2 - width / 2;
                textY = rect.Y + rect.Height / 2 - height / 2;
            }
            else
            {
                textX = rect.X;
                textY = rect.Y;
            }

            verts = FontRenderer.Print(textX, textY, text, textSize, font);
        }

        public override void PostRender(float mousex, float mousey, float frametime)
        {
            base.PostRender(mousex, mousey, frametime);
            float[] a = new float[verts.Length];

            if (alpha > 0)
                for (int i = 0; i < a.Length; i++)
                    a[i] = 1 - alpha;

            GLState.EnableProgram(FontRenderer.unicode ? Shader.UnicodeProgram : Shader.FontProgram);
            FontRenderer.SetActive(font);
            FontRenderer.SetColor(textColor);
            FontRenderer.RenderData(font, verts, a);
        }

        public virtual void SetText(string? text = null, int? textSize = null, string? font = null)
        {
            this.text = text ?? this.text;
            this.textSize = textSize ?? this.textSize;
            this.font = font ?? this.font;

            Update();
        }

        public virtual void SetColor(Color? color = null, float? alpha = null)
        {
            textColor = color ?? textColor;
            this.alpha = alpha ?? this.alpha;
        }
    }
}
