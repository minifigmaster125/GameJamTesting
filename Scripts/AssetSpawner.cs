using Godot;
using System;
using System.Collections.Generic;

public partial class AssetSpawner: Node3D
{
    public override void _Ready()
    {
        
        SpawnByDimensions("ShelfHalf.tscn", new Vector3(0, .5f, 0), new Vector3(30, 1, 30), new Vector3(3, 3, 3));
        SpawnByDimensions("column.tscn", new Vector3(0, 3, 0), new Vector3(5,5,5), new Vector3(0f, 0f, 0f));

        // SpawnByDimensionsGroups("column.tscn", new Vector3(0, 3, 0), new Vector3(5,5,5), new Vector3(0f, 0f, 0f),
        //     new Vector3(1,5,1), new Vector3(2f, 0f, 2f));
    }

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

    public static Vector3[] GetMeshVertices(Mesh mesh)
    {
        // Handle PrimitiveMesh types
        if (mesh is PrimitiveMesh primitiveMesh)
        {
            var arrayMesh = new ArrayMesh();
            arrayMesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, primitiveMesh.GetMeshArrays());
            mesh = arrayMesh;
        }

        // Get surface count
        int surfaceCount = mesh.GetSurfaceCount();
        var allVertices = new List<Vector3>();

        // Collect vertices from all surfaces
        for (int surface = 0; surface < surfaceCount; surface++)
        {
            var arrays = mesh.SurfaceGetArrays(surface);
            var surfaceVertices = (Vector3[])arrays[(int)Mesh.ArrayType.Vertex];
            allVertices.AddRange(surfaceVertices);
        }

        return allVertices.ToArray();
    }

    // Get indices similarly
    public static int[] GetMeshIndices(Mesh mesh, int surface = 0)
    {
        var arrays = mesh.SurfaceGetArrays(surface);
        return (int[])arrays[(int)Mesh.ArrayType.Index];
    }

   // Function to find the min and max vertices along each axis (x, y, z)
    public (Vector3 min, Vector3 max) GetMinMaxVertices(Mesh mesh)
    {
        // Initialize min and max values with extreme values
        Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        Visible = false;
        //https://docs.godotengine.org/en/stable/classes/class_meshdatatool.html
        // Add this node to the terrain_modifier group for easy finding
        AddToGroup("terrain_modifier");
        
        Vector3[] MeshPoints = GetMeshVertices(mesh);


        // Iterate through each vertex and update min/max values
        foreach (Vector3 vertex in MeshPoints)
        {
            // Update min and max values for each axis
            min.X = Math.Min(min.X, vertex.X);
            min.Y = Math.Min(min.Y, vertex.Y);
            min.Z = Math.Min(min.Z, vertex.Z);

            max.X = Math.Max(max.X, vertex.X);
            max.Y = Math.Max(max.Y, vertex.Y);
            max.Z = Math.Max(max.Z, vertex.Z);
        }

        // Return the min and max values
        return (min, max);
    }
    public void SpawnByDimensions(String assetPath, Vector3 position, Vector3 dimensions, Vector3 offset)
    {
        PackedScene packedScene = (PackedScene)ResourceLoader.Load(assetPath);
        // if(packedScene == null)
        // {
        //     throw new NullReferenceException();
        //     return;
        // }

        // Instance the scene
        //TypeCheck Needed before this to check castability?
        RigidBody3D sceneInstance = (RigidBody3D)packedScene.Instantiate();
        Vector3 colliderDimensions = FindColliderDimensions(sceneInstance);
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