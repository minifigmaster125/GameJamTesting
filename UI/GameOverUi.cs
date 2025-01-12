using Godot;
using System;
using System.Diagnostics.Contracts;

public partial class GameOverUi : Control
{

    private RichTextLabel _scoreLabel;
    private RichTextLabel _finalScoreDisplayLabel;
    private Button _restartButton;
    private Button _quitButton;

    [Export]
    private string _mainScene;

    public override void _Ready()
    {
        _scoreLabel = GetNode<RichTextLabel>("PanelContainer/MarginContainer/VBoxContainer/ScoreDisplay/ActualScoreLabel");
        _finalScoreDisplayLabel = GetNode<RichTextLabel>("PanelContainer/MarginContainer/VBoxContainer/ScoreDisplay/FinalScoreLabel");
        _restartButton = GetNode<Button>("%RetryButton");
        _quitButton = GetNode<Button>("%QuitButton");

        _restartButton.Pressed += OnRestartButtonPressed;
        _quitButton.Pressed += OnQuitButtonPressed;

    }


    public void SetEndState(int score, bool success)
    {
        _scoreLabel.Text = $"[center]{score}[/center]";
        if (success)
        {
            _finalScoreDisplayLabel.Text = "[center]You got out![/center]";
        }
        else
        {
            _finalScoreDisplayLabel.Text = "[center]You were caught :([/center]";
        }
    }

    private void OnRestartButtonPressed()
    {
        GetTree().Paused = false;
        GetTree().ChangeSceneToFile($"res://Scenes/{_mainScene}.tscn");
    }

    private void OnQuitButtonPressed()
    {
        GetTree().Quit();
    }
}
