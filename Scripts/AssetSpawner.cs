using Godot;
using System;
using System.Collections.Generic;

public partial class AssetSpawner: Node3D
{
    private Vector3 shelfHalfDimensions = new Vector3(5.6f, 6.3f, 1.9f);
    public override void _Ready()
    {
        //create floor 30x30
        SpawnByDimensions("floor_tile.tscn", new Vector3(0, -.1f, 0), new Vector3(50, .1f, 50), new Vector3(0f, 0f, 0f));

        // SpawnByDimensions("column.tscn", new Vector3(0, 1.5, 0), new Vector3(2,2,2), new Vector3(0f, 0f, 0f));

        // SpawnByDimensionsGroups("column.tscn", new Vector3(0, 3, 0), new Vector3(5,5,5), new Vector3(0f, 0f, 0f),
        //     new Vector3(1,5,1), new Vector3(2f, 0f, 2f));

        // SpawnByDimensions("shelf_half.tscn",  new Vector3(0, 2f, 0), new Vector3(15, .1f,15), new Vector3(0f, 0f, 7f), -90);

        // SpawnByDimensions("column.tscn", new Vector3(0, 1f, 0), new Vector3(5, .1f, .1f), new Vector3(0f, 0f, 0f));
        // SpawnByDimensions("column.tscn", new Vector3(0, 2.2f, 0), new Vector3(5, .1f, .1f), new Vector3(0f, 0f, 0f));
        // SpawnByDimensions("column.tscn", new Vector3(0, 3.4f, 0), new Vector3(5, .1f, .1f), new Vector3(0f, 0f, 0f));

        //test spawn fit for shelf at origin
        // string[] shelfContentAssetPaths = {"column.tscn", "column.tscn", "column.tscn", "column.tscn", "column.tscn"};
        // SpawnShelfContents(new Vector3(0, 0, 0), 0, shelfContentAssetPaths);

        SpawnShelves();
        
    }

    private void SpawnShelves()
    {
        float rotationAboutAzimuthDegrees = 0;
        string[] shelfContentAssetPaths = {"column.tscn", "column.tscn", "column.tscn", "column.tscn", "column.tscn"};
        Vector3 shelfHalfOffset = new Vector3(shelfHalfDimensions.X, 0f, 0f);
        int shelfRotationDeg = -90;//?
        shelfHalfOffset = DimensionsRotated(shelfRotationDeg, shelfHalfOffset);

        Vector3 dimensions = new Vector3(30, .1f,30);
        Vector3 offsets = new Vector3(.1f, 0f, 8f);

        List<Vector3> shelfPositions1 = SpawnByDimensions("shelf_half.tscn", Vector3.Zero, dimensions, offsets, shelfRotationDeg);
        // List<Vector3> shelfPositions2 = SpawnByDimensions("shelf_half.tscn",  Vector3.Zero+shelfHalfOffset, dimensions, offsets, -shelfRotationDeg);

        foreach(Vector3 shelfPosition1 in shelfPositions1)
        {
            SpawnShelfContents(shelfPosition1, shelfRotationDeg, shelfContentAssetPaths);
        }
        // foreach(Vector3 shelfPosition2 in shelfPositions2)
        // {
        //     SpawnShelfContents(shelfPosition2, shelfRotationDeg, shelfContentAssetPaths);
        // }
    }

//note shelf bottom edge needs to be 0f in scene
    private void SpawnShelfContents(Vector3 position, float rotationAboutAzimuthDegrees, string[] shelfContentAssetPaths)
    {
        
        float offsetPerRow = 1.2f;
        float offset = 1f;

        Vector3 baseShelfDimension = new Vector3(5, .1f, .1f);

        for(int row = 0; row < 5; row++)
        {
            PackedScene shelfContent = (PackedScene)ResourceLoader.Load<PackedScene>(shelfContentAssetPaths[row]);
            PhysicsBody3D _shelfContent = (PhysicsBody3D)shelfContent.Instantiate();
            
            Vector3 _shelfDimensions = DimensionsRotated(rotationAboutAzimuthDegrees, baseShelfDimension);//?check rotationAboutAzimuthDegrees
            SpawnByDimensions(shelfContentAssetPaths[row], position+new Vector3(0f, offset, 0f), _shelfDimensions, Vector3.Zero, rotationAboutAzimuthDegrees);
            offset+=offsetPerRow;
        }
    }

    private Vector3 DimensionsRotated(float rotationAboutAzimuthDegrees, Vector3 dimensions)//suitable approx for symmetrical??
    {
        if(rotationAboutAzimuthDegrees % 90 == 0)
        {
            return new Vector3(dimensions.Z, dimensions.Y, dimensions.X);
        }
        return dimensions;
    }


