using Godot;
using System;

public partial class magic_bullet : Area2D
{
	public const float Speed = 3.0f;
	
	[Export] 
	public Vector2 Velocity { get; set; } = Vector2.Zero;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		Translate(Velocity);
	}
}
