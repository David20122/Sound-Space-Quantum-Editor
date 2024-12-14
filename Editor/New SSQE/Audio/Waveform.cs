using New_SSQE.GUI.Shaders;
using New_SSQE.Preferences;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Drawing;

namespace New_SSQE.Audio
{
    internal class Waveform
    {
        private static float[] WaveModel = Array.Empty<float>();
        private static int waveLength = 0;
        private static VertexArrayHandle VaO;
        private static BufferHandle VbO;
        private static int posLocation;

        private static bool isUploaded = false;
        private static double resolution = 0.0d;
        private static bool classic = false;

        public static long Offset = -15;

        private struct Level
        {
            public short left;
            public short right;
        }

        private static Level ParseBuffer(short[] data, int start, int end)
        {
            short left = 0;
            short right = 0;

            for (int i = start; i < Math.Min(end + 1, data.Length); i += 2)
            {
                if (Math.Abs((int)data[i]) > Math.Abs((int)left))
                    left = data[i];
                if (Math.Abs((int)data[i + 1]) > Math.Abs((int)right))
                    right = data[i + 1];
            }

            return new()
            {
                left = left,
                right = right
            };
        }

        public static void Init(int bps)
        {
            resolution = 1 / (double)Settings.waveformDetail.Value / 100;

            double bpf = bps * resolution;
            short[] buffer = new short[1024 * 1024];
            Level[] data = [];

            while (true)
            {
                int len = MusicPlayer.GetData(ref buffer);

                if (len >= 0)
                {
                    Level[] set = new Level[(int)(len / bpf / 2)];
                    int cur = 0;

                    for (double i = 0; i < len; i += bpf)
                    {
                        int index = (int)(i / 2) * 2;
                        if (index >= 0 && cur < set.Length)
                            set[cur] = ParseBuffer(buffer, index, index + (int)bpf);
                        cur++;
                    }

                    data = data.Concat(set).ToArray();
                }
                else
                    break;
            }

            int offset = (int)Math.Abs(Offset / 1000d / resolution / 2);
            Level[] blank = new Level[offset];

            if (Offset > 0)
                data = blank.Concat(data[..^offset]).ToArray();
            else
                data = data[offset..].Concat(blank).ToArray();

            classic = Settings.classicWaveform.Value;
            WaveModel = new float[data.Length * (classic ? 2 : 4)];

            int maxPeak = 0;

            for (int i = 0; i < data.Length; i++)
            {
                int absL = Math.Abs((int)data[i].left);
                int absR = Math.Abs((int)data[i].right);

                maxPeak = Math.Max(maxPeak, Math.Max(absL, absR));
            }

            bool aboveZero = maxPeak > 0;

            for (int i = 0; i < data.Length; i++)
            {
                float pos = i / (float)data.Length;

                float left = (float)data[i].left / (aboveZero ? maxPeak : 1);
                float right = (float)data[i].right / (aboveZero ? maxPeak : 1);

                if (classic)
                {
                    float absL = Math.Abs(left);
                    float absR = Math.Abs(right);

                    WaveModel[i * 2 + 0] = pos;
                    WaveModel[i * 2 + 1] = 1 - Math.Max(absL, absR) * 2f;
                }
                else
                {
                    WaveModel[i * 4 + 0] = pos;
                    WaveModel[i * 4 + 1] = left;
                    WaveModel[i * 4 + 2] = pos;
                    WaveModel[i * 4 + 3] = right;
                }
            }

            if (isUploaded)
                Dispose();
            Upload();
        }

        private static void Upload()
        {
            GL.UseProgram(Shader.WaveformProgram);
            VaO = GL.GenVertexArray();
            VbO = GL.GenBuffer();
            posLocation = GL.GetUniformLocation(Shader.WaveformProgram, "WavePos");

            Color color = Settings.color4.Value;
            float[] c = new float[3] { color.R / 255f, color.G / 255f, color.B / 255f };
            int colorLocation = GL.GetUniformLocation(Shader.WaveformProgram, "LineColor");
            GL.Uniform3f(colorLocation, c[0], c[1], c[2]);

            GL.BindVertexArray(VaO);

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, VbO);
            GL.BufferData(BufferTargetARB.ArrayBuffer, WaveModel, BufferUsageARB.StaticDraw);

            waveLength = WaveModel.Length / 2;
            WaveModel = Array.Empty<float>();

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, BufferHandle.Zero);
            GL.BindVertexArray(VertexArrayHandle.Zero);

            isUploaded = true;
        }

        public static void Render(Vector4 pos, float trackHeight)
        {
            if (!isUploaded)
                return;

            int start = Math.Max(0, (int)(pos.Z * waveLength) - 1);
            int end = Math.Min(waveLength, (int)(pos.W * waveLength) + 1);

            GL.UseProgram(Shader.WaveformProgram);
            GL.Uniform3f(posLocation, pos.X, pos.Y - pos.X, trackHeight);

            GL.BindVertexArray(VaO);
            GL.DrawArrays(PrimitiveType.LineStrip, start, end - start);
        }

        public static void Dispose()
        {
            GL.BindBuffer(BufferTargetARB.ArrayBuffer, VbO);
            GL.BufferData(BufferTargetARB.ArrayBuffer, 0, nint.Zero, BufferUsageARB.StaticDraw);
            GL.BindBuffer(BufferTargetARB.ArrayBuffer, BufferHandle.Zero);
            GL.DeleteBuffer(VbO);

            GL.DeleteVertexArray(VaO);

            isUploaded = false;
        }
    }
}
