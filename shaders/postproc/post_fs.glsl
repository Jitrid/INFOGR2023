#version 330

// shader inputs
in vec2 uv;						// fragment uv texture coordinates
in vec2 positionFromBottomLeft;
uniform sampler2D pixels;		// input texture (1st pass render target)
uniform int applyVignette = 1;
uniform int applyChrom = 1;
uniform int toggleLUT = 1;
uniform sampler2D lut;

// shader output
out vec3 outputColor;

// fragment shader
void main()
{

	// retrieve input pixel
	outputColor = texture(pixels, uv).rgb;
	vec3 color = texture(pixels, uv).rgb;
	vec2 positionFromCenter = positionFromBottomLeft - vec2(0.5, 0.5);

    if (applyVignette == 1){
    // apply vignetting	  
    float radius = length(positionFromCenter);
    //Een radial vignette lijkt meer op foto's
	float vignette = pow(0.25  / (0.25 + radius), 0.5);
	color *= vignette;
}
 if (applyChrom == 1) {
        // apply chromatic aberration 
        vec2 offset = vec2(0.003, 0.003) * length(positionFromBottomLeft);
        float r = texture(pixels, uv + offset).r;
        float b = texture(pixels, uv - offset).b;
        color = vec3(r, color.g, b);
    }
    
    if (toggleLUT == 1) {
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
   
	//vec2 lutS = textureSize(lut, 0);
	//float slice = 1.0 / lutS.y;
	//vec2 slicePixelS = vec2(1.0) / lutS;
	//vec2 sliceInnerS = slicePixelS * (lutS.x - 1.0);
	//vec2 uv = color.xy * sliceInnerS + slicePixelS * 0.5;
	//float z0 = min(floor(color.z * lutS.y), lutS.y - 1.0);
	//float z1 = min(z0 + 1.0, lutS.y - 1.0);
	//vec2 uv0 = uv + vec2(0.0, z0) * slicePixelS;
	//vec2 uv1 = uv + vec2(0.0, z1) * slicePixelS;
//	vec3 gradedColor0 = texture(lut, uv0).rgb;
//	vec3 gradedColor1 = texture(lut, uv1).rgb;
//	vec3 gradedColor = mix(gradedColor0, gradedColor1, fract(color.z * lutS.y));
	

    outputColor = color;
	
}
