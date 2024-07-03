using Godot;
using System;

public partial class victory : Node2D
{
	private gm global;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		global = GetNode<gm>("/root/GM");
		if (global.GameStats != null)
		{
			var statsLabel = GetNode<Label>("CanvasLayer/StatsLabel");
			statsLabel.Text = $"You completed the game in {global.GameStats.SurvivalTime:0.00} seconds.";
		}

		var audioPlayer = global.GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		audioPlayer.Stop();
		audioPlayer.Stream = GD.Load<AudioStream>("res://assets/music/Game_Jam_Rise_of_Rinkollette_Victory_Song_trimmed_normalized.mp3");
		audioPlayer.Play();
		GetNode<Button>("CanvasLayer/VBoxContainer/RestartButton").GrabFocus();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_restart_button_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/level.tscn");
	}

	private void _on_credits_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/credits.tscn");
	}

	private void _on_return_to_main_menu_button_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/title.tscn");
	}

	private void _on_quit_game_button_pressed()
	{
		GetTree().Quit();
	}
}
