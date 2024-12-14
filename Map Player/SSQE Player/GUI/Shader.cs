using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace SSQE_Player.GUI
{
    internal class Shader
    {
        // Texture15: "main" font
        
        public static ProgramHandle Program;
        public static ProgramHandle ModelProgram;
        public static ProgramHandle FontTexProgram;

        private static readonly string vertexShader = @"#version 330 core
layout (location = 0) in vec3 position;
layout (location = 1) in vec4 color;
out vec4 pass_color;

uniform mat4 projectionMatrix;

void main()
{
    gl_Position = projectionMatrix * vec4(position, 1.0);
    pass_color = color;
}";

        private static readonly string fragmentShader = @"#version 330 core
in vec4 pass_color;
out vec4 out_Color;

void main()
{
    out_Color = pass_color;
}";

        private static readonly string modelVertShader = @"#version 330 core
layout (location = 0) in vec3 position;
layout (location = 1) in vec3 mPosition;
layout (location = 2) in vec4 mColor;
out vec4 pass_color;

uniform mat4 transformationMatrix;
uniform mat4 projectionMatrix;
uniform mat4 viewMatrix;

void main()
{
    mat4 m;
    m[0][0] = 1;
    m[1][1] = 1;
    m[2][2] = 1;
    m[3] = vec4(mPosition, 1.0);

	vec4 worldPos = m * transformationMatrix * vec4(position, 1.0);
	gl_Position = projectionMatrix * viewMatrix * worldPos;
	pass_color = mColor;
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

    texColor = vec4(TexColor.xyz, TexColor.w * (1.0f - aCharAlpha));
    texCoord = vec2(tx, ty);
}";

        private readonly static string fontTexFragShader = @"#version 330 core
out vec4 FragColor;

in vec4 texColor;
in vec2 texCoord;

uniform sampler2D texture0;

void main()
{
    FragColor = vec4(texColor.xyz, texture(texture0, texCoord).w * texColor.w);
}";

        private readonly static Dictionary<string, int> uniforms = [];

        public static void Init()
        {
            Program = CompileShader(vertexShader, fragmentShader);
            ModelProgram = CompileShader(modelVertShader, fragmentShader);
            FontTexProgram = CompileShader(fontTexVertShader, fontTexFragShader);

            uniforms.Add("VertexProjection", GL.GetUniformLocation(Program, "projectionMatrix"));
            uniforms.Add("ModelTransformation", GL.GetUniformLocation(ModelProgram, "transformationMatrix"));
            uniforms.Add("ModelProjection", GL.GetUniformLocation(ModelProgram, "projectionMatrix"));
            uniforms.Add("ModelView", GL.GetUniformLocation(ModelProgram, "viewMatrix"));
        }

        private static ProgramHandle CompileShader(string vertShader, string fragShader)
        {
            ShaderHandle vs = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vs, vertShader);
            GL.CompileShader(vs);

            ShaderHandle fs = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fs, fragShader);
            GL.CompileShader(fs);

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
            Matrix4 ortho = Matrix4.CreateOrthographicOffCenter(0.0f, w, h, 0.0f, 0.0f, 1.0f);

            GL.UseProgram(program);
            int location = GL.GetUniformLocation(program, "Projection");
            GL.UniformMatrix4f(location, false, ortho);
        }

        public static void SetTransform(Matrix4 transform)
        {
            GL.UseProgram(ModelProgram);
            GL.UniformMatrix4f(uniforms["ModelTransformation"], false, transform);
        }

        public static void SetProjection(Matrix4 projection)
        {
            GL.UseProgram(ModelProgram);
            GL.UniformMatrix4f(uniforms["ModelProjection"], false, projection);
        }

        public static void SetView(Matrix4 view)
        {
            GL.UseProgram(ModelProgram);
            GL.UniformMatrix4f(uniforms["ModelView"], false, view);
        }

        public static void SetProjView(Matrix4 projection, Matrix4 view)
        {
            GL.UseProgram(Program);
            Matrix4 projview = view * projection;
            GL.UniformMatrix4f(uniforms["VertexProjection"], false, projview);
        }
    }
}
