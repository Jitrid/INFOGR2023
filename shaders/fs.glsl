/* Fragment Shader */
#version 330

/* Represents an individual light source. */
struct Light
{ 
    vec3 Position;
    vec3 Color;
};

in vec4 positionWorld;              // fragment position in World Space
in vec4 normalWorld;                // fragment normal in World Space
in vec2 uv;                         // fragment uv texture coordinates

uniform vec3 cameraPosition;
uniform Light lights[2];
uniform sampler2D diffuseTexture;

out vec4 fragmentColor;

void main()
{
    // Constant values
    vec3 diffuseColor = texture(diffuseTexture, uv).rgb;
    vec3 N = normalize(normalWorld.xyz);

    vec3 res = vec3(0);

    // Ambient lighting
    vec3 ambientLighting = vec3(0.1) * diffuseColor;
    
    // Apply Phong for all light sources.
    for (int i = 0; i < 2; i++ )
    {
        Light light = lights[i];

        // Diffuse lighting
        float r = distance(light.Position, positionWorld.xyz);
        vec3 L = normalize(light.Position - positionWorld.xyz);
        vec3 diffuseLighting = diffuseColor * max(0, dot(N, L));
        
        // Specular lighting
        vec3 R = reflect(-L, N);
        vec3 V = normalize(cameraPosition - positionWorld.xyz);
        float spec = pow(max(dot(V, R), 0.0), 32.0);
        vec3 specularLighting = vec3(1) * spec;

        res += (light.Color/(r * r) * (diffuseLighting + specularLighting));
    }

    res += ambientLighting * diffuseColor;
    fragmentColor = vec4(res, 1.0);
}
