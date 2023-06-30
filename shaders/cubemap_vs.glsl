/* Cubemap Vertex Shader */
#version 330

in vec3 vertexPosition;

uniform mat4 view;
uniform mat4 projection;

out vec3 uv;

void main()
{ 
	uv = vertexPosition;

	vec4 pos = view * projection * vec4(vertexPosition, 1.0);

	gl_Position = pos.xyww;
}
