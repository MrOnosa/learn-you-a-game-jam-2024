using Godot;
using System;
using System.Linq;

public partial class life_bar : Node2D
{
    private const int Offset = 34;
    [Export] public PackedScene HeartScene { get; set; }

    private int _hp;
    private double _wiggleCount = 0.05;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Paint(10);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        _wiggleCount -= delta;
        if (_wiggleCount < 0)
        {
            _wiggleCount = 0.05;
            if (_hp < 5)
            {
                var x = 0;
                foreach (var child in GetChildren().OfType<AnimatedSprite2D>().ToList())
                {
                    child.Position = new Vector2(x + GD.RandRange(-1,1) , GD.RandRange(-1,1));
                    x += Offset;
                }
            }
        }
    }

    public void Paint(int hp)
    {
        _hp = hp;
        GD.Print("Setting life bar to "+hp);
        
        for (int i = 0; i < 5; i++)
        {
            foreach (var child in GetChildren())
            {
                child.QueueFree();
            }
        }

        var x = 0;
        var odd = hp % 2 == 1;
        for (int i = 0; i < hp / 2; i++)
        {
            var heart = HeartScene.Instantiate<AnimatedSprite2D>();
            heart.Position = new Vector2(x, 0);
            x += Offset;
            AddChild(heart);
        }

        if (odd)
        {
            var heart = HeartScene.Instantiate<AnimatedSprite2D>();
            heart.Position = new Vector2(x, 0);
            heart.Frame = 1;
            AddChild(heart);
        }
    }
}