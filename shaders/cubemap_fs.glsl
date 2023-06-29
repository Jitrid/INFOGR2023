/* Cubemap Fragment Shader */
#version 330

in vec3 uv;

uniform samplerCube skybox;

out vec4 fragmentColor;

void main()
{
	fragmentColor = texture(skybox, uv);
}
