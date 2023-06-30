#version 330

// shader inputs
in vec2 uv;						// fragment uv texture coordinates
in vec2 positionFromBottomLeft;
uniform sampler2D pixels;		// input texture (1st pass render target)
uniform int applyVignette = 1;
uniform int applyChrom = 1;

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
	float vignette = pow(0.25  / (0.25 + radius), 0.05);
	//float vignette = 0.3 + 0.7 * length(positionFromCenter);
	color *= vignette;
}
    if (applyChrom == 1) {
	// apply chromatic aberration 
	
	//Ook een radial aberration is beter
    //vec2 offset = positionFromCenter * 0.08;
    vec2 offset = vec2(0.003, 0.003) * length(positionFromBottomLeft);
	float r = texture(pixels, uv + offset).r;
	float b = texture(pixels, uv - offset).b;
	color = vec3(r, color.g, b);
}
outputColor = color;
	
	// apply dummy postprocessing effect
	float dist = length(positionFromBottomLeft);
	//outputColor *= sin(dist * 50.0) * 0.25 + 0.75;
}
