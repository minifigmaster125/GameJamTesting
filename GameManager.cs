using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class GameManager : Node3D
{

    [Export]
    private MainUI gameUI;

    private Timer _timer;

    [Export]
    public CharacterBody3D Character;

    private Node3D _hatLocator;

    private int _score = 0;
    [Export]
    private CanvasLayer _gameOverOverlay;

    [Export]
    private CanvasLayer _gameStartOverlay;

    [Export]
    public TextureRect _alert;

    public override void _Ready()
    {
        // AssetSpawner assetSpawner = new AssetSpawner();
        // AddChild(assetSpawner);
        Array<Node> scoreables = GetTree().GetNodesInGroup("Scoreable");
        foreach (ScoreableComponent node in scoreables)
        {
            node.Scored += HandleScored;
        };
        _timer = gameUI._timerUI.GetNode<Timer>("Timer");
        _timer.Timeout += OnTimerTimeout;
        _hatLocator = Character.GetNode<Node3D>("HatLocator");
        GetNode<Area3D>("Level/Exit/Area3D").AreaEntered += OnExitAreaEntered;

        GetTree().Paused = true;
    }

    private void HandleScored(int scoreValue, bool isMultiplier, Node3D model)
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
            var tempModel = (Node3D)model.Duplicate();

            tempModel.Position = _hatLocator.Position;
            Character.AddChild(tempModel);
            _hatLocator.Position = _hatLocator.Position + new Vector3(0, 0.3f, 0);
        }
    }

    public override void _Process(double delta)
    {

        if (_timer.TimeLeft < 7 && _timer.TimeLeft > 5)
        {
            _alert.Visible = true;
        }
        else
        {
            _alert.Visible = false;
        }
    }

    private void OnTimerTimeout()
    {
        GD.Print("Time's up!");
        _gameOverOverlay.Visible = true;


        ((GameOverUi)_gameOverOverlay.GetNode("GameOverUI")).SetEndState(0, false);
        GetTree().Paused = true;

    }

    private void OnExitAreaEntered(Area3D area)
    {
        GD.Print("Exit reached!");
        _gameOverOverlay.Visible = true;

        ((GameOverUi)_gameOverOverlay.GetNode("GameOverUI")).SetEndState(_score, true);
        GetTree().Paused = true;
    }
}

