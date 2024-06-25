using Godot;
using System;

public partial class Inventory : TextureRect
{
	private AnimatedSprite2D _animatedSprite2D;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animatedSprite2D.Frame = (int)ItemType.None;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_witch_item_changed(ItemType itemType)
	{
		_animatedSprite2D.Frame = (int)itemType;
	}
}
