using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using Color = System.Drawing.Color;

namespace Sound_Space_Editor.Gui
{
    class GuiScreenSelectMap : GuiScreen
    {
        private int logoTxt;
        private FontRenderer fr = EditorWindow.Instance.FontRenderer;
        private GuiButton _createMapButton;
        private GuiButton _loadMapButton;
        private GuiButton _importButton;
        private GuiButton _lastMapButton;

        public GuiScreenSelectMap() : base(0, 0, EditorWindow.Instance.ClientSize.Width, EditorWindow.Instance.ClientSize.Height)
        {
            using (var img = Properties.Resources.logo)
            {
                logoTxt = TextureManager.GetOrRegister("logo",img,true);
            }
            if (File.Exists(Properties.Settings.Default.LastFile))
            {
                var widt = fr.GetWidth(Properties.Settings.Default.LastFile, 24);
                _lastMapButton = new GuiButton(3, 0, 0, widt, 48, Properties.Settings.Default.LastFile);
                Buttons.Add(_lastMapButton);
            }
            _createMapButton = new GuiButton(0, 0, 0, 256, 48, "CREATE NEW MAP");
            _loadMapButton = new GuiButton(1, 0, 0, 256, 48, "EDIT EXISTING MAP");
            _importButton = new GuiButton(1, 0, 0, 256, 48, "IMPORT MAP");
            Buttons.Add(_createMapButton);
            Buttons.Add(_loadMapButton);
            Buttons.Add(_importButton);
            OnResize(EditorWindow.Instance.ClientSize);
        }

        public override void Render(float delta, float mouseX, float mouseY)
        {
            var size = EditorWindow.Instance.ClientSize;
            Glu.RenderTexturedQuad(ClientRectangle.Width / 2 - 400 / 2, -72, 400, 400, 0, 0, 1, 1, logoTxt);
            var widt1 = fr.GetWidth("QUANTUM MAP", 22);
            var widt2 = fr.GetWidth("EDITOR", 22);
            fr.Render("QUANTUM MAP", size.Width / 2 - widt1 / 2, 248, 22);
            fr.Render("EDITOR", size.Width / 2 - widt2 / 2, 272, 22);
            base.Render(delta, mouseX, mouseY);
        }

        public override void OnResize(Size size)
        {
            _createMapButton.ClientRectangle.X = ClientRectangle.Width / 2 - _createMapButton.ClientRectangle.Width - 2;
            _createMapButton.ClientRectangle.Y = ClientRectangle.Height / 2;
            _loadMapButton.ClientRectangle.X = ClientRectangle.Width / 2 + 2;
            _loadMapButton.ClientRectangle.Y = ClientRectangle.Height / 2;
            _importButton.ClientRectangle.X = ClientRectangle.Width / 2 - _importButton.ClientRectangle.Width / 2;
            _importButton.ClientRectangle.Y = ClientRectangle.Height / 2 + 52;
            if (!(_lastMapButton == null))
            {
                _lastMapButton.ClientRectangle.X = ClientRectangle.Width / 2 - _lastMapButton.ClientRectangle.Width / 2;
                _lastMapButton.ClientRectangle.Y = ClientRectangle.Height / 2 + 104;
            }
            base.OnResize(size);
        }

        protected override void OnButtonClicked(int id)
        {
            switch (id)
            {
                case 0:

                    break;
            }
            base.OnButtonClicked(id);
        }
    }
}
