using Godot;
using System;

public partial class title : Control
{
	private gm global;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		global = GetNode<gm>("/root/GM");
		var audioPlayer = global.GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		audioPlayer.Stop();
		audioPlayer.Stream = GD.Load<AudioStream>("res://assets/music/Game_Jam_Rise_of_Rinkollette_Title_Song_trimmed_normalized.mp3");
		audioPlayer.Play();
		GetNode<Button>("VBoxContainer/StartButton").GrabFocus();

		var slider = GetNode<HSlider>("VBoxContainer/HBoxContainer/VolumeSlider");
		slider.Value = global.Volume * 100;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void _on_start_button_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/level.tscn");
	}

	private void _on_credits_button_pressed()
	{
		
		GetTree().ChangeSceneToFile("res://scenes/credits.tscn");
	}
	
	private void _on_quit_game_button_pressed()
	{
		GetTree().Quit();
	}

	private void _on_volume_slider_value_changed(float value)
	{
		global.SetVolume(value / 100.0f);
	}
	
}
