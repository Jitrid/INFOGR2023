/* Vertex Shader */
#version 330

layout (location = 0) in vec3 vertexPosition;		// vertex position in Object Space
layout (location = 1) in vec3 vertexNormal;			// vertex normal in Object Space
layout (location = 2) in vec2 vertexUV;				// vertex uv texture coordinates

uniform mat4 objectToScreen;
uniform mat4 objectToWorld;

// Will be interpolated from vertices to fragments.
out vec4 positionWorld;			// vertex position in World Space
out vec4 normalWorld;			// vertex normal in World Space
out vec2 uv;					// vertex uv texture coordinates (pass-through)

void main()
{
	// transform vertex position to 2D Screen Space + depth
	gl_Position = objectToScreen * vec4(vertexPosition, 1.0);

	// transform vertex position and normal to an appropriate space for shading calculations
    positionWorld = objectToWorld * vec4(vertexPosition, 1.0);
	normalWorld = inverse(transpose(objectToWorld)) * vec4(vertexNormal, 0);

	// pass the uv coordinate
	uv = vertexUV;
}
