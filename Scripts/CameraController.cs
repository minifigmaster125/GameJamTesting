using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
public partial class CameraController : Camera3D
{
    [Export] public float FollowSpeed = 5.0f;
    [Export] public Vector3 Offset = new Vector3(0, 10, -10);

    private Node3D _target;

    Vector3 rayStart;
    Vector3 rayEnd;
    PhysicsDirectSpaceState3D spaceState;
    PhysicsRayQueryParameters3D rayQuery;

    private List<Node3D> _transparentObjects = new();

    public override void _Ready()
    {
        // Ensure the parent node is the target character
        _target = GetParent().GetParent().GetNode<Node3D>("CharacterBody3D");

    }

    public override void _Process(double delta)
    {

        rayStart = GlobalPosition;
        rayEnd = ((Node3D)_target.GetNode("RayTarget")).GlobalPosition;
        // GD.Print("rayStart: " + rayStart);
        // GD.Print("rayEnd: " + rayEnd);
        GetAllCollisionsAlongRay(rayStart, rayEnd);


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

    public class RaycastHitInfo
    {
        public Vector3 Position { get; set; }
        public Vector3 Normal { get; set; }
        public CollisionObject3D Collider { get; set; }
        public float Distance { get; set; }
    }

    private string Collider = "collider";

    public List<RaycastHitInfo> GetAllCollisionsAlongRay(
        Vector3 from,
        Vector3 to,
        uint collisionMask = 1,
        int maxResults = 32)
    {

        var currentObstructions = new List<Node3D>();
        var currentObstructionRids = new Godot.Collections.Array<Rid>();

        var hits = new List<RaycastHitInfo>();
        // var spaceState = PhysicsServer3D.SpaceGetDirectState(GetWorld3D().Space);
        var spaceState = GetWorld3D().DirectSpaceState;
        var queryParams = new PhysicsRayQueryParameters3D
        {
            From = from,
            To = to,
            // CollisionMask = collisionMask,
            CollideWithAreas = true,
            CollideWithBodies = true,
            Exclude = currentObstructionRids
        };

        // Continue raycasting until we hit the maximum number of results or reach the end
        var result = spaceState.IntersectRay(queryParams);

        while (result.Count > 0)
        {

            // No more collisions found
            var hitPosition = (Vector3)result["position"];
            var hitNormal = (Vector3)result["normal"];
            var hitCollider = result["collider"].As<CollisionObject3D>();
            var hitDistance = from.DistanceTo(hitPosition);

            // GD.Print(hitCollider.GetPath());
            if (hitCollider.Name == "CharacterBody3D")
            {
                break;
            }

            if (!hitCollider.HasNode("CameraOcclusionComponent"))
            {
                break;
            }

            CameraOcclusionComponent coc = hitCollider.GetNode<CameraOcclusionComponent>("CameraOcclusionComponent");

            // Adjust the ray start position slightly past the hit point for the next iteration
            currentObstructionRids.Add(hitCollider.GetRid());
            queryParams.Exclude = currentObstructionRids;
            currentObstructions.Add(hitCollider);

            if (!_transparentObjects.Contains(hitCollider))
            {
                // MAKE TRANSPARENT
                coc.MakeTranparent();
                _transparentObjects.Add(hitCollider);
            }

            result = spaceState.IntersectRay(queryParams);
        }

        foreach (var obj in _transparentObjects)
        {
            if (!currentObstructions.Contains(obj))
            {
                if (obj.HasNode("CameraOcclusionComponent"))
                {
                    obj.GetNode<CameraOcclusionComponent>("CameraOcclusionComponent").MakeOpaque();
                }
            }
        }

        _transparentObjects = currentObstructions;

        // hits.Add(new RaycastHitInfo
        // {
        //     Position = hitPosition,
        //     Normal = hitNormal,
        //     Collider = hitCollider,
        //     Distance = hitDistance
        // });


        // Sort hits by distance from origin
        // hits.Sort((a, b) => a.Distance.CompareTo(b.Distance));
        return hits;
    }
}
