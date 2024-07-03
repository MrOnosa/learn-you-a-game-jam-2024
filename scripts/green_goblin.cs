using Godot;

public partial class green_goblin : Area2D
{
    [Export] public float Speed = 1.0f;

    [Export] public float BulletFireWaitTime = 1.0f;
    [Export] public float BulletFireVariance = 0.5f;
    [Export] public ItemType WeakToType = ItemType.GreenStaff;
    
    
    [Signal]
    public delegate void DeadEventHandler(ItemType type);

    private AnimatedSprite2D _animatedSprite2D;
    private health_component _healthComponent;
    private Timer _shootBulletTimer;
    private Timer _hurtTimer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _healthComponent = GetNode<health_component>("HealthComponent");
        _shootBulletTimer = GetNode<Timer>("ShootBulletTimer");
        _hurtTimer = GetNode<Timer>("HurtTimer");

        SetShootBulletTimerWaitTimeToARandomRange();


        var sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        if (sprite.Material is ShaderMaterial shader)
        {
            sprite.Material = shader.Duplicate() as ShaderMaterial;
        }
        //_animatedSprite2D.Autoplay
    }

    private void SetShootBulletTimerWaitTimeToARandomRange()
    {
        if (_shootBulletTimer != null)
        {
            _shootBulletTimer.WaitTime = GD.RandRange(BulletFireWaitTime * BulletFireVariance,
                BulletFireWaitTime + (BulletFireWaitTime * BulletFireVariance));
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    //A physics process update to move towards the player
    public void MoveTowardsPlayer(Vector2 witchPosition, double delta)
    {
        Vector2
            direction = (witchPosition - Position)
                .Normalized(); // Calculate the direction to the player from the goblin 
        Vector2 velocity = direction * Speed;
        Translate(velocity);

        if (velocity.X < 0)
        {
            _animatedSprite2D.Scale = new Vector2(-1, 1);
        }
        else
        {
            _animatedSprite2D.Scale = new Vector2(1, 1);
        }
    }

    private void _on_area_entered(Area2D area)
    {
        if (area is magic_bullet bullet)
        {
            if (bullet.FriendlyFire)
            {
                GD.Print("Bullet hit green goblin");
                area.QueueFree();


                if (bullet.Velocity != null)
                {
                    Position += bullet.Velocity * (float)1;
                }

                if (bullet.Type == WeakToType)
                {
                    // Handle damage
                    _healthComponent.Damage(1);
                    if (_hurtTimer.IsStopped())
                    {
                        var sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
                        (sprite.Material as ShaderMaterial)?.SetShaderParameter("hurt", true);
                    }

                    _hurtTimer.Start(1);
                }
            }
        }
    }

    private void _on_hurt_timer_timeout()
    {
        var sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        (sprite.Material as ShaderMaterial)?.SetShaderParameter("hurt", false);
        _hurtTimer.Stop();
    }

    private void _on_health_component_died()
    {
        //TODO: Death animation or something
        QueueFree();
        
        EmitSignal(SignalName.Dead, (int)WeakToType);
    }

    private void _on_shoot_bullet_timer_timeout()
    {
        SetShootBulletTimerWaitTimeToARandomRange();
        var scene = GD.Load<PackedScene>("res://scenes/bullet2.tscn");
        Marker2D bulletSpawnPoint;
        if(_animatedSprite2D.Scale.X < 0)
            bulletSpawnPoint = GetNode<Marker2D>("BulletSpawnMarker2DLeft");
        else
            bulletSpawnPoint = GetNode<Marker2D>("BulletSpawnMarker2DRight");
        var inst = scene.Instantiate<magic_bullet>();
        inst.FriendlyFire = false;
        inst.GlobalPosition = bulletSpawnPoint?.GlobalPosition ?? GlobalPosition;

        var level = GetParent<Node2D>();
        var witch = (CharacterBody2D)level.FindChild("Witch");
        Vector2 mousePos = witch.GlobalPosition;

        Vector2 direction = (mousePos - inst.GlobalPosition)
                .Normalized(); // Calculate the direction to the mouse from the bullet 
        

        Vector2 velocity = direction * magic_bullet.Speed;

        inst.Velocity = velocity;
        inst.Rotation = velocity.Angle() + Mathf.Pi;

        AddSibling(inst);
    }
}