using Godot;
using System;

public partial class health_component : Node2D
{
    [Export] public int MaxHealth = 10;

    [Signal]
    public delegate void HealthChangedEventHandler(HealthUpdate healthUpdate);
    [Signal]
    public delegate void DiedEventHandler();

    private int _currentHealth;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _currentHealth = MaxHealth;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void Damage(int damage)
    {
        ChangeHealth(-damage);
    }

    public void ChangeHealth(int delta)
    {
        var previousHealth = _currentHealth;
        _currentHealth = Math.Clamp((_currentHealth + delta), 0, MaxHealth);
        var healthUpdate = new HealthUpdate
        {
            PreviousHealth = previousHealth,
            CurrentHealth = _currentHealth,
            MaxHealth = MaxHealth,
            HealthPercent = CurrentHealthPercent(),
            IsHeal = delta > 0
        };
        EmitSignal(SignalName.HealthChanged, healthUpdate);
        if (_currentHealth <= 0)
        {
            EmitSignal(SignalName.Died);
        }
    }

    public float CurrentHealthPercent()
    {
        return (_currentHealth <= 0 ? 0 : _currentHealth / (float)MaxHealth);
    }
}

public partial class HealthUpdate : Node
{
    public int PreviousHealth;
    public int CurrentHealth;
    public int MaxHealth;
    public float HealthPercent;
    public bool IsHeal;
}