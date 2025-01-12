using Godot;
using System;

public partial class MainUI : Control
{

    [Export]
    private RichTextLabel _scoreLabel;

    private Vector2 _originalScoreLabelPosition;
    [Export]
    public TimerUI _timerUI;

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here
        _originalScoreLabelPosition = _scoreLabel.Position;

    }
    public void SetScore(int score)
    {
        // Add logic to set the score
        _scoreLabel.Text = $"[center]{score}[/center]";
        // _scoreLabel.Position = _originalScoreLabelPosition + new Vector2(0, -1);
        var tween = _scoreLabel.CreateTween();
        tween.TweenProperty(_scoreLabel, "position", _originalScoreLabelPosition + new Vector2(0, -10), 0.1);

        tween.TweenCallback(Callable.From(_ResetScoreLabelPosition));
    }

    private void _ResetScoreLabelPosition()
    {
        _scoreLabel.Position = _originalScoreLabelPosition;
    }
}
