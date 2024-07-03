using Godot;
using System;

public partial class credits : Node2D
{
	private gm global;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		global = GetNode<gm>("/root/GM");
		var audioPlayer = global.GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		audioPlayer.Stop();
		audioPlayer.Stream = GD.Load<AudioStream>("res://assets/music/RoR_Credits_Song_trimmed_normalized.mp3");
		audioPlayer.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_button_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/title.tscn");
	}
}
