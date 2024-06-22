using Godot;
using System;

public partial class green_goblin : Area2D
{
	[Export]
	public float Speed = 1.0f;
	
	private health_component _healthComponent;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_healthComponent = GetNode<health_component>("HealthComponent");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	//A physics process update to move towards the player
	public void MoveTowardsPlayer(Vector2 witchPosition, double delta)
	{
		Vector2 direction = (witchPosition - Position).Normalized(); // Calculate the direction to the player from the goblin 
		Vector2 velocity = direction * Speed;
		Translate(velocity);
	}
	
	private void _on_area_entered(Area2D area)
	{
		if (area is magic_bullet)
		{
			GD.Print("Bullet hit green goblin");
			area.QueueFree();
			
			// Handle damage
			_healthComponent.Damage(1);
		}
	}

	private void _on_health_component_died()
	{
		//TODO: Death animation or something
		QueueFree();
	}
}
