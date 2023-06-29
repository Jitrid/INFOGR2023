/* Cubemap Vertex Shader */
#version 330

in vec3 vertexPosition;

uniform mat4 viewMatrix;
uniform mat4 projectionMatrix;

out vec3 uv;

void main()
{ 
	gl_Position = viewMatrix * projectionMatrix * vec4(vertexPosition, 1.0);

	uv = vertexPosition;
}
