using New_SSQE.ExternalUtils;
using New_SSQE.GUI.Shaders.Set;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace New_SSQE.GUI.Shaders
{
    internal static class Shader
    {
        // Texture0: Backgrounds
        // Texture1: Play/Pause Widget
        // Texture2: VFX Spritesheet

        // Texture3: Grid FrameBuffer

        // Texture12: Unicode Font
        // Texture13: "squareo" Font
        // Texture14: "square" Font
        // Texture15: "main" Font

        public static ProgramHandle Program;
        public static ProgramHandle TextureProgram;
        public static ProgramHandle FontProgram;
        public static ProgramHandle TrackProgram;
        public static ProgramHandle TimelineProgram;
        public static ProgramHandle ScalingProgram;
        public static ProgramHandle ColoredProgram;
        public static ProgramHandle XScalingProgram;
        public static ProgramHandle WaveformProgram;
        public static ProgramHandle UnicodeProgram;
        public static ProgramHandle IconTexProgram;
        public static ProgramHandle VFXNoteProgram;
        public static ProgramHandle VFXGridProgram;
        public static ProgramHandle VFXFBOProgram;


        private static int VFXNoteProj, VFXNoteView, VFXNoteTrans;
        private static int VFXNoteBCSB, VFXNoteTint;

        private static int VFXGridProj, VFXGridView, VFXGridTrans;
        private static int VFXGridBCSB, VFXGridTint;

        public static void Init()
        {
            Program = CompileShader(MainShader.Vertex, MainShader.Fragment, "Main");
            TextureProgram = CompileShader(TextureShader.Vertex, TextureShader.Fragment, "Texture");
            FontProgram = CompileShader(FontShader.Vertex, FontShader.Fragment, "Font");
            TrackProgram = CompileShader(TrackShader.Vertex, TrackShader.Fragment, "Track");
            TimelineProgram = CompileShader(TimelineShader.Vertex, TimelineShader.Fragment, "Timeline");
            ScalingProgram = CompileShader(ScalingShader.Vertex, ScalingShader.Fragment, "Scaling");
            ColoredProgram = CompileShader(ColoredShader.Vertex, ColoredShader.Fragment, "Colored");
            XScalingProgram = CompileShader(XScalingShader.Vertex, XScalingShader.Fragment, "XScaling");
            WaveformProgram = CompileShader(WaveformShader.Vertex, WaveformShader.Fragment, "Waveform");
            UnicodeProgram = CompileShader(UnicodeShader.Vertex, UnicodeShader.Fragment, "Unicode");
            IconTexProgram = CompileShader(IconTexShader.Vertex, IconTexShader.Fragment, "IconTex");
            VFXNoteProgram = CompileShader(VFXNoteShader.Vertex, VFXNoteShader.Fragment, "VFXNote");
            VFXGridProgram = CompileShader(VFXGridShader.Vertex, VFXGridShader.Fragment, "VFXGrid");
            VFXFBOProgram = CompileShader(VFXFBOShader.Vertex, VFXFBOShader.Fragment, "VFXFBO");

            GL.UseProgram(VFXNoteProgram);
            VFXNoteProj = GL.GetUniformLocation(VFXNoteProgram, "Projection");
            VFXNoteView = GL.GetUniformLocation(VFXNoteProgram, "View");
            VFXNoteTrans = GL.GetUniformLocation(VFXNoteProgram, "Transform");

            VFXNoteBCSB = GL.GetUniformLocation(VFXNoteProgram, "BCSB");
            VFXNoteTint = GL.GetUniformLocation(VFXNoteProgram, "Tint");

            GL.UseProgram(VFXGridProgram);
            VFXGridProj = GL.GetUniformLocation(VFXGridProgram, "Projection");
            VFXGridView = GL.GetUniformLocation(VFXGridProgram, "View");
            VFXGridTrans = GL.GetUniformLocation(VFXGridProgram, "Transform");

            VFXGridBCSB = GL.GetUniformLocation(VFXGridProgram, "BCSB");
            VFXGridTint = GL.GetUniformLocation(VFXGridProgram, "Tint");
        }

        private static ProgramHandle CompileShader(string vertShader, string fragShader, string tag)
        {
            ShaderHandle vs = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vs, vertShader);
            GL.CompileShader(vs);

            GL.GetShaderInfoLog(vs, out string vsLog);
            if (!string.IsNullOrWhiteSpace(vsLog))
                Logging.Register($"Failed to compile vertex shader with tag '{tag}' - {vsLog}", LogSeverity.ERROR);

            ShaderHandle fs = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fs, fragShader);
            GL.CompileShader(fs);

            GL.GetShaderInfoLog(fs, out string fsLog);
            if (!string.IsNullOrWhiteSpace(fsLog))
                Logging.Register($"Failed to compile fragment shader with tag '{tag}' - {fsLog}", LogSeverity.ERROR);

            ProgramHandle program = GL.CreateProgram();
            GL.AttachShader(program, vs);
            GL.AttachShader(program, fs);

            GL.LinkProgram(program);

            GL.DetachShader(program, vs);
            GL.DetachShader(program, fs);

            GL.DeleteShader(vs);
            GL.DeleteShader(fs);

            return program;
        }

        public static void UploadOrtho(ProgramHandle program, float w, float h)
        {
            Matrix4 ortho = Matrix4.CreateOrthographicOffCenter(0f, w, h, 0f, 0.0f, 1.0f);

            GL.UseProgram(program);
            int location = GL.GetUniformLocation(program, "Projection");
            GL.UniformMatrix4f(location, false, ortho);
        }

        public static void UploadOrtho(float w, float h)
        {
            UploadOrtho(Program, w, h);
            UploadOrtho(TextureProgram, w, h);
            UploadOrtho(FontProgram, w, h);
            UploadOrtho(TrackProgram, w, h);
            UploadOrtho(TimelineProgram, w, h);
            UploadOrtho(ScalingProgram, w, h);
            UploadOrtho(ColoredProgram, w, h);
            UploadOrtho(XScalingProgram, w, h);
            UploadOrtho(WaveformProgram, w, h);
            UploadOrtho(UnicodeProgram, w, h);
            UploadOrtho(IconTexProgram, w, h);
        }

        public static void SetPVT(Matrix4 proj, Matrix4 view, Matrix4 trans)
        {
            GL.UseProgram(VFXNoteProgram);
            GL.UniformMatrix4f(VFXNoteProj, false, proj);
            GL.UniformMatrix4f(VFXNoteView, false, view);
            GL.UniformMatrix4f(VFXNoteTrans, false, trans);

            GL.UseProgram(VFXGridProgram);
            GL.UniformMatrix4f(VFXGridProj, false, proj);
            GL.UniformMatrix4f(VFXGridView, false, view);
            GL.UniformMatrix4f(VFXGridTrans, false, trans);
        }

        public static void SetBCSBT(float brightness, float contrast, float saturation, float blur, Vector3 tint)
        {
            GL.UseProgram(VFXNoteProgram);
            GL.Uniform4f(VFXNoteBCSB, brightness, contrast, saturation, blur);
            GL.Uniform3f(VFXNoteTint, tint);

            GL.UseProgram(VFXGridProgram);
            GL.Uniform4f(VFXGridBCSB, brightness, contrast, saturation, blur);
            GL.Uniform3f(VFXGridTint, tint);
        }

        public static void SetBlur(float blur)
        {
            GL.UseProgram(VFXFBOProgram);
            int location = GL.GetUniformLocation(VFXFBOProgram, "offset");
            GL.Uniform1f(location, 1f / 2000f * (blur * 10f + 1f));
        }
    }
}
