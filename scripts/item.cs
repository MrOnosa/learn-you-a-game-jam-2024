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

	private void _on_life_timer_timeout()
	{
		QueueFree();
	}

	private void _on_visible_on_screen_notifier_2d_screen_exited()
	{
		var lifeTimer = GetNode<Timer>("LifeTimer");
		lifeTimer.Start();
	}

	private void _on_visible_on_screen_notifier_2d_screen_entered()
	{
		var lifeTimer = GetNode<Timer>("LifeTimer");
		lifeTimer.Stop();
	}
}

public enum ItemType
{
	None,
	GreenStaff,
	PinkStaff
}