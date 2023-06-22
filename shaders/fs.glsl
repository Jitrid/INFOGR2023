#version 330
 
// shader inputs
in vec4 positionWorld;              // fragment position in World Space
in vec4 normalWorld;                // fragment normal in World Space
in vec2 uv;                         // fragment uv texture coordinates

uniform vec3 cameraPosition;
uniform vec3 lightPosition;
uniform vec3 lightColor;

uniform sampler2D diffuseTexture;	// texture sampler

// shader output
out vec4 outputColor;

// fragment shader
void main()
{
    outputColor = texture(diffuseTexture, uv) + 0.5 * normalWorld;
}