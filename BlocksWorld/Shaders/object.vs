#version 330

layout(location = 0) in vec3 vPosition;
layout(location = 1) in vec3 vNormal;
layout(location = 2) in vec3 vColor;

uniform mat4 uWorldViewProjection;

out vec3 color;

void main()
{
	vec3 light = normalize(vec3(1.5f, -2.0f, 1.0f));

	float l = 0.4f + 0.6f * clamp(-dot(vNormal, light), 0.0f, 1.0f);
	color = vec3(l) * vColor;
	gl_Position = uWorldViewProjection * vec4(vPosition, 1.0f);
}