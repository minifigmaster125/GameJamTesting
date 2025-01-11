#if TOOLS
using Godot;
using System;

[Tool]
public partial class AssetSpawnerEditorPlugin : EditorPlugin
{
    public override void _EnterTree()
    {
        // Initialization of the plugin goes here.
        // Add the new type with a name, a parent type, a script and an icon.
        var script = GD.Load<Script>("res://addons/MyCustomNode/MyButton.cs");
        var texture = GD.Load<Texture2D>("res://addons/MyCustomNode/Icon.png");
        AddCustomType("MyButton", "Button", script, texture);
    }

    public override void _ExitTree()
    {
        // Clean-up of the plugin goes here.
        // Always remember to remove it from the engine when deactivated.
        RemoveCustomType("MyButton");
    }

    // private LineEdit textField;
    // private Button actionButton;
    // private Label resultLabel;
    // private int value;

    // private AssetSpawner assetSpawner;

    // public override void _EnterTree()
    // {
    //     // assetSpawner = new AssetSpawner();
    //     // Initialize components
    //     textField = new LineEdit();
    //     actionButton = new Button();
    //     resultLabel = new Label();

    //     // Set up the Text Field (LineEdit)
    //     textField.PlaceholderText = "Enter a number";
    //     // textField.RectMinSize = new Vector2(200, 30);  // Make the text field larger
    //     textField.Position = new Vector2(10, 10);

    //     // Set up the Button
    //     actionButton.Text = "Update Value";
    //     // actionButton.RectMinSize = new Vector2(200, 30);
    //     actionButton.Position = new Vector2(10, 50);

    //     // Set up the result label to show the current value
    //     resultLabel.Text = "Current Value: 0";
    //     // resultLabel.RectMinSize = new Vector2(200, 30);
    //     resultLabel.Position = new Vector2(10, 90);

    //     // Connect the button's signal to an event handler
    //     actionButton.Pressed += OnButtonPressed;

    //     // Add the controls to the plugin dock
    //     AddControlToDock(DockSlot.LeftUl, textField);
    //     AddControlToDock(DockSlot.LeftUl, actionButton);
    //     AddControlToDock(DockSlot.LeftUl, resultLabel);

    //     // Initialize value
    //     value = 0;
    // }

    // private void OnButtonPressed()
    // {
    //     // Try to parse the text field input to an integer
    //     if (int.TryParse(textField.Text, out int newValue))
    //     {
    //         // If parsing is successful, update the value
    //         value = newValue;
    //         // assetSpawner.SpawnByDimensions(value);
    //     }
    //     else
    //     {
    //         // If the input is invalid, show an error message
    //         GD.Print("Invalid input, please enter a valid number.");
    //     }

    //     // Update the result label to reflect the current value
    //     resultLabel.Text = $"Current Value: {value}";
    // }

    // public override void _ExitTree()
    // {
    //     // Cleanup resources when the plugin is disabled or removed
    //     textField.QueueFree();
    //     actionButton.QueueFree();
    //     resultLabel.QueueFree();
    // }
}
#endif