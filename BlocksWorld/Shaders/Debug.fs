#version 330

layout(location = 0) out vec4 fragment;

uniform vec3 uColor;

void main()
{
	fragment = vec4(uColor, 1.0f);
}