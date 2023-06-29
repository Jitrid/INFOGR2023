/* Fragment Shader */
#version 330

/* Represents an individual light source. */
struct Light
{ 
    vec3 Position;
    vec3 Color;
};

in vec4 positionWorld;
in vec4 normalWorld;
in vec2 uv;

uniform vec3 cameraPosition;
uniform int lightsCount;            // how many lights are in the scene.
uniform Light lights[10];           // the array of the light sources - maximum set to 10 as it must be a constant.

uniform sampler2D diffuseTexture;
uniform sampler2D normalTexture;

out vec4 fragmentColor;

// normal mapping
in mat3 TBN;
uniform int mappingEnabled; // 1 for true, 0 for false.

void main()
{
    // Constant values
    vec3 diffuseColor = texture(diffuseTexture, uv).rgb;

    // Set the normal based on whether or not normal mapping has been enabled.
    vec3 N = mappingEnabled == 1 ? normalize(TBN * (texture(normalTexture, uv).rgb * 2.0 - 1.0)) : normalize(normalWorld.xyz);

    vec3 res = vec3(0);

    // Ambient lighting
    vec3 ambientLighting = vec3(0.1) * diffuseColor;
    
    // Apply Phong for all light sources.
    for (int i = 0; i < lightsCount; i++ )
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
