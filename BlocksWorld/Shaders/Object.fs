#version 330

layout(location = 0) out vec4 fragment;

in vec3 color;
in vec3 uv;
in vec3 normal;

uniform sampler2DArray uTextures;

void main()
{
	vec3 light = normalize(vec3(1.5f, -2.0f, 1.0f));

	float l = 0.4f + 0.6f * clamp(-dot(normal, light), 0.0f, 1.0f);

	fragment = vec4(l,l,l,1.0f) * texture(uTextures, uv) * vec4(color, 1.0f);
}