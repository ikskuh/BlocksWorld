#version 330

layout(location = 0) in vec2 vPosition; 

out vec2 uv;

void main()
{
	uv = vPosition;
	uv.y = 1.0f - uv.y;
	gl_Position = vec4(2.0f * vPosition - vec2(1.0f), 0.0f, 1.0f);
}