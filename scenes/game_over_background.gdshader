shader_type canvas_item;

uniform float difference: hint_range(0.0, 1.0) = 0.3;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	//https://stackoverflow.com/a/60068021
    vec4 tex = texture(TEXTURE, UV);
    COLOR.rgb = tex.rgb - vec3(difference);
    COLOR.a = tex.a;
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
