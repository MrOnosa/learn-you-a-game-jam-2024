using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class level : Node2D
{
    [Export] public PackedScene GreenMobScene { get; set; }
    [Export] public PackedScene PinkMobScene { get; set; }
    [Export] public PackedScene PickableStaffScene { get; set; }
    private CharacterBody2D _witch;

    [Export] public int TotalGreenGoblins = 5;
    [Export] public int TotalPinkGoblins = 6;

    public override void _Ready()
    {
        _witch = GetNode<CharacterBody2D>("Witch");
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
        for (int i = 0; i < TotalGreenGoblins; i++)
        {
            goblinBag.Add('G');
        }

        for (int i = 0; i < TotalPinkGoblins; i++)
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
                TotalGreenGoblins--;
                mob = GreenMobScene.Instantiate<green_goblin>();
                break;
            case 'P':
                TotalPinkGoblins--;
                mob = PinkMobScene.Instantiate<green_goblin>();
                break;
        }
        
        var mobSpawnLocation = GetNode<PathFollow2D>("Witch/MobPath2D/MobSpawnLocation");
        mobSpawnLocation.ProgressRatio = GD.Randf();

        // Set the mob's position to a random location.
        mob.GlobalPosition = mobSpawnLocation.GlobalPosition;

        // Spawn the mob by adding it to the Main scene.
        AddChild(mob);
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
}