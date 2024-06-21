using Godot;
using System;

public partial class witch : CharacterBody2D
{
    public const float Speed = 300.0f;
    public bool shooting = false;

    private Timer _timer;

    public override void _Ready()
    {
        _timer = GetNode<Timer>("ShootCooldownTimer");
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionPressed("Shoot"))
            Shoot();
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        // Get the input direction and handle the movement/deceleration.
        Vector2 direction = Input.GetVector("Left", "Right", "Up", "Down");
        if (direction != Vector2.Zero)
        {
            velocity.X = direction.X * Speed;
            velocity.Y = direction.Y * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    public void Shoot()
    {
        if (shooting == false)
        {
            GD.Print("Boom!");
            shooting = true;
            _timer.Start();
            var scene = GD.Load<PackedScene>("res://scenes/magic_bullet.tscn");
            var inst = scene.Instantiate<magic_bullet>();
            inst.GlobalPosition = GlobalPosition;
            
            
            Vector2 mousePos = GetViewport().GetMousePosition(); // Get the position of the mouse 
            Vector2 direction = (mousePos - inst.GlobalPosition).Normalized(); // Calculate the direction to the mouse from the bullet 

            Vector2 velocity = direction * magic_bullet.Speed;
    
            inst.Velocity = velocity;
            
            AddSibling(inst);
        }
    }

    private void _on_shoot_cooldown_timer_timeout()
    {
        shooting = false;
    }
}