using OpenTK.Mathematics;

namespace SSQE_Player.GUI
{
    internal class Camera
    {
        private Vector2 rotation = (0, MathHelper.Pi);

        private Matrix4 projection;
        private Matrix4 view;

        private Vector3 lookVector;
        private Vector3 cameraPos;
        private Vector3 camOffset = new();

        private readonly float rad88 = MathHelper.DegreesToRadians(88);
        private readonly float radFoV = MathHelper.DegreesToRadians(Settings.fov.Value);

        private readonly string camMode = Settings.cameraMode.Value.Current;
        private readonly bool lockCursor = Settings.lockCursor.Value;
        private readonly float sensitivity = Settings.sensitivity.Value;
        private readonly float parallax = Settings.parallax.Value;

        public Vector2 LockedPos = (0, 0);

        public void Update()
        {
            Matrix4 rot = Matrix4.CreateRotationY(rotation.Y) * Matrix4.CreateRotationX(rotation.X);

            lookVector = (rot * -Vector4.UnitZ).Xyz;
            cameraPos = -Vector3.UnitZ * 3.5f + lookVector * (1.25f, 1.25f, 0) + camOffset;

            view = Matrix4.CreateTranslation(-cameraPos) * rot;

            Shader.SetProjection(projection);
            Shader.SetView(view);
            Shader.SetProjView(projection, view);

            // update cursor
            {
                Vector3 cursorSize = MainWindow.CursorSize;
                Vector3 pos;

                if (camMode == "spin")
                    pos = cameraPos + new Vector3(lookVector.X, lookVector.Y, -Math.Abs(lookVector.Z)) * (float)Math.Abs(Math.Abs(cameraPos.Z + 0.15f) / lookVector.Z);
                else
                    pos = (LockedPos.X, LockedPos.Y, 0);

                float xf = MathHelper.Clamp(pos.X, -1.5f + cursorSize.X / 2f, 1.5f - cursorSize.X / 2f);
                float yf = MathHelper.Clamp(pos.Y, -1.5f + cursorSize.Y / 2f, 1.5f - cursorSize.Y / 2f);

                MainWindow.Instance.CursorPos = (xf, yf, 0);
            }
        }

        public void SetReplay(Vector2 mouse)
        {
            if (camMode == "spin")
                rotation = (MathHelper.Clamp(-mouse.Y / 4.6f, -rad88, rad88), MathHelper.Pi - mouse.X / 4.6f);
            else
            {
                LockedPos = mouse;

                Vector3 cursorSize = MainWindow.CursorSize;

                float xf = MathHelper.Clamp(LockedPos.X, -1.5f + cursorSize.X / 2f, 1.5f - cursorSize.X / 2f);
                float yf = MathHelper.Clamp(LockedPos.Y, -1.5f + cursorSize.Y / 2f, 1.5f - cursorSize.Y / 2f);

                if (lockCursor)
                    LockedPos = (xf, yf);

                if (camMode == "half lock")
                    camOffset = (xf * parallax / 5f, yf * parallax / 5f, 0);
            }

            Update();
        }

        public void SetMouse(Vector2 mouse)
        {
            if (camMode == "spin")
                rotation = (MathHelper.Clamp(rotation.X + mouse.Y / 1000f * sensitivity, -rad88, rad88), rotation.Y + mouse.X / 1000f * sensitivity);
            else
            {
                LockedPos -= (mouse.X / 250f * sensitivity, mouse.Y / 250f * sensitivity);

                Vector3 cursorSize = MainWindow.CursorSize;

                float xf = MathHelper.Clamp(LockedPos.X, -1.5f + cursorSize.X / 2f, 1.5f - cursorSize.X / 2f);
                float yf = MathHelper.Clamp(LockedPos.Y, -1.5f + cursorSize.Y / 2f, 1.5f - cursorSize.Y / 2f);

                if (lockCursor)
                    LockedPos = (xf, yf);

                if (camMode == "half lock")
                    camOffset = (xf * parallax / 5f, yf * parallax / 5f, 0);
            }

            Update();
        }

        public void CalculateProjection()
        {
            projection = Matrix4.CreatePerspectiveFieldOfView(radFoV, MainWindow.Instance.Size.X / (float)MainWindow.Instance.Size.Y, 0.1f, 1000);
        }
    }
}
