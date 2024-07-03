using Godot;
using System;

public partial class pause : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	private void _on_menu_button_pressed()
	{
		GetTree().Paused = true;
		GetNode<CanvasLayer>("CanvasLayer").Show();
		GetNode<Button>("CanvasLayer/Panel/VBoxContainer/ResumeButton").GrabFocus();
	}

	private void _on_resume_button_pressed()
	{
		GetNode<CanvasLayer>("CanvasLayer").Hide();
		GetTree().Paused = false;
	}

	private void _on_main_menu_button_pressed()
	{
		GetNode<CanvasLayer>("CanvasLayer").Hide();
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile("res://scenes/title.tscn");
	}
}
