using Godot;
using System;

public partial class AssetSpawner: Node3D
{
    public override void _Ready()
    {
        
        SpawnByDimensions("column.tscn", new Vector3(0, 3, 0), new Vector3(5,5,5), new Vector3(0f, 3f, 0f));
    }

    private Vector3 FindColliderDimensions(RigidBody3D rigidBody3D)
    {

        var collisionShape = rigidBody3D.GetNode<CollisionShape3D>("CollisionShape3D");

        if (collisionShape.Shape is BoxShape3D boxShape)
        {
            // Access box dimensions (size)
            Vector3 boxDimensions = boxShape.Size * 2;  // Extents is half the size, so multiply by 2
            GD.Print("Box Dimensions: " + boxDimensions);
            return boxDimensions;
        }
        else if (collisionShape.Shape is SphereShape3D sphereShape)
        {
            // Access sphere radius
            float radius = sphereShape.Radius;
            GD.Print("Sphere Radius: " + radius);
            float diameter = radius*2;
            return Vector3.One * diameter;
        }
        else if (collisionShape.Shape is CylinderShape3D cylinderShape)
        {
            // Access sphere radius
            float radius = cylinderShape.Radius;
            GD.Print("Sphere Radius: " + radius);
            float height = cylinderShape.Height;
            GD.Print("Cylinder Height: " + height);
            float diameter = radius*2;
            return new Vector3(diameter, height, diameter);
        }
        return Vector3.Zero;
    }

    public void SpawnByDimensions(String assetPath, Vector3 position, Vector3 dimensions, Vector3 Offset)
    {
        PackedScene packedScene = (PackedScene)ResourceLoader.Load(assetPath);
        // if(packedScene == null)
        // {
        //     throw new NullReferenceException();
        //     return;
        // }

        // Instance the scene
        RigidBody3D sceneInstance = (RigidBody3D)packedScene.Instantiate();
        Vector3 colliderDimensions = FindColliderDimensions(sceneInstance);
        // GD.Print("colliderDimensions: " + colliderDimensions);
        Vector3 startPosition = new Vector3(position.X - dimensions.X/2, position.Y - dimensions.Y/2, position.Z - dimensions.Z/2);
        Vector3 endPosition = new Vector3(startPosition.X + dimensions.X, startPosition.Y + dimensions.Y, startPosition.Z + dimensions.Z);
        // GD.Print("startPosition: " + startPosition);
        // GD.Print("endPosition: " + endPosition);
        for(float x = startPosition.X; x <= endPosition.X; x+=colliderDimensions.X+Offset.X)
        {
            for(float y = startPosition.Y; y <= endPosition.Y; y+=colliderDimensions.Y+Offset.Y)
            {
                for(float z = startPosition.Z; z <= endPosition.Z; z+=colliderDimensions.Z+Offset.Z)
                {
                    sceneInstance = (RigidBody3D)packedScene.Instantiate();
                    Vector3 spawnPosition = new Vector3(x, y, z);
                    // GD.Print("Spawning at: " + spawnPosition);
                    sceneInstance.Position = spawnPosition;
                    // Add the scene instance to the current node tree
                    AddChild(sceneInstance);
                }
            }
        }

    }

    // public void SpawnByNumbers(String assetPath, Vector3 position, Vector3 numbers)
    // {
    //     GD.Load<PackedScene>(assetPath);
    // }

    public void SpawnIntoBoxMesh(String assetPath, MeshInstance3D meshInstance3D, Vector3 offset)
    {
        if(meshInstance3D.Mesh is BoxMesh boxMesh)
        {
            Vector3 boxDimensions = boxMesh.Size * 2;
            SpawnByDimensions(assetPath, meshInstance3D.Position, boxDimensions, offset);
        }
        // else
        // {
        //     throw InvalidOperationException("Shape for mesh spawning must be BoxShape3D");
        // }
    }

    public void FindAndSpawnInAllMeshes()
    {

    }
}