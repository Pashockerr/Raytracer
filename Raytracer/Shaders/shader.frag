#version 330 core
out vec4 FragColor;
in vec3 rgb;
in vec3 p;
in vec2 uvo;

uniform sampler2D tex;
void main()
{
    FragColor = texture(tex, uvo);
}