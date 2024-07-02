using Godot;
using System;

public partial class gm : Node
{
	[Export] public bool UsingController = false;
	[Export] public float Volume;
	
	private AudioStreamPlayer _audioStreamPlayer;
	public Stats GameStats { get; set; }
	public bool GoblinMode { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		var config = new ConfigFile();
		Error err = config.Load("user://settings.cfg");
		if (err != Error.Ok)
		{
			// Try creating one instead
			config = new ConfigFile();
			config.SetValue("Options", "volume", Volume);
			config.Save("user://settings.cfg");
		}
		else
		{
			Volume = (float)config.GetValue("Options", "volume");
		}

		SetVolume(Volume);
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

	public void SetVolume(float volume)
	{
		Volume = volume;
		var dbVolume = Mathf.LinearToDb(Volume);
		if (dbVolume > 3.0) dbVolume = 3.0f;
		_audioStreamPlayer.VolumeDb = dbVolume;
		GD.Print("dbVolume " + dbVolume);
		
		// Save setting to config after a moment
		var timer = GetNode<Timer>("SettingsDebouncingTimer");
		if (timer != null)
		{
			timer.Start(1);
		}
		
	}

	private void _on_settings_debouncing_timer_timeout()
	{
		GD.Print("Saving settings.");
		var config = new ConfigFile();
		Error err = config.Load("user://settings.cfg");
		if (err == Error.Ok)
		{
			config.SetValue("Options", "volume", Volume);
			config.Save("user://settings.cfg");
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
