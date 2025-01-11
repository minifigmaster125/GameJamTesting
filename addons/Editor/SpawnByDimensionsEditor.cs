using Godot;
using System;
using System.Collections.Generic;
#if TOOLS
[Tool]
public partial class SpawnByDimensionsEditor : EditorPlugin
{
    private Window pluginWindow;
    private Button toolbarButton;

    private AssetSpawner assetSpawner;
    public override void _EnterTree()
    {
        // Create the main toolbar button
        toolbarButton = new Button
        {
            Text = "SpawnByDimensionsEditor"
        };
        toolbarButton.Pressed += ShowPluginWindow;
        
        // Add the button to the editor toolbar
        AddControlToContainer(CustomControlContainer.Toolbar, toolbarButton);
        
        // Initialize the plugin window
        InitializeWindow();
    }

    public override void _ExitTree()
    {
        // Clean up
        if (toolbarButton != null)
            toolbarButton.QueueFree();
        if (pluginWindow != null)
            pluginWindow.QueueFree();
    }

    private SpinBox GenerateSpinBox(int minSize=100)
    {
        return new SpinBox
        {
            MinValue = -100,
            MaxValue = 100,
            Step = 0.1f,
            CustomMinimumSize = new Vector2(minSize, 0)
        };
    }

    private (SpinBox, SpinBox, SpinBox) GenerateVectorInputs(String text, VBoxContainer vbox)
    {
        // Vector3 input section
        var vectorLabel = new Label { Text = text };
        vbox.AddChild(vectorLabel);

        var vectorContainer = new HBoxContainer();
        vbox.AddChild(vectorContainer);
        // X input
        var xLabel = new Label { Text = "X:" };
        vectorContainer.AddChild(xLabel);
        var xInput = GenerateSpinBox();
        vectorContainer.AddChild(xInput);

        // Y input
        var yLabel = new Label { Text = "Y:" };
        vectorContainer.AddChild(yLabel);
        var yInput = GenerateSpinBox();
        vectorContainer.AddChild(yInput);

        // Z input
        var zLabel = new Label { Text = "Z:" };
        vectorContainer.AddChild(zLabel);
        var zInput = GenerateSpinBox();
        vectorContainer.AddChild(zInput);

        return (xInput, yInput, zInput);
    }

    private void InitializeWindow()
    {
        pluginWindow = new Window
        {
            Title = "Custom Input Plugin",
            // InitialPosition = WindowInitialPosition.CenterScreen,
            Size = new Vector2I(500, 500),
            Exclusive = true,
            Unresizable = true,
            AlwaysOnTop = true,
            Visible = false
        };

        AddChild(pluginWindow);

        // Create margins around the content
        var marginContainer = new MarginContainer
        {
            ThemeTypeVariation = "MarginContainer",
            CustomMinimumSize = new Vector2(480, 580)
        };
        marginContainer.AddThemeConstantOverride("margin_left", 10);
        marginContainer.AddThemeConstantOverride("margin_right", 10);
        marginContainer.AddThemeConstantOverride("margin_top", 10);
        marginContainer.AddThemeConstantOverride("margin_bottom", 10);
        pluginWindow.AddChild(marginContainer);

        // Create a vertical container for all elements
        var vbox = new VBoxContainer();
        marginContainer.AddChild(vbox);

        // Text input section
        var assetTextLabel = new Label { Text = "Asset Pathname:" };
        vbox.AddChild(assetTextLabel);

        var assetTextInput = new LineEdit
        {
            CustomMinimumSize = new Vector2(200, 0),
            PlaceholderText = "Enter text here..."
        };
        vbox.AddChild(assetTextInput);
        
        (SpinBox posX, SpinBox posY, SpinBox posZ) = GenerateVectorInputs("position", vbox);
        (SpinBox dimX, SpinBox dimY, SpinBox dimZ) = GenerateVectorInputs("dimension", vbox);
        (SpinBox offX, SpinBox offY, SpinBox offZ) = GenerateVectorInputs("offsets", vbox);

        // Float input section
        var floatLabel = new Label { Text = "Float Input:" };
        vbox.AddChild(floatLabel);

        var floatInput = GenerateSpinBox(150);
        vbox.AddChild(floatInput);

        // Add some spacing
        vbox.AddChild(new HSeparator());

        // Button container for centering
        var buttonContainer = new HBoxContainer();
        buttonContainer.AddThemeConstantOverride("separation", 10);
        vbox.AddChild(buttonContainer);

        // Process button
        var processButton = new Button
        {
            Text = "Process Input",
            CustomMinimumSize = new Vector2(150, 40)
        };

        assetSpawner = new AssetSpawner();
        var editedRoot = GetEditorInterface().GetEditedSceneRoot();
        processButton.Pressed += () =>
        {
            var position = new Vector3(
                (float)posX.Value,
                (float)posY.Value,
                (float)posZ.Value
            );
            var dimensions = new Vector3(
                (float)dimX.Value,
                (float)dimY.Value,
                (float)dimZ.Value
            );
            var offsets = new Vector3(
                (float)offX.Value,
                (float)offY.Value,
                (float)offZ.Value
            );
            GD.Print("Process Button Pressed!");
            // SpawnObject();
            SpawnByDimensions(assetTextInput.Text, position, 
                dimensions, offsets, (float)floatInput.Value);
            // assetSpawner.SpawnByDimensions(assetTextInput.Text, position, 
            //     dimensions, offsets, (float)floatInput.Value, GetEditorInterface());
            // ProcessInput(vector, (float)floatInput.Value, assetTextInput.Text);
        };
        buttonContainer.AddChild(processButton);

        // Close button
        var closeButton = new Button
        {
            Text = "Close",
            CustomMinimumSize = new Vector2(150, 40)
        };
        closeButton.Pressed += () => pluginWindow.Hide();
        buttonContainer.AddChild(closeButton);

        // Result label
        var resultLabel = new Label
        {
            AutowrapMode = TextServer.AutowrapMode.WordSmart
        };
        vbox.AddChild(resultLabel);

        // Handle window close button
        pluginWindow.CloseRequested += () => pluginWindow.Hide();

        // void ProcessInput(Vector3 vector, float floatValue, string textValue)
        // {
        //     string result = $"Processed Input:\n" +
        //                    $"Vector3: {vector}\n" +
        //                    $"Float: {floatValue}\n" +
        //                    $"Text: {textValue}";
            
        //     resultLabel.Text = result;
        //     GD.Print(result);
        // }
        
    }

    // Function to handle spawning the object
    private void SpawnObject(Node3D sceneInstance)
    {
         // Get the currently edited scene's root
        var editedRoot = GetEditorInterface().GetEditedSceneRoot();
        if (editedRoot == null)
            return;
        // PackedScene packedScene = ResourceLoader.Load<PackedScene>("floor_tile.tscn");
        // Node3D sceneInstance = (Node3D)packedScene.Instantiate();
        editedRoot.AddChild(sceneInstance);
        sceneInstance.Owner = editedRoot;
    }



    private void ShowPluginWindow()
    {
        if (pluginWindow != null)
        {
            pluginWindow.Show();
            pluginWindow.GrabFocus();
        }
    }

    public List<Vector3> SpawnByDimensions(String assetPath, Vector3 position, Vector3 dimensions, Vector3 offset, float rotationAboutAzimuthDegrees=0)
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
                    sceneInstance.Name = assetPath+sceneInstance.GetInstanceId();
                    SpawnObject(sceneInstance);
                    spawnPositions.Add(spawnPosition);
                }
            }
        }
        return spawnPositions;
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
}
#endif