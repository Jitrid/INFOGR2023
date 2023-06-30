/* Post-processing Vertex Shader */
#version 330

layout (location = 0) in vec3 vertexPosition;
layout (location = 1) in vec2 vertexUV;

out vec2 uv;
out vec2 positionFromBottomLeft;	// vertex position on the screen, with (0, 0) at the bottom left and (1, 1) at the top right

void main()
{
	// vertex position already in Screen Space so no transformation needed
	gl_Position = vec4(vertexPosition, 1.0);

	// pass the uv coordinate
	uv = vertexUV;

	positionFromBottomLeft = 0.5 * vertexPosition.xy + 0.5;
}
