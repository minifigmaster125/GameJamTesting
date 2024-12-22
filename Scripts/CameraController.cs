using Godot;

public partial class CameraController : Camera3D
{
    [Export] public float FollowSpeed = 5.0f;
    [Export] public Vector3 Offset = new Vector3(0, 10, -10);

    private Node3D _target;

    public override void _Ready()
    {
        // Ensure the parent node is the target character
        _target = GetParent().GetNode<Node3D>("CharacterBody3D");
    }

    public override void _Process(double delta)
    {
        // if (_target != null)
        // {
        //     Vector3 desiredPosition = _target.GlobalTransform.Origin + Offset;
        //     GlobalTransform = new Transform(
        //         GlobalTransform.Basis,
        //         GlobalTransform.Origin.Lerp(desiredPosition, (float)(FollowSpeed * delta))
        //     );

        //     LookAt(_target.GlobalTransform.Origin, Vector3.Up);
        // }
    }
}
