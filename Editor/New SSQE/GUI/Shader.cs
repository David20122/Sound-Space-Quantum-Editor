using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;

namespace New_SSQE.GUI
{
    internal class Shader
    {
        // Texture0: Backgrounds
        // Texture1: Play/Pause Widget

        // Texture13: "squareo" Font
        // Texture14: "square" Font
        // Texture15: "main" Font

        public static ProgramHandle Program;
        public static ProgramHandle TexProgram;
        public static ProgramHandle FontTexProgram;
        public static ProgramHandle InstancedProgram;
        public static ProgramHandle GridInstancedProgram;
        public static ProgramHandle WaveformProgram;

        private readonly static string vertexShader = @"#version 330 core
                                               layout (location = 0) in vec2 aPosition;
                                               layout (location = 1) in vec4 aColor;
                                               out vec4 vertexColor;

                                               uniform mat4 Projection;
                                                
                                               void main()
                                               {
                                                   gl_Position = Projection * vec4(aPosition.x, aPosition.y, 0.0f, 1.0f);
                                                   vertexColor = aColor;
                                               }";

        private readonly static string fragmentShader = @"#version 330 core
                                               out vec4 FragColor;
                                               in vec4 vertexColor;
                                                
                                               void main()
                                               {
                                                   FragColor = vertexColor;
                                               }";

        private readonly static string texVertShader = @"#version 330 core
                                               layout (location = 0) in vec2 aPosition;
                                               layout (location = 1) in vec2 aTexCoord;
                                               layout (location = 2) in float aAlpha;

                                               out vec2 texCoord;
                                               out float alpha;

                                               uniform mat4 Projection;
                                                
                                               void main()
                                               {
                                                   gl_Position = Projection * vec4(aPosition.x, aPosition.y, 0.0f, 1.0f);
                                                   texCoord = aTexCoord;
                                                   alpha = aAlpha;
                                               }";

        private readonly static string texFragShader = @"#version 330 core
                                               out vec4 FragColor;
                                               in vec2 texCoord;
                                               in float alpha;

                                               uniform sampler2D texture0;
                                                
                                               void main()
                                               {
                                                   vec4 color = texture(texture0, texCoord);
                                                   FragColor = vec4(color.x, color.y, color.z, color.w * alpha);
                                               }";

        private readonly static string fontTexVertShader = @"#version 330 core
                                               layout (location = 0) in vec2 aPosition;
                                               layout (location = 1) in vec4 aCharLayout;
                                               layout (location = 2) in float aCharAlpha;

                                               out vec4 texColor;
                                               out vec2 texCoord;

                                               uniform vec4 TexLookup[128];
                                               uniform vec2 CharSize;
                                               
                                               uniform vec4 TexColor;
                                               uniform mat4 Projection;
                                                
                                               void main()
                                               {
                                                   vec4 texLocation = TexLookup[int(aCharLayout.w)];

                                                   float x = aCharLayout.x + aPosition.x * aCharLayout.z;
                                                   float y = aCharLayout.y + aPosition.y * aCharLayout.z;
                                                   float tx = texLocation.x + texLocation.z * (aPosition.x / CharSize.x);
                                                   float ty = texLocation.y + texLocation.w * (aPosition.y / CharSize.y);

                                                   gl_Position = Projection * vec4(x, y, 0.0f, 1.0f);

                                                   texColor = vec4(TexColor.x, TexColor.y, TexColor.z, TexColor.w * (1.0f - aCharAlpha));
                                                   texCoord = vec2(tx, ty);
                                               }";

        private readonly static string fontTexFragShader = @"#version 330 core
                                               out vec4 FragColor;

                                               in vec4 texColor;
                                               in vec2 texCoord;

                                               uniform sampler2D texture0;
                                               
                                               void main()
                                               {
                                                   FragColor = vec4(texColor.x, texColor.y, texColor.z, texture(texture0, texCoord).w * texColor.w);
                                               }";

        private readonly static string instancedVertShader = @"#version 330 core
                                               layout (location = 0) in vec2 aPosition;
                                               layout (location = 1) in vec4 aColor; // only using one component but inputting vec4 for compatibility
                                               layout (location = 2) in vec4 aOffset; // x, r/y, g, b
                                                                                      // due to needing Y and vec5 not existing, Y = int(r / 2) and R = r - Y * 2
                                               out vec4 vertexColor;

                                               uniform mat4 Projection;
                                                
