using Godot;
using System;

public partial class level : Node2D
{
	private CharacterBody2D _witch;
	
	public override void _Ready()
	{
		_witch = GetNode<CharacterBody2D>("Witch");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public override void _PhysicsProcess(double delta)
	{
		//GD.Print("ps");
		foreach (var child in GetChildren())
		{
			
			if (child is green_goblin goblin)
			{
				//GD.Print("move");
				goblin.MoveTowardsPlayer(_witch.Position, delta);
			}
		}

		
	}
}
