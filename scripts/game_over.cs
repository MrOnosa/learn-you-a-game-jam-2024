using Godot;
using System;

public partial class game_over : Node2D
{

    private gm global;
    private TextureRect _backgroundTextureRect;

    private double timeSinceSceneStarted = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
        _backgroundTextureRect = GetNode<TextureRect>("CanvasLayer/BackgroundTextureRect");
        
        
        global = GetNode<gm>("/root/GM");
        if (global.GameStats != null)
        {
            var statsLabel = GetNode<Label>("CanvasLayer/StatsLabel");
            statsLabel.Text =
                $"Wave {global.GameStats.Wave}: You defeated {global.GameStats.TotalDeadGreenGoblins} Green {(global.GameStats.TotalDeadGreenGoblins == 1 ? "Goblin" : "Goblins")}, {global.GameStats.TotalDeadPinkGoblins} Pink {(global.GameStats.TotalDeadPinkGoblins == 1 ? "Goblin" : "Goblins")}, and survived for {global.GameStats.SurvivalTime:0.00} seconds.";
        }

        var audioPlayer = global.GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        audioPlayer.Stop();
        audioPlayer.Stream = GD.Load<AudioStream>("res://assets/music/Game_Jam_Rise_of_Rinkollette_Defeat_Music_trimmed_normalized.mp3");
        audioPlayer.Play();
        GetNode<Button>("CanvasLayer/VBoxContainer/RestartButton").GrabFocus();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        timeSinceSceneStarted += delta;
        
        double duration = 10; // The duration over which the transformation should occur
        double startValue = 0.7;
        double endValue = 0.3;

        double t = Math.Max(0, timeSinceSceneStarted - 4) / duration;
        t = Math.Min(t, 1); // Ensure t doesn't exceed 1

        double interpolatedValue = startValue + t * (endValue - startValue);
        (_backgroundTextureRect.Material as ShaderMaterial)?.SetShaderParameter("difference", (float)interpolatedValue);
        
    }

    private void _on_restart_button_pressed()
    {
        GetTree().ChangeSceneToFile("res://scenes/level.tscn");
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