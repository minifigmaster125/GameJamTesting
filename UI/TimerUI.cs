using Godot;

public partial class TimerUI : Control
{
    private Timer _timer;

    private RichTextLabel _timerLabel;

    public override void _Ready()
    {
        _timer = GetNode<Timer>("Timer");
        _timerLabel = GetNode<RichTextLabel>("RichTextLabel");

        ResetTimer();
    }

    public override void _Process(double delta)
    {
        UpdateTimerLabel();
    }

    public void ResetTimer()
    {
        _timer.OneShot = false;
        _timer.Start();
        UpdateTimerLabel();
    }

    public void PauseTimer()
    {
        _timer.Stop();
    }

    public void ResumeTimer()
    {
        _timer.Start();
    }

    private void UpdateTimerLabel()
    {
        _timerLabel.Text = FormatTime((float)_timer.TimeLeft);
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return $"{minutes:00}:{seconds:00}";
    }

    private void OnTimerEnd()
    {
        GD.Print("Timer ended!");
        // Add logic for when the timer ends
    }
}
