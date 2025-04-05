#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 color;
layout (location = 2) in vec2 uv;
out vec3 rgb;
out vec3 p;
out vec2 uvo;

void main()
{
    gl_Position = vec4(aPosition, 1.0);
    rgb = color;
    p = aPosition;
    uvo = uv;
}