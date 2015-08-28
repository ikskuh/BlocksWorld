#version 330

layout(location = 0) out vec4 fragment;

in vec2 uv;

uniform float uTextureID;
uniform sampler2DArray uTextures;

void main()
{
	fragment = texture(uTextures, vec3(uv, uTextureID));
}