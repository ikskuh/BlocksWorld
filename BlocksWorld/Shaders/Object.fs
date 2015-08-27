#version 330

layout(location = 0) out vec4 fragment;

in vec3 color;
in vec3 uv;

uniform sampler2DArray uTextures;

void main()
{
	fragment = texture(uTextures, uv) * vec4(color, 1.0f);
}