#version 330

layout(location = 0) out vec4 fragment;

in vec2 uv;

uniform sampler2D uTexture;

void main()
{
	fragment = texture(uTexture, uv);
}