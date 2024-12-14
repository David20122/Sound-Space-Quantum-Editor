namespace New_SSQE.GUI.Shaders.Set
{
    internal static class VFXFBOShader
    {
        public readonly static string Vertex = @"#version 330 core
layout (location = 0) in vec2 aPosition;
layout (location = 1) in vec2 aTexCoord;

out vec2 texCoords;

void main()
{
    gl_Position = vec4(aPosition, 0.0f, 1.0f);
    texCoords = aTexCoord;
}";

        public readonly static string Fragment = @"#version 330 core
out vec4 FragColor;
in vec2 texCoords;

uniform sampler2D texture0;

uniform float offset;

const float kernel[9] = float[](
    1.0f / 16.0f, 2.0f / 16.0f, 1.0f / 16.0f,
    2.0f / 16.0f, 4.0f / 16.0f, 2.0f / 16.0f,
    1.0f / 16.0f, 2.0f / 16.0f, 1.0f / 16.0f
);

void main()
{
    vec2 offsets[9] = vec2[](
        vec2(-offset, offset),
        vec2(0.0f, offset),
        vec2(offset, offset),
        vec2(-offset, 0.0f),
        vec2(0.0f, 0.0f),
        vec2(offset, 0.0f),
        vec2(-offset, -offset),
        vec2(0.0f, -offset),
        vec2(offset, -offset)
    );

    vec3 samples[9];
    for (int i = 0; i < 9; i++)
        samples[i] = vec3(texture(texture0, texCoords.st + offsets[i]));

    vec3 col = vec3(0.0f);
    for (int i = 0; i < 9; i++)
        col += samples[i] * kernel[i];

    FragColor = vec4(col, 1.0f);
}";
    }
}
