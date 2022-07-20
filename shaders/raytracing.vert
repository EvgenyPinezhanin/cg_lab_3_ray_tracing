#version 330

layout (location = 0) in vec3 vPos;

out vec3 glPosition;

void main()
{
	gl_Position = vec4(vPos, 1.0);
	glPosition = vPos;
}
