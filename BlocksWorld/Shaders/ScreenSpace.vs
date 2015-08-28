#version 330

layout(location = 0) in vec2 vPosition; 

uniform vec2 uUpperLeft;
uniform vec2 uSize;

out vec2 uv;

void main()
{
	uv = vPosition;
	gl_Position = vec4(2.0f * (uUpperLeft + vPosition * uSize) - 1.0f, 0.0f, 1.0f);
}