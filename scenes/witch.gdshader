shader_type canvas_item;
uniform bool invincible = false;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	if(invincible)
	{
		COLOR = COLOR * (sin(TIME * 12.0) / 5.0 + 0.5);
	}
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
