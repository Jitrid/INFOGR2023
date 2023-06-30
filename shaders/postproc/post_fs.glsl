/* Post-processing Fragment Shader */
#version 330

in vec2 uv;
in vec2 positionFromBottomLeft;

uniform sampler2D pixels;
uniform sampler2D lut;
// booleans
uniform int applyVignette = 0;
uniform int applyChrom = 0;
uniform int toggleLUT = 0;

out vec3 outputColor;

void main()
{
	// retrieve input pixel
	outputColor = texture(pixels, uv).rgb;
	vec3 color = texture(pixels, uv).rgb;
	vec2 positionFromCenter = positionFromBottomLeft - vec2(0.5, 0.5);

    float vignette = 1.0;
    if (applyVignette == 1)
    {
        // apply vignetting
        float radius = length(positionFromCenter);
        // linear vignette
        vignette = 1.0 - abs(radius * 10);
        // Clamp the vignette value between 0 and 1
        vignette = clamp(vignette, 0.0, 1.0);
    }

    if (applyChrom == 1) 
    {
        // apply chromatic aberration 
        vec2 offset = vec2(0.003, 0.003) * length(positionFromBottomLeft);
        float r = texture(pixels, uv + offset).r;
        float b = texture(pixels, uv - offset).b;
        color = vec3(r, color.g, b);
    }
    
    if (toggleLUT == 1) 
    {
        vec3 lutpos;
        lutpos.x = color.r * 7.0;
        lutpos.y = color.g * 7.0;
        lutpos.z = color.b * 7.0;
    
        float lutS = 512.0;
        float cellS = lutS / 8;
        vec2 lutUV;
        lutUV.x = (fract(lutpos.x) + floor(mod(lutpos.z, 2.0))) / 8.0;
        lutUV.y = (fract(lutpos.y) + floor(mod(lutpos.x, 2.0))) / 8.0;
        lutUV *= (cellS - 1.0) / lutS;
        lutUV += floor(lutpos.yz) / lutS;
    
        color =  texture(lut, lutUV).rgb;
    }
   
    outputColor = color * vignette;
}
