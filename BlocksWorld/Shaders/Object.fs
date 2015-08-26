#version 330

layout(location = 0) out vec4 fragment;

in vec3 color;

void main()
{
	fragment = vec4(color, 1.0f);
}