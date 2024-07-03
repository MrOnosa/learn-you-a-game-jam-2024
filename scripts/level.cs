using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class level : Node2D
{
    private gm global;
    [Export] public PackedScene GreenMobScene { get; set; }
    [Export] public PackedScene PinkMobScene { get; set; }
    [Export] public PackedScene PickableStaffScene { get; set; }

    /// <summary>
    /// The current stage, zero index based. The first stage the player plays is 0.
    /// </summary>
    [Export] public int CurrentStage { get; set; }
    public List<Stage> Stages { get; set; } = new List<Stage>();


    private Stats _stats;
    private Label _waveCountLabel;
    private Label _tutorialLabel;
    private CharacterBody2D _witch;

    public override void _Ready()
    {
        _stats = new Stats();
        global = GetNode<gm>("/root/GM");
        var audioPlayer = global.GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        audioPlayer.Stop();
        audioPlayer.Stream = GD.Load<AudioStream>("res://assets/music/Game_Jam_Rise_of_Rinkollette_Main_Song_trimmed_normalized.mp3");
        audioPlayer.Play();
        
        _witch = GetNode<CharacterBody2D>("Witch");
        _waveCountLabel = GetNode<Label>("CanvasLayer/WaveCountLabel");;
        _tutorialLabel = GetNode<Label>("CanvasLayer/Tutorial");
        
        // Tutorial logic
        if (!global.EverGotPastWaveOne)
        {
            // This one needs more lessons...
            global.EverHeldItem = false;
            global.EverHeldCorrectItem = false;
            _tutorialLabel.Visible = true;
        }

        if (global.GoblinMode)
        {
            global.EverGotPastWaveOne = false;
            _tutorialLabel.Visible = true;
        }
        
        GetNode<Label>("CanvasLayer/GoblinModeLabel").Visible = global.GoblinMode;
        if (global.GoblinMode)
        {
            // One thousand years ago...
             // Goblins ruled the scene...
            // There was no stopping the goblins...
            // The goblins gonna getcha...

            GetNode<Label>("CanvasLayer/GoblinModeLabel").Visible = true;
            Stages = new List<Stage>()
            {
                new Stage()
                {
                    TotalGreenGoblins = 2,
                    TotalPinkGoblins = 2
                }
            };
        }
        else
        {
            GetNode<Label>("CanvasLayer/GoblinModeLabel").Visible = false;
            Stages = new List<Stage>()
            {
                new Stage()
                {
                    TotalGreenGoblins = 0,
                    TotalPinkGoblins = 3
                },
                new Stage()
                {
                    TotalGreenGoblins = 3,
                    TotalPinkGoblins = 0
                },
                new Stage()
                {
                    TotalGreenGoblins = 2,
                    TotalPinkGoblins = 2
                },
                new Stage()
                {
                    TotalGreenGoblins = 10,
                    TotalPinkGoblins = 10
                }
            };
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        _stats.SurvivalTime += delta;
        
        // Tutorial logic
        if (global.EverGotPastWaveOne)
        {
            _tutorialLabel.Visible = false;
        } else if (global.EverKilledGoblin)
        {
            if (global.GoblinMode)
            {
                _tutorialLabel.Text = "GOBLIN MODE: The goblins never end!!";
            }
            else
            {
                _tutorialLabel.Text = "Defeat all goblins";
            }
        } else if (global.EverHeldCorrectItem)
        {
            _tutorialLabel.Text = "Shoot the goblins";
        } else if (global.EverHeldItem)
        {
            _tutorialLabel.Text = "Wrong staff!";
        }
        else
        {
            _tutorialLabel.Text = "Grab a staff!";
        }

        if (Input.IsActionPressed("C_Pause"))
        {
            GetTree().Paused = true;
            GetNode<CanvasLayer>("Pause/CanvasLayer").Show();
            GetNode<Button>("Pause/CanvasLayer/Panel/VBoxContainer/ResumeButton").GrabFocus();
        }
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
        var goblinBag = new List<char>();
        Stage currentStage = null;
        if(Stages.Count > CurrentStage )
            currentStage = Stages[CurrentStage];
        if (currentStage == null) return;
        
        for (int i = 0; i < currentStage.TotalGreenGoblins; i++)
        {
            goblinBag.Add('G');
        }

        for (int i = 0; i < currentStage.TotalPinkGoblins; i++)
        {
            goblinBag.Add('P');
        }

        if (goblinBag.Count == 0) return;

        var winner = goblinBag[GD.RandRange(0, goblinBag.Count - 1)];

        GD.Print("Winner "+winner);
        green_goblin mob = null;

        switch (winner)
        {
            case 'G':
                currentStage.TotalGreenGoblins--;
                mob = GreenMobScene.Instantiate<green_goblin>();
                break;
            case 'P':
                currentStage.TotalPinkGoblins--;
                mob = PinkMobScene.Instantiate<green_goblin>();
                break;
        }
        
        var mobSpawnLocation = GetNode<PathFollow2D>("Witch/MobPath2D/MobSpawnLocation");
        mobSpawnLocation.ProgressRatio = GD.Randf();

        // Set the mob's position to a random location.
        mob.GlobalPosition = mobSpawnLocation.GlobalPosition;
        mob.Dead += _mob_died_handler;

        // Spawn the mob by adding it to the Main scene.
        AddChild(mob);
    }

    private async void _mob_died_handler(ItemType type)
    {
        global.EverKilledGoblin = true;
        switch (type)
        {
            case ItemType.GreenStaff:
                _stats.TotalDeadGreenGoblins++;
                break;
            case ItemType.PinkStaff:
                _stats.TotalDeadPinkGoblins++;
                break;
        }
        // Short pause to let things settle. 
        await ToSignal(GetTree().CreateTimer(1), "timeout");
        
        Stage currentStage = null;
        if (CurrentStage < Stages.Count)
        {
            currentStage = Stages[CurrentStage];
        }
        
        if (currentStage == null 
            /* If the following is true, there are more mobs yet to be spawned. We should wait for all of them */
            || currentStage.TotalPinkGoblins + currentStage.TotalGreenGoblins > 0 ) return;
        
        // Check if any goblins are still on the screen
        var allGoblins = this.GetChildren().OfType<green_goblin>().ToList();
        if (allGoblins.Count == 0)
        {
            // The last mob died. New wave!
            CurrentStage++;
            global.EverGotPastWaveOne = true;
            _stats.Wave = CurrentStage + 1;
            
            if (global.GoblinMode)
            {
                // Goblins gonna goblin forever...
                var total = Mathf.FloorToInt((Mathf.Pow(CurrentStage, 1.5)) + 4.0);
                var green = GD.RandRange(0, total);
                Stages.Add(new Stage()
                {
                    TotalGreenGoblins = green,
                    TotalPinkGoblins = total - green
                });
            } 
            else if (CurrentStage >= Stages.Count)
            {
                //Victory!!!
            
                global.GameStats = _stats;
                GetTree().ChangeSceneToFile("res://scenes/victory.tscn");
                return;
            }
            _waveCountLabel.Text = (CurrentStage + 1).ToString();
        }
    }

    private void _on_staff_spawn_timer_timeout()
    {
        var allItems = this.GetChildren().OfType<item>().ToList();
        if (allItems.Count < 4)
        {
            var staff = PickableStaffScene.Instantiate<item>();
            var spawnLocation = GetNode<PathFollow2D>("Witch/StaffPath2D/StaffPathLocation");
            spawnLocation.ProgressRatio = GD.Randf();

            // Set the mob's position to a random location.
            staff.GlobalPosition = spawnLocation.GlobalPosition;
            staff.Type = GD.RandRange(0, 1) == 0 ? ItemType.GreenStaff : ItemType.PinkStaff;

            // Spawn the mob by adding it to the Main scene.
            AddChild(staff);
        }
    }
    
    private void _on_witch_health_changed(HealthUpdate healthUpdate)
    {
        var lifeBar = GetNode<life_bar>("CanvasLayer/LifeBar");
        lifeBar.Paint(healthUpdate.CurrentHealth);
    }
    private void _on_witch_item_changed(ItemType itemType)
    {
        global.EverHeldItem = true;
        if (this.GetChildren().OfType<green_goblin>().Any(g => g.WeakToType == itemType))
        {
            global.EverHeldCorrectItem = true;
        }
    }

    private void _on_witch_died()
    {
        //TODO: Death animation

        global.GameStats = _stats;
        GetTree().ChangeSceneToFile("res://scenes/game_over.tscn");
    }
}



public class Stage 
{
    public int TotalGreenGoblins = 5;
    public int TotalPinkGoblins = 6;
}

public class Stats
{
    public int TotalDeadGreenGoblins;
    public int TotalDeadPinkGoblins;
    public double SurvivalTime;
    public int Wave = 1;
}