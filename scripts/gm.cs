using Godot;
using System;

public partial class gm : Node
{
	[Export] public bool UsingController = false;
	public Stats GameStats { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!UsingController && AnyControllerInput())
		{
			// Player swapped from Keyboard & Mouse to controller
			UsingController = true;
			ToggleMouse(!UsingController);
		}
		else if (UsingController && AnyKeyboardAndMouseInput())
		{
			// Player swapped from controller to Keyboard & Mouse
			UsingController = false;
			ToggleMouse(!UsingController);
		}
	}

	private void ToggleMouse(bool show)
	{
		if (show)
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		else
		{
			Input.MouseMode = Input.MouseModeEnum.Hidden;
		}
	}

	private bool AnyControllerInput()
	{
		return Input.IsActionPressed("AnyControllerInput");
	}

	private bool AnyKeyboardAndMouseInput()
	{
		return Input.IsActionPressed("Shoot");
	}
}
