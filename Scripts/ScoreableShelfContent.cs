using Godot;
using System;

public partial class ScoreableShelfContent : Node3D
{
    [Export]
    public int ScoreValue;

    // This method will delete the object when it's called
    public void DeleteSelf()
    {
        QueueFree();
    }
}