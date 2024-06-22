using Godot;
using System;

public partial class green_goblin : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void _on_area_entered(Area2D area)
	{
		if (area is magic_bullet)
		{
			GD.Print("Bullet hit green goblin");
			area.QueueFree();
		}
	}
}