using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class GameManager : Node3D
{

    [Export]
    private MainUI gameUI;

    private TimerUI _timerUI;

    [Export]
    public CharacterBody3D Character;

    private int _score = 0;
    [Export]
    private CanvasLayer _gameOverOverlay;

    public override void _Ready()
    {
        AssetSpawner assetSpawner = new AssetSpawner();
        AddChild(assetSpawner);
        Array<Node> scoreables = GetTree().GetNodesInGroup("Scoreable");
        foreach (ScoreableComponent node in scoreables)
        {
            node.Scored += HandleScored;
        };
        _timerUI = gameUI._timerUI;
        _timerUI.GetNode<Timer>("Timer").Timeout += OnTimerTimeout;
    }

    private void HandleScored(int scoreValue, bool isMultiplier, MeshInstance3D model)
    {
        if (isMultiplier)
        {
            _score *= scoreValue;
        }
        else
        {
            _score += scoreValue;
        }

        gameUI.SetScore(_score);

        if (model != null)
        {
            MeshInstance3D tempModel = (MeshInstance3D)model.Duplicate();

            tempModel.GlobalTransform = Character.GetNode<Node3D>("HatLocator").GlobalTransform;
            Character.AddChild(tempModel);
        }
    }

    private void OnTimerTimeout()
    {
        GD.Print("Time's up!");
        _gameOverOverlay.Visible = true;

        bool success = true;
        if (!success)
        {
            _score = 0;
        }

        ((GameOverUi)_gameOverOverlay.GetNode("GameOverUI")).SetEndState(_score, success);
        GetTree().Paused = true;

    }
}

