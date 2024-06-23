using Godot;
using System;

public partial class level : Node2D
{
	[Export]
	public PackedScene MobScene { get; set; }
	private CharacterBody2D _witch;

	[Export] public int TotalGreenGoblins = 5;
	
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

	private void _on_mob_spawn_timer_timeout()
	{
		if (TotalGreenGoblins > 0)
		{
			TotalGreenGoblins--;
			var mob = MobScene.Instantiate<green_goblin>();
			
			var mobSpawnLocation = GetNode<PathFollow2D>("Witch/MobPath2D/MobSpawnLocation");
			mobSpawnLocation.ProgressRatio = GD.Randf();
	        
			// Set the mob's position to a random location.
			mob.GlobalPosition = mobSpawnLocation.GlobalPosition;
			
			// Spawn the mob by adding it to the Main scene.
			AddChild(mob);
		}
	}
}
