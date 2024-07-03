using Godot;
using System;

public partial class gm : Node
{
	[Export] public bool UsingController = false;
	[Export] public float Volume;
	[Export] public float SfxVolume;
	
	private AudioStreamPlayer _audioStreamPlayer;
	public Stats GameStats { get; set; }
	public bool GoblinMode { get; set; }
	
	// Tutorial Flags
	public bool EverHeldItem = false;
	public bool EverHeldCorrectItem = false;
	public bool EverKilledGoblin = false;
	public bool EverGotPastWaveOne = false;

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
			config.SetValue("Options", "sfx_volume", SfxVolume);
			config.Save("user://settings.cfg");
		}
		else
		{
			Volume = (float)config.GetValue("Options", "volume");
			SfxVolume = (float)config.GetValue("Options", "sfx_volume");
		}

		SetVolume(Volume);
		SetSfxVolume(SfxVolume);
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

	public float ConvertVolumeToDbVolume(float volume)
	{
		var dbVolume = Mathf.LinearToDb(volume);
		if (dbVolume > 3.0) dbVolume = 3.0f;
		return dbVolume;
	}
	
	public void SetVolume(float volume)
	{
		Volume = volume;
		var dbVolume = ConvertVolumeToDbVolume(Volume);
		_audioStreamPlayer.VolumeDb = dbVolume;
		GD.Print("dbVolume " + dbVolume);
		
		// Save setting to config after a moment
		var timer = GetNode<Timer>("SettingsDebouncingTimer");
		if (timer != null)
		{
			timer.Start(1);
		}
	}
	public void SetSfxVolume(float volume, bool test = false)
	{
		SfxVolume = volume;
		var dbVolume = ConvertVolumeToDbVolume(SfxVolume);
		GD.Print("SfxVolume dbVolume " + dbVolume);

		if (test)
		{
			// Test the sound
			var sfxPlayer = GetNode<AudioStreamPlayer>("SfxAudioStreamPlayer");
			sfxPlayer.VolumeDb = dbVolume;
			if (!sfxPlayer.Playing)
			{
				sfxPlayer.Play();
			}
		}

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
			config.SetValue("Options", "sfx_volume", SfxVolume);
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
