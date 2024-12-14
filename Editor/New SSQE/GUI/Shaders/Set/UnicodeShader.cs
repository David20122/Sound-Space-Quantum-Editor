namespace New_SSQE.GUI.Shaders.Set
{
    internal static class UnicodeShader
    {
        public readonly static string Vertex = @"#version 330 core
layout (location = 0) in vec2 aPosition; // 0-1, 0-1
layout (location = 1) in vec4 aCharLayout; // x, y, s, c
layout (location = 2) in float aCharAlpha;

out vec4 texColor;
out vec2 texCoord;

uniform vec2 CharSize;
uniform vec4 TexColor;

uniform mat4 Projection;
                                                
void main()
{
    int yOff = int(aCharLayout.w / 256);
    int xOff = int(aCharLayout.w - yOff * 256);

    float tx = (xOff + 0.02f) / 256.0f + aPosition.x * CharSize.x;
    float ty = (yOff + 0.02f) / 256.0f + aPosition.y * CharSize.y;

    gl_Position = Projection * vec4(aCharLayout.xy + aPosition * aCharLayout.z, 0.0f, 1.0f);

    texColor = vec4(TexColor.xyz, TexColor.w * (1.0f - aCharAlpha));
    texCoord = vec2(tx, ty);
}";

        public static string Fragment => FontShader.Fragment;
    }
}