    private Vector3 FindColliderDimensions(PhysicsBody3D physicsBody3D)
    {

        var collisionShape = physicsBody3D.GetNode<CollisionShape3D>("CollisionShape3D");

        if (collisionShape.Shape is BoxShape3D boxShape)
        {
            // Access box dimensions (size)
            Vector3 boxDimensions = boxShape.Size;  // Extents is half the size, so multiply by 2
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

    private Transform3D RotateAboutAzimuth(float degrees, Transform3D transform)
    {
        Vector3 axis = new Vector3(0, 1, 0); // Or Vector3.Right
        float rotationAmount = Mathf.DegToRad(degrees/2);

        transform.Basis = new Basis(axis, rotationAmount) * transform.Basis;
        // shortened
        transform.Basis = transform.Basis.Rotated(axis, rotationAmount);
        return transform;
    }
    
    public List<Vector3> SpawnByDimensions(String assetPath, Vector3 position, Vector3 dimensions, Vector3 offset, float rotationAboutAzimuthDegrees=0, EditorInterface editorInterface=null)
    {
        PackedScene packedScene = (PackedScene)ResourceLoader.Load(assetPath);

        
        List<Vector3> spawnPositions = new List<Vector3>();
        // Instance the scene
        //TypeCheck Needed before this to check castability?
        PhysicsBody3D sceneInstance = (PhysicsBody3D)packedScene.Instantiate();
        Vector3 colliderDimensions = FindColliderDimensions(sceneInstance);

        colliderDimensions = DimensionsRotated(rotationAboutAzimuthDegrees, colliderDimensions);
        offset = DimensionsRotated(rotationAboutAzimuthDegrees, offset);//?

        // GD.Print("colliderDimensions: " + colliderDimensions);
        Vector3 startPosition = new Vector3(position.X - dimensions.X/2, position.Y - dimensions.Y/2, position.Z - dimensions.Z/2);
        Vector3 endPosition = new Vector3(startPosition.X + dimensions.X, startPosition.Y + dimensions.Y, startPosition.Z + dimensions.Z);
        // GD.Print("startPosition: " + startPosition);
        // GD.Print("endPosition: " + endPosition);
        for(float x = startPosition.X; x <= endPosition.X; x+=colliderDimensions.X+offset.X)
        {
            for(float y = startPosition.Y; y <= endPosition.Y; y+=colliderDimensions.Y+offset.Y)
            {
                for(float z = startPosition.Z; z <= endPosition.Z; z+=colliderDimensions.Z+offset.Z)
                {
                    sceneInstance = (PhysicsBody3D)packedScene.Instantiate();
                    Vector3 spawnPosition = new Vector3(x, y, z);
                    // GD.Print("Spawning at: " + spawnPosition);
                    sceneInstance.Position = spawnPosition;
                    Transform3D t = sceneInstance.Transform;
                    sceneInstance.Transform = RotateAboutAzimuth(rotationAboutAzimuthDegrees, sceneInstance.Transform);
                    if(editorInterface == null)
                    {
                        GetTree().CurrentScene.AddChild(sceneInstance);
                    }
                    else
                    {
                        // Now you have access to the editor interface within this method
                        
                        var sceneRoot= editorInterface.GetEditedSceneRoot();
                        sceneRoot.GetNode<Node3D>("Node3D").AddChild(sceneInstance);
                        // sceneRoot.AddChild(sceneInstance);
                    }

                    spawnPositions.Add(spawnPosition);
                }
            }
        }
        return spawnPositions;
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

    //needs full impl/debug
    private void SpawnByDimensionsGroups(String assetPath, 
        Vector3 position, Vector3 dimensions, Vector3 offsetHard, 
        Vector3 dimensionsInner, Vector3 OffestInner)
    {
        // GD.Print("colliderDimensions: " + colliderDimensions);
        Vector3 startPosition = new Vector3(position.X - dimensions.X/2, position.Y - dimensions.Y/2, position.Z - dimensions.Z/2);
        Vector3 endPosition = new Vector3(startPosition.X + dimensions.X, startPosition.Y + dimensions.Y, startPosition.Z + dimensions.Z);
        // GD.Print("startPosition: " + startPosition);
        // GD.Print("endPosition: " + endPosition);
        for(float x = startPosition.X; x <= endPosition.X; x+=offsetHard.X)
        {
            for(float y = startPosition.Y; y <= endPosition.Y; y+=offsetHard.Y)
            {
                for(float z = startPosition.Z; z <= endPosition.Z; z+=offsetHard.Z)
                {

                    SpawnByDimensions(assetPath, position + new Vector3(x, y, z), dimensionsInner, OffestInner);
                }
            }
        }
    }
}