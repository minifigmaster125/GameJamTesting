using Godot;
using System;

public partial class GameStartUi : Control
{
    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here

        GetNode<Button>("%StartButton").Pressed += OnStartButtonPressed;
    }

    private void OnStartButtonPressed()
    {
        GetTree().Paused = false;
        GetParent<CanvasLayer>().Visible = false;
    }
}
