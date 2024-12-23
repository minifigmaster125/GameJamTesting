using Godot;
using System;

public partial class PlayerController : CharacterBody3D
{
    [Signal]
    public delegate void CoinCollectedEventHandler(int coins);

    [ExportGroup("Components")]
    [Export]
    public Node3D View;

    [ExportGroup("Properties")]
    [Export]
    public float MovementSpeed = 250f;

    [Export]
    public float JumpStrength = 7f;

    private Vector3 _movementVelocity;
    private float _rotationDirection;
    private float _gravity = 0f;

    private bool _previouslyFloored = false;
    private bool _jumpSingle = true;
    private bool _jumpDouble = true;

    private int _coins = 0;

    private CpuParticles3D _particlesTrail;
    private AudioStreamPlayer _soundFootsteps;
    private Node3D _model;
    private AnimationPlayer _animation;

    public override void _Ready()
    {
        _particlesTrail = GetNode<CpuParticles3D>("ParticlesTrail");
        _soundFootsteps = GetNode<AudioStreamPlayer>("SoundFootsteps");
        // _model = GetNode<Node3D>("Character");
        // _animation = GetNode<AnimationPlayer>("Character/AnimationPlayer");
    }

    public override void _PhysicsProcess(double delta)
    {
        HandleControls((float)delta);
        HandleGravity((float)delta);
        HandleEffects((float)delta);

        // Movement
        var appliedVelocity = Velocity.Lerp(_movementVelocity, (float)delta * 50f);
        appliedVelocity.Y = -_gravity;

        Velocity = appliedVelocity;
        MoveAndSlide();

        // Rotation
        if (new Vector2(Velocity.Z, Velocity.X).Length() > 0)
        {
            _rotationDirection = new Vector2(Velocity.Z, Velocity.X).Angle();
        }

        Rotation = new Vector3(Rotation.X, Mathf.LerpAngle(Rotation.Y, _rotationDirection, (float)delta * 10f), Rotation.Z);

        // Falling/respawning
        if (Position.Y < -10)
        {
            GetTree().ReloadCurrentScene();
        }

        // Animation for scale (jumping and landing)
        // _model.Scale = _model.Scale.Lerp(new Vector3(1, 1, 1), (float)delta * 10f);

        // Animation when landing
        if (IsOnFloor() && _gravity > 2 && !_previouslyFloored)
        {
            // _model.Scale = new Vector3(1.25f, 0.75f, 1.25f);
            Audio.Instance.Play("res://sounds/land.ogg");
        }

        _previouslyFloored = IsOnFloor();
    }
    private void HandleEffects(float delta)
    {
        _particlesTrail.Emitting = false;
        _soundFootsteps.StreamPaused = true;

        if (IsOnFloor())
        {
            var horizontalVelocity = new Vector2(Velocity.X, Velocity.Z);
            var speedFactor = horizontalVelocity.Length() / (MovementSpeed * delta);

            if (speedFactor > 0.05f)
            {
                // if (_animation.CurrentAnimation != "walk")
                // {
                //     _animation.Play("walk", 0.1f);
                // }

                if (speedFactor > 0.3f)
                {
                    _soundFootsteps.StreamPaused = false;
                    _soundFootsteps.PitchScale = speedFactor;
                }

                if (speedFactor > 0.75f)
                {
                    _particlesTrail.Emitting = true;
                }
            }
            // else if (_animation.CurrentAnimation != "idle")
            // {
            //     _animation.Play("idle", 0.1f);
            // }
        }
        // else if (_animation.CurrentAnimation != "jump")
        // {
        //     _animation.Play("jump", 0.1f);
        // }
    }

    private void HandleControls(float delta)
    {
        // Movement
        var inputDirection = Vector3.Zero;
        inputDirection.X = Input.GetAxis("move_left", "move_right");
        inputDirection.Z = Input.GetAxis("move_forward", "move_back");

        inputDirection = inputDirection.Rotated(Vector3.Up, View.Rotation.Y);

        if (inputDirection != Vector3.Zero)
        {
            inputDirection = inputDirection.Normalized();
            // Rotate input direction for isometric perspective
            inputDirection = inputDirection.Rotated(Vector3.Up, -Mathf.Pi / 4);
        }

        if (inputDirection.Length() > 1)
        {
            inputDirection = inputDirection.Normalized();
        }

        _movementVelocity = inputDirection * MovementSpeed * delta;

        // Jumping
        if (Input.IsActionJustPressed("jump"))
        {
            if (_jumpSingle || _jumpDouble)
            {
                Jump();
            }
        }
    }

    private void HandleGravity(float delta)
    {
        _gravity += 25f * delta;

        if (_gravity > 0 && IsOnFloor())
        {
            _jumpSingle = true;
            _gravity = 0;
        }
    }

    private void Jump()
    {
        Audio.Instance.Play("res://sounds/jump.ogg");

        _gravity = -JumpStrength;

        // _model.Scale = new Vector3(0.5f, 1.5f, 0.5f);

        if (_jumpSingle)
        {
            _jumpSingle = false;
            _jumpDouble = true;
        }
        else
        {
            _jumpDouble = false;
        }
    }

    public void CollectCoin()
    {
        _coins += 1;
        EmitSignal(nameof(CoinCollected), _coins);
    }
}
