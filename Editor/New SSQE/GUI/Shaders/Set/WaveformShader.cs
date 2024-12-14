namespace New_SSQE.GUI.Shaders.Set
{
    internal static class WaveformShader
    {
        public readonly static string Vertex = @"#version 330 core
layout (location = 0) in vec2 aPosition;
out vec4 vertexColor;

uniform mat4 Projection;
uniform vec3 WavePos;
uniform vec3 LineColor;
                                                
void main()
{
    gl_Position = Projection * vec4(aPosition.x * WavePos.y + WavePos.x, (aPosition.y + 1) * (WavePos.z * 0.5f), 0.0f, 1.0f);
    vertexColor = vec4(LineColor, 1.0f);
}";

        public static string Fragment => MainShader.Fragment;
    }
}
