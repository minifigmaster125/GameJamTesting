using Godot;

public partial class CharacterController : CharacterBody3D
{
    [Export] public float Speed = 5.0f;
    [Export] public float Acceleration = 10.0f;

    private Vector3 _velocity = Vector3.Zero;

    public override void _PhysicsProcess(double delta)
    {
        Vector3 inputDirection = Vector3.Zero;

        // Get input (WASD or arrow keys)
        inputDirection.X = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
        inputDirection.Z = Input.GetActionStrength("move_back") - Input.GetActionStrength("move_forward");

        if (inputDirection != Vector3.Zero)
        {
            inputDirection = inputDirection.Normalized();
            // Rotate input direction for isometric perspective
            inputDirection = inputDirection.Rotated(Vector3.Up, -Mathf.Pi / 4);
        }

        // Smooth acceleration/deceleration
        _velocity = _velocity.Lerp(inputDirection * Speed, (float)(Acceleration * delta));

        // Apply movement
        Velocity = _velocity;

        MoveAndSlide();
    }
}
