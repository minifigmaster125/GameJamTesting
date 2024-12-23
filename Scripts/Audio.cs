using Godot;
using System.Collections.Generic;

public partial class Audio : Node
{
    private int _numPlayers = 12;
    private string _bus = "master";

    public static Audio Instance { get; private set; }

    // The available players
    private List<AudioStreamPlayer> _available = new();
    // The queue of sounds to play
    private Queue<string> _queue = new();

    public override void _Ready()
    {
        for (int i = 0; i < _numPlayers; i++)
        {
            var player = new AudioStreamPlayer();
            AddChild(player);

            _available.Add(player);

            player.VolumeDb = -10;
            player.Finished += () => OnStreamFinished(player);
            player.Bus = _bus;
        }
        Instance = this;
    }

    private void OnStreamFinished(AudioStreamPlayer stream)
    {
        _available.Add(stream);
    }

    public void Play(string soundPath)
    {
        _queue.Enqueue(soundPath);
    }

    public override void _Process(double delta)
    {
        if (_queue.Count > 0 && _available.Count > 0)
        {
            string soundPath = _queue.Dequeue();
            AudioStreamPlayer player = _available[0];
            _available.RemoveAt(0);

            player.Stream = GD.Load<AudioStream>(soundPath);
            player.Play();
            player.PitchScale = (float)GD.RandRange(0.9, 1.1);
        }
    }
}