using New_SSQE.GUI;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using Un4seen.Bass;

namespace New_SSQE
{
    internal class Waveform
    {
        private static float[] WaveModel = Array.Empty<float>();
        private static VertexArrayHandle VaO;
        private static BufferHandle VbO;
        private static int posLocation;

        private static bool isUploaded = false;

        private struct Level
        {
            public short left;
            public short right;
        }

        public static void Init(int streamID)
        {
            double length = Bass.BASS_ChannelGetLength(streamID);
            if (length < 0)
            {
                length = 0;
                byte[] buffer = new byte[32768];
                int curBuffer;

                while ((curBuffer = Bass.BASS_ChannelGetData(streamID, buffer, 32768)) >= 0)
                    length += curBuffer;

                Bass.BASS_ChannelSetPosition(streamID, 0L);
            }

            double resolution = 1 / (double)Settings.settings["waveformDetail"] / 100;
            int bpf = (int)Bass.BASS_ChannelSeconds2Bytes(streamID, resolution);

            int framesToRender = (int)Math.Ceiling(length / bpf);

            Level[] data = new Level[framesToRender];

            int bps = 2;

            // i dont care enough to go through these and figure out actual names right now
            int num1 = 0;
            int num2 = (int)Bass.BASS_ChannelSeconds2Bytes(streamID, 1.0);
            int num3 = num2 / bpf;
            int num4 = num2 / bps;
            int num5 = bpf / bps;

            bool killScan = false;
            short[]? peakLevelShort = null;
            Level currentLevel;

            try
            {
                while (!killScan)
                {
                    if (Bass.BASS_ChannelIsActive(streamID) == BASSActive.BASS_ACTIVE_STOPPED)
                    {
                        num1 = framesToRender;
                        killScan = true;
                    }
                    else
                    {
                        if (peakLevelShort == null || peakLevelShort.Length < num4)
                            peakLevelShort = new short[num4];

                        int num6 = 0;
                        num4 = Bass.BASS_ChannelGetData(streamID, peakLevelShort, num2) / bps;

                        for (int i = 0; i < num3; i++)
                        {
                            if (num6 + num5 > num4)
                                currentLevel = GetLevel(peakLevelShort, num6, num4 - num6);
                            else
                                currentLevel = GetLevel(peakLevelShort, num6, num5);

                            num6 += num5;

                            if (num1 < framesToRender)
                            {
                                data[num1] = currentLevel;
                                num1++;
                            }

                            if (num6 > num4)
                                break;
                        }
                    }
                }
            }
            catch { }

            // process data
            bool classic = Settings.settings["classicWaveform"];
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

            if (!isUploaded)
                Dispose();
            Upload();

            Console.WriteLine("Waveform rendered");
        }

        private static Level GetLevel(short[] buffer, int startIndex, int length)
        {
            Level currentLevel = default;

            if (buffer == null)
                return currentLevel;

            int num1 = startIndex + length;

            for (int i = startIndex; i < num1; i++)
            {
                short num2 = buffer[i];

                if (i % 2 == 0)
                    currentLevel.left = num2;
                else
                    currentLevel.right = num2;
            }

            return currentLevel;
        }

        private static void Upload()
        {
            GL.UseProgram(Shader.WaveformProgram);
            VaO = GL.GenVertexArray();
            VbO = GL.GenBuffer();
            posLocation = GL.GetUniformLocation(Shader.WaveformProgram, "WavePos");

            var color = Settings.settings["color4"];
            float[] c = new float[3] { color.R / 255f, color.G / 255f, color.B / 255f };
            int colorLocation = GL.GetUniformLocation(Shader.WaveformProgram, "LineColor");
            GL.Uniform3f(colorLocation, c[0], c[1], c[2]);

            GL.BindVertexArray(VaO);
            
            GL.BindBuffer(BufferTargetARB.ArrayBuffer, VbO);
            GL.BufferData(BufferTargetARB.ArrayBuffer, WaveModel, BufferUsageARB.StaticDraw);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindBuffer(BufferTargetARB.ArrayBuffer, BufferHandle.Zero);
            GL.BindVertexArray(VertexArrayHandle.Zero);

            isUploaded = true;
        }

        public static void Render(float startPos, float endPos, float trackHeight)
        {
            GL.UseProgram(Shader.WaveformProgram);
            GL.Uniform3f(posLocation, startPos, endPos - startPos, trackHeight);

            GL.BindVertexArray(VaO);
            GL.DrawArrays(PrimitiveType.LineStrip, 0, WaveModel.Length / 2);
        }

        private static void Dispose()
        {
            GL.DeleteVertexArray(VaO);
            GL.DeleteBuffer(VbO);

            isUploaded = false;
        }
    }
}
