using Godot;
using System;


public partial class ScoreableComponent : Node3D
{
    [Export]
    public int ScoreValue;

    [Export]
    public bool IsMultiplier = false;

    [Export]
    public AudioStream ScoreSound { get; set; }

    [Signal]
    public delegate void ScoredEventHandler(AudioStream stream, int scoreValue);

    private Camera3D _camera;
    private PackedScene _scoreablePickupUI = GD.Load<PackedScene>("res://UI/ScoreablPickupUI.tscn");

    public override void _Ready()
    {
        _camera = GetViewport().GetCamera3D();
    }

    public void Score()
    {
        // Emit the signal to the parent
        EmitSignal(SignalName.Scored, ScoreSound, ScoreValue);
        var tempAudioPlayer = new AudioStreamPlayer3D
        {
            Stream = ScoreSound,
            GlobalTransform = GlobalTransform // Set its position to match the current node
        };

        // Add the player to the scene and play the sound
        GetTree().Root.AddChild(tempAudioPlayer);
        tempAudioPlayer.Play();

        // Queue the audio player for deletion after the sound finishes
        tempAudioPlayer.Finished += tempAudioPlayer.QueueFree;
        SpawnVisual();

        // Delete the object
        DeleteSelf();
    }
    public void DeleteSelf()
    {
        GetParent().QueueFree();
    }

    private void SpawnVisual()
    {
        // Get the camera
        if (_camera == null)
        {
            GD.PrintErr("Camera not found!");
            return;
        }

        // Convert the 3D position to 2D
        Vector3 worldPosition = GlobalTransform.Origin;
        Vector2 screenPosition = _camera.UnprojectPosition(worldPosition);

        // Spawn the "+1" UI element
        var uiElement = (RichTextLabel)_scoreablePickupUI.Instantiate();
        uiElement.Text = $"[i]+{ScoreValue}[/i]";
        uiElement.Position = screenPosition; // Position the UI element
        GetTree().Root.GetNode<Node3D>("Main").GetNode<CanvasLayer>("ScoreableIndicatorLayer").AddChild(uiElement);

        // Optional: Animate the "+1" (e.g., move up and fade out)
        var tween = uiElement.CreateTween();
        tween.TweenProperty(uiElement, "modulate", new Color(1, 1, 1, 0), 1.0)
             .SetTrans(Tween.TransitionType.Quad)
             .SetEase(Tween.EaseType.InOut);
        tween.TweenProperty(uiElement, "position", screenPosition + new Vector2(0, -50), 1.0);

        // Remove the UI element after the animation
        tween.TweenCallback(Callable.From(() => uiElement.QueueFree()));
    }
}