shader_type canvas_item;
uniform bool hurt = false;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	if(hurt)
	{
		COLOR.gb = vec2(COLOR.gb * (sin(TIME * 12.0) / 5.0 + 0.5));
	}
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
