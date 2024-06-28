using Godot;
using System;

public partial class title : Control
{
	private gm global;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		global = GetNode<gm>("/root/GM");
		GetNode<Button>("VBoxContainer/StartButton").GrabFocus();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void _on_start_button_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/level.tscn");
		var audioPlayer = global.GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		audioPlayer.Stop();
		audioPlayer.Stream = GD.Load<AudioStream>("res://assets/Game_Jam_Main_Song.mp3");
		audioPlayer.Play();
	}
	
	private void _on_quit_game_button_pressed()
	{
		GetTree().Quit();
	}
	
}
