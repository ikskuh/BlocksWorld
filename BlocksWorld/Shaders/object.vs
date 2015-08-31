#version 330

layout(location = 0) in vec3 vPosition;
layout(location = 1) in vec3 vNormal;
layout(location = 2) in vec3 vColor;
layout(location = 3) in vec3 vUV;

uniform mat4 uWorld;
uniform mat4 uWorldViewProjection;

out vec3 color;
out vec3 normal;
out vec3 uv;

void main()
{
	color = vColor;
	uv = vUV;
	normal = mat3x3(uWorld) * vNormal;
	gl_Position = uWorldViewProjection * vec4(vPosition, 1.0f);
}