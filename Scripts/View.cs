using Godot;
using System;

public partial class View : Node3D
{
    [ExportGroup("Properties")]
    [Export]
    public Node3D Target { get; set; }

    [ExportGroup("Zoom")]
    [Export]
    public float ZoomMinimum { get; set; } = 16f;

    [Export]
    public float ZoomMaximum { get; set; } = 4f;

    [Export]
    public float ZoomSpeed { get; set; } = 10f;

    [ExportGroup("Rotation")]
    [Export]
    public float RotationSpeed { get; set; } = 120f;

    private Vector3 _cameraRotation;
    private float _zoom = 10f;
    private Camera3D _camera;

    public override void _Ready()
    {
        _cameraRotation = RotationDegrees;
        _camera = GetNode<Camera3D>("Camera");
    }

    public override void _PhysicsProcess(double delta)
    {
        float deltaFloat = (float)delta;

        // Set position and rotation to targets
        Position = Position.Lerp(Target.Position, deltaFloat * 4f);
        RotationDegrees = RotationDegrees.Lerp(_cameraRotation, deltaFloat * 6f);
        
        // _camera.Position = _camera.Position.Lerp(new Vector3(0, _zoom, 0), 8f * deltaFloat);
        
        // HandleInput(deltaFloat);
    }

    private void HandleInput(float delta)
    {
        // Rotation
        Vector3 input = Vector3.Zero;
        
        input.Y = Input.GetAxis("camera_left", "camera_right");
        input.X = Input.GetAxis("camera_up", "camera_down");
        
        Vector3 limitedInput = input.LimitLength(1.0f);
        _cameraRotation += limitedInput * RotationSpeed * delta;
        _cameraRotation = _cameraRotation with { X = Mathf.Clamp(_cameraRotation.X, -80f, -10f) };

        // Zooming
        _zoom += Input.GetAxis("zoom_in", "zoom_out") * ZoomSpeed * delta;
        _zoom = Mathf.Clamp(_zoom, ZoomMaximum, ZoomMinimum);
    }
}