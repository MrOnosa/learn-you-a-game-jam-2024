using Godot;
using System;

public partial class life_bar : Node2D
{
    private const int TileIndex = 1;

    private TileMap _tileMap;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _tileMap = GetNode<TileMap>("TileMap");
        Paint(10);
    }

    public void Paint(int hp)
    {
        GD.Print("Setting life bar to "+hp);
        
        for (int i = 0; i < 5; i++)
        {
            //Clear
            _tileMap.SetCell(0, new Vector2I(i, 0), TileIndex);
        }
        var odd = hp % 2 == 1;
        for (int i = 0; i < hp / 2; i++)
        {
            _tileMap.SetCell(0, new Vector2I(i, 0), TileIndex, Vector2I.Zero);
        }

        if (odd)
        {
            _tileMap.SetCell(0, new Vector2I((hp / 2), 0), TileIndex, new Vector2I(1, 0));
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}