                                               void main()
                                               {
                                                   int yf = int(aOffset.y * 0.5);

                                                   gl_Position = Projection * vec4(aPosition.x + aOffset.x, aPosition.y + yf, 0.0f, 1.0f);
                                                   vertexColor = vec4(aOffset.y - yf * 2, aOffset.z, aOffset.w, aColor.w);
                                               }";

        private readonly static string instancedFragShader = @"#version 330 core
                                               out vec4 FragColor;
                                               in vec4 vertexColor;
                                                
                                               void main()
                                               {
                                                   FragColor = vertexColor;
                                               }";

        private readonly static string gridInstancedVertShader = @"#version 330 core
                                               layout (location = 0) in vec2 aPosition;
                                               layout (location = 1) in vec4 aColor;
                                               layout (location = 2) in vec4 aOffset; // x/r, y/g, w/b, h/a
                                                                                      // vector8 moment?
                                               out vec4 vertexColor;

                                               uniform mat4 Projection;
                                                
                                               void main()
                                               {
                                                   // first half of component: int(a * 0.5) - divide by 2 and take its integer portion
                                                   // second half of component: subtract 2 * first from the component
                                                   int x = int(aOffset.x * 0.5);
                                                   int y = int(aOffset.y * 0.5);
                                                   int w = int(aOffset.z * 0.5);
                                                   int h = int(aOffset.w * 0.5);

                                                   float r = aOffset.x - x * 2;
                                                   float g = aOffset.y - y * 2;
                                                   float b = aOffset.z - w * 2;
                                                   float a = aOffset.w - h * 2;

                                                   gl_Position = Projection * vec4(x + aPosition.x * w, y + aPosition.y * h, 0.0f, 1.0f);
                                                   vertexColor = vec4(r, g, b, a * aColor.w);
                                               }";

        private readonly static string gridInstancedFragShader = @"#version 330 core
                                               out vec4 FragColor;
                                               in vec4 vertexColor;
                                                
                                               void main()
                                               {
                                                   FragColor = vertexColor;
                                               }";

        private readonly static string waveformVertShader = @"#version 330 core
                                               layout (location = 0) in vec2 aPosition;
                                               out vec4 vertexColor;

                                               uniform mat4 Projection;
                                               uniform vec3 WavePos;
                                               uniform vec3 LineColor;
                                                
                                               void main()
                                               {
                                                   float yOff = WavePos.z * 0.5f;
                                                   float x = aPosition.x * WavePos.y + WavePos.x;
                                                   float y = (aPosition.y + 1) * yOff;
                                                   float cy = abs(aPosition.y);

                                                   gl_Position = Projection * vec4(x, y, 0.0f, 1.0f);
                                                   //vertexColor = vec4(cy, (1.0f - cy) * 0.5f, 0.0f, 1.0f);
                                                   vertexColor = vec4(LineColor, 1.0f);
                                               }";

        private readonly static string waveformFragShader = @"#version 330 core
                                               out vec4 FragColor;
                                               in vec4 vertexColor;
                                                
                                               void main()
                                               {
                                                   FragColor = vertexColor;
                                               }";

        public static void Init()
        {
            Program = CompileShader(vertexShader, fragmentShader, "Main");
            TexProgram = CompileShader(texVertShader, texFragShader, "Texture");
            FontTexProgram = CompileShader(fontTexVertShader, fontTexFragShader, "Font");
            InstancedProgram = CompileShader(instancedVertShader, instancedFragShader, "Instancing");
            GridInstancedProgram = CompileShader(gridInstancedVertShader, gridInstancedFragShader, "Grid Instancing");
            WaveformProgram = CompileShader(waveformVertShader, waveformFragShader, "Waveform");
        }

        private static ProgramHandle CompileShader(string vertShader, string fragShader, string tag)
        {
            ShaderHandle vs = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vs, vertShader);
            GL.CompileShader(vs);

            GL.GetShaderInfoLog(vs, out string vsLog);
            if (!string.IsNullOrWhiteSpace(vsLog))
                ActionLogging.Register($"Failed to compile vertex shader with tag '{tag}' - {vsLog}", "ERROR");

            ShaderHandle fs = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fs, fragShader);
            GL.CompileShader(fs);

            GL.GetShaderInfoLog(fs, out string fsLog);
            if (!string.IsNullOrWhiteSpace(fsLog))
                ActionLogging.Register($"Failed to compile fragment shader with tag '{tag}' - {fsLog}", "ERROR");

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
    }
}
