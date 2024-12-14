namespace New_SSQE.GUI.Shaders.Set
{
    internal static class XScalingShader
    {
        public readonly static string Vertex = @"#version 330 core
layout (location = 0) in vec2 aPosition;
layout (location = 1) in vec4 aColor; // only using one component but inputting vec4 for compatibility
layout (location = 2) in vec4 aOffset; // x, y, s/a, c
                                               
out vec4 vertexColor;

uniform mat4 Projection;
uniform vec4 NoteColors[32];
                                                
void main()
{
    int s = int(aOffset.z * 0.5);
    float a = aOffset.z - s * 2;

    vec4 color = NoteColors[int(aOffset.w)];

    gl_Position = Projection * vec4(aPosition.x * s + aOffset.x, aPosition.y + aOffset.y, 0.0f, 1.0f);
    vertexColor = vec4(color.xyz, aColor.w * a);
}";

        public static string Fragment => MainShader.Fragment;
    }
}
