using Godot;
using System;
using System.Collections.Generic;

public partial class CameraOcclusionComponent : Node3D
{
    Camera3D camera;
    CharacterBody3D character;
    RayCast3D raycast;

    [Export]
    MeshInstance3D mesh;
    public int tick = 3;
    int tickOcclusionThreshold = 3;
    public override void _Ready()
    {
        base._Ready();
        // camera = GetViewport().GetCamera3D();
        // character = GetTree().Root.GetNode<Node3D>("Main").GetNode<CharacterBody3D>("CharacterBody3D");
        // assetName = GetParent().Name;
        // raycast = camera.GetNode<RayCast3D>("RayCast3D");

        // GetTree().GetNodesInGroup("Occludables");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        // if (raycast.IsColliding())
        // {
        //     // Get the collision point, normal, and collider
        //     var collider = raycast.GetCollider();
        //     var collisionPoint = raycast.GetCollisionPoint();
        // }
        // if (tick < tickOcclusionThreshold)
        // {
        //     tick++;
        // }

        // var material = mesh.GetSurfaceOverrideMaterial(0);
        // ShaderMaterial shaderMaterial = (ShaderMaterial)material;
        // shaderMaterial.SetShaderParameter("ray_is_colliding", tick < tickOcclusionThreshold);
        // shaderMaterial.SetShaderParameter("ray_is_colliding", true);
        // shaderMaterial.SetShaderParameter("camera_position", camera.GlobalPosition);
        // shaderMaterial.SetShaderParameter("character_position", character.GlobalPosition);


    }

    public void MakeTranparent()
    {
        var standardMaterial = mesh.GetActiveMaterial(0);
        var transparentMaterial = (StandardMaterial3D)standardMaterial.Duplicate();
        transparentMaterial.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
        transparentMaterial.AlbedoColor = new Color(transparentMaterial.AlbedoColor.R, transparentMaterial.AlbedoColor.G, transparentMaterial.AlbedoColor.B, 0.1f);
        transparentMaterial.BlendMode = BaseMaterial3D.BlendModeEnum.Add;
        mesh.SetSurfaceOverrideMaterial(0, transparentMaterial);
    }

    public void MakeOpaque()
    {
        mesh.SetSurfaceOverrideMaterial(0, null);
    }
}