using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class level : Node2D
{
    [Export] public PackedScene GreenMobScene { get; set; }
    [Export] public PackedScene PinkMobScene { get; set; }
    [Export] public PackedScene PickableStaffScene { get; set; }

    /// <summary>
    /// The current stage, zero index based. The first stage the player plays is 0.
    /// </summary>
    [Export] public int CurrentStage { get; set; }
    public List<Stage> Stages { get; set; } = new List<Stage>();

    
    private Label _waveCountLabel;
    private CharacterBody2D _witch;
    public override void _Ready()
    {
        _witch = GetNode<CharacterBody2D>("Witch");
        _waveCountLabel = GetNode<Label>("CanvasLayer/WaveCountLabel");

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
                TotalGreenGoblins = 10,
                TotalPinkGoblins = 10
            },
            new Stage()
            {
                TotalGreenGoblins = 100,
                TotalPinkGoblins = 100
            }
        };
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
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

    private async void _mob_died_handler()
    {
        // Short pause to let things settle. 
        await ToSignal(GetTree().CreateTimer(1), "timeout");
        
        Stage currentStage = null;
        if(Stages.Count > CurrentStage )
            currentStage = Stages[CurrentStage];
        if (currentStage == null 
            /* If the following is true, there are more mobs yet to be spawned. We should wait for all of them */
            || currentStage.TotalPinkGoblins + currentStage.TotalGreenGoblins > 0 ) return;
        
        // Check if any goblins are still on the screen
        var allGoblins = this.GetChildren().OfType<green_goblin>().ToList();
        if (allGoblins.Count == 0)
        {
            // The last mob died. New wave!
            CurrentStage++;
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
}



public partial class Stage 
{
    [Export] public int TotalGreenGoblins = 5;
    [Export] public int TotalPinkGoblins = 6;
}