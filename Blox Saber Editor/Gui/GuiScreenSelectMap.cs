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
        private GuiButton _lastMapButton;

        public GuiScreenSelectMap() : base(0, 0, EditorWindow.Instance.ClientSize.Width, EditorWindow.Instance.ClientSize.Height)
        {
            using (var img = Properties.Resources.logo)
            {
                logoTxt = TextureManager.GetOrRegister("logo",img,true);
            }
            if (File.Exists(Properties.Settings.Default.LastFile))
            {
                var widt = fr.GetWidth(Properties.Settings.Default.LastFile, 20);
                _lastMapButton = new GuiButton(3, 0, 0, widt, 32, Properties.Settings.Default.LastFile);
                Buttons.Add(_lastMapButton);
            }
            _createMapButton = new GuiButton(0, 0, 0, 256, 32, "CREATE NEW MAP");
            _loadMapButton = new GuiButton(1, 0, 0, 256, 32, "EDIT EXISTING MAP");
            Buttons.Add(_createMapButton);
            Buttons.Add(_loadMapButton);
            OnResize(EditorWindow.Instance.ClientSize);
        }

        public override void Render(float delta, float mouseX, float mouseY)
        {
            var size = EditorWindow.Instance.ClientSize;
            Glu.RenderTexturedQuad(ClientRectangle.Width / 2 - 256 / 2, 0, 256, 256, 0, 0, 1, 1, logoTxt);
            var widt1 = fr.GetWidth("QUANTUM EDITOR", 24);
            fr.Render("QUANTUM EDITOR", size.Width / 2 - widt1 / 2, 200, 24);
            base.Render(delta, mouseX, mouseY);
        }

        public override void OnResize(Size size)
        {
            _createMapButton.ClientRectangle.X = ClientRectangle.Width / 2 - _createMapButton.ClientRectangle.Width - 2;
            _createMapButton.ClientRectangle.Y = ClientRectangle.Height / 2;
            _loadMapButton.ClientRectangle.X = ClientRectangle.Width / 2 + 2;
            _loadMapButton.ClientRectangle.Y = ClientRectangle.Height / 2;
            if (!(_lastMapButton == null))
            {
                _lastMapButton.ClientRectangle.X = ClientRectangle.Width / 2 - _lastMapButton.ClientRectangle.Width / 2;
                _lastMapButton.ClientRectangle.Y = ClientRectangle.Height / 2 + 36;
            }
            base.OnResize(size);
        }
    }
}
