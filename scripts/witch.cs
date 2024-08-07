using Godot;
using System;

public partial class witch : CharacterBody2D
{
    private gm global;
    
	[Signal]
	public delegate void ItemChangedEventHandler(ItemType itemType);
	[Signal]
	public delegate void HealthChangedEventHandler(HealthUpdate healthUpdate);
	[Signal]
	public delegate void DiedEventHandler();
	
	public const float Speed = 100.0f;
	public bool shooting = false;
	public bool Invincible = false;

	[Export] public ItemType InventorySlot1 = ItemType.None;

	private AnimatedSprite2D _animatedSprite2D;
	private Timer _timer;
	private Timer _invincibilityTimer;
	private Camera2D _camera;
	private health_component _healthComponent;
	
	

	public override void _Ready()
	{
		global = GetNode<gm>("/root/GM");
		_animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_timer = GetNode<Timer>("ShootCooldownTimer");
		_invincibilityTimer = GetNode<Timer>("InvincibilityTimer");
		_camera = GetNode<Camera2D>("Camera2D");
		_healthComponent = GetNode<health_component>("HealthComponent");
		_toggle_invincible_shader(false);
	}

	public override void _Process(double delta)
	{
		var twin = Input.GetVector("TwinStickShootLeft", "TwinStickShootRight", "TwinStickShootUp", "TwinStickShootDown"); 
		if (Input.IsActionPressed("Shoot"))
			Shoot();
		else if (twin != Vector2.Zero)
			TwinStickShoot();
		
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Get the input direction and handle the movement/deceleration.
		var global = GetNode<gm>("/root/GM");
		Vector2 direction = Input.GetVector("Left", "Right", "Up", "Down");
		if (global.UsingController)
		{
			direction = Input.GetVector("C_Left", "C_Right", "C_Up", "C_Down");
		}
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
		
		// Determine if the witch is moving or idle and set the appropriate animation.
		if (Velocity != Vector2.Zero)
		{
			// Run slower if they are moving slower.
			_animatedSprite2D.SpeedScale = velocity.Length() / Speed;
			if (_animatedSprite2D.Animation != "run")
				_animatedSprite2D.Play("run");
		}
		else
		{
			// Always idle at full speed.
			_animatedSprite2D.SpeedScale = 1;
			if (_animatedSprite2D.Animation != "idle")
				_animatedSprite2D.Play("idle");
		}
		
		if (velocity.X < 0)
		{
			_animatedSprite2D.Scale = new Vector2(-1, 1);
		}
		else
		{
			_animatedSprite2D.Scale = new Vector2(1, 1);
		}
		MoveAndSlide();
	}

	public void Shoot()
	{
		if (shooting == false && InventorySlot1 != ItemType.None)
		{
			GD.Print("Boom!");
			shooting = true;
			_timer.Start();
			var inst = InitMagicBullet();

			//Calc magic bullet velocity for a mouse
			Vector2 mousePos = _camera.GetGlobalMousePosition(); 
			Vector2 direction = (mousePos - inst.GlobalPosition).Normalized(); // Calculate the direction to the mouse from the bullet 
			Vector2 velocity = direction * magic_bullet.Speed;
			inst.Velocity = velocity;
			
			AddSibling(inst);
		}
	}

	private magic_bullet InitMagicBullet()
	{
		var scene = GD.Load<PackedScene>("res://scenes/magic_bullet.tscn");
		var inst = scene.Instantiate<magic_bullet>();
		inst.FriendlyFire = true;
		inst.GlobalPosition = GlobalPosition;
		inst.Type = InventorySlot1;

		if (InventorySlot1 == ItemType.GreenStaff)
		{
			var sfx = GetNode<AudioStreamPlayer2D>("ShootGreenAudioStreamPlayer2D");
			sfx.VolumeDb = global.ConvertVolumeToDbVolume(global.SfxVolume);
			sfx.Play();
		} else if (InventorySlot1 == ItemType.PinkStaff)
		{
			var sfx = GetNode<AudioStreamPlayer2D>("ShootPinkAudioStreamPlayer2D");
			sfx.VolumeDb = global.ConvertVolumeToDbVolume(global.SfxVolume);
			sfx.Play();
			
		}

		return inst;
	}

	public void TwinStickShoot()
	{
		if (shooting == false && InventorySlot1 != ItemType.None)
		{
			GD.Print("Twin Stick Boom!");
			shooting = true;
			_timer.Start();
			var inst = InitMagicBullet();

			//Calc magic bullet velocity for twin stick
			Vector2 direction = Input.GetVector("TwinStickShootLeft", "TwinStickShootRight", "TwinStickShootUp", "TwinStickShootDown").Normalized();
			Vector2 velocity = direction * magic_bullet.Speed;
			inst.Velocity = velocity;
			
			AddSibling(inst);
		}
	}

	private void _on_shoot_cooldown_timer_timeout()
	{
		shooting = false;
	}

	private void _on_health_component_health_changed(HealthUpdate healthUpdate)
	{
		if (healthUpdate.CurrentHealth > 0)
		{
			var sfx = GetNode<AudioStreamPlayer2D>("HurtAudioStreamPlayer2D");
			sfx.VolumeDb = global.ConvertVolumeToDbVolume(global.SfxVolume);
			sfx.Play();
		}

		EmitSignal(SignalName.HealthChanged, healthUpdate);
	}

	private async void _on_health_component_died()
	{
		var sfx = GetNode<AudioStreamPlayer2D>("DiedAudioStreamPlayer2D");
		sfx.VolumeDb = global.ConvertVolumeToDbVolume(global.SfxVolume);
		sfx.Play();
		Visible = false;
		
		// Wait for death sound to play
		await ToSignal(GetTree().CreateTimer(sfx.Stream.GetLength()), "timeout");
		
		EmitSignal(SignalName.Died);
	}

	private void _on_hit_box_area_2d_area_entered(Area2D area)
	{
		if (!Invincible)
		{
			if (area is green_goblin goblin)
			{
				TakeDamageRoutine(2);
			}
			else if (area is magic_bullet bullet)
			{
				if (!bullet.FriendlyFire)
				{
					TakeDamageRoutine(1);
				}
			}
		}
		
		if(area is item item)
		{
			InventorySlot1 = item.Type;
			item.QueueFree();
			EmitSignal(SignalName.ItemChanged, (int)InventorySlot1);
			
			var sfx = GetNode<AudioStreamPlayer2D>("PickUpStaffAudioStreamPlayer2D");
			sfx.VolumeDb = global.ConvertVolumeToDbVolume(global.SfxVolume);
			sfx.Play();
		}
	}

	private void TakeDamageRoutine(int damage)
	{
		_invincibilityTimer.Start();
		_healthComponent.Damage(damage);
		Invincible = true;
		_toggle_invincible_shader(true);
		
	}


	private void _on_invincibility_timer_timeout()
	{
		Invincible = false;
		_toggle_invincible_shader(false);
	}

	private void _toggle_invincible_shader(bool active)
	{
		var sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		(sprite.Material as ShaderMaterial)?.SetShaderParameter("invincible", active);
	}
}
