using New_SSQE.GUI.Font;
using OpenTK.Mathematics;

namespace New_SSQE.NewGUI
{
    internal abstract class TextControl : Control
    {
        protected string text;
        protected int textSize;
        protected string font;

        public bool Centered = true;
        private Vector4[] verts;

        private float textX;
        private float textY;

        public TextControl(float x, float y, float w, float h, string text = "", int textSize = 0, string font = "main") : base(x, y, w, h)
        {
            this.text = text;
            this.textSize = textSize;
            this.font = font;

            Update();
        }

        public TextControl(float x, float y, float w, float h, string text = "", int textSize = 0, string font = "main", bool centered = true) : this(x, y, w, h, text, textSize, font)
        {
            Centered = centered;
        }

        public override void Update()
        {
            base.Update();


        }

        public override void PostRender(float mousex, float mousey, float frametime)
        {
            base.PostRender(mousex, mousey, frametime);


        }

        public virtual void SetText(string? text = null, int? textSize = null, string? font = null)
        {
            this.text = text ?? this.text;
            this.textSize = textSize ?? this.textSize;
            this.font = font ?? this.font;

            Update();
        }
    }
}
