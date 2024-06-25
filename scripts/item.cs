using Godot;
using System;

public partial class item : Area2D
{
	private AnimatedSprite2D _animatedSprite2D;
	[Export] public ItemType Type = ItemType.GreenStaff;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animatedSprite2D.Frame = (int)Type;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}

public enum ItemType
{
	None,
	GreenStaff,
	PinkStaff
}