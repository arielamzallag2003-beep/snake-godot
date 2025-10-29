using Godot;

public partial class Menu : Control
{
    private Button _startButton;
    private Button _exitButton;
    private AudioStreamPlayer _menuMusic;

    public override void _Ready()
    {
        
        _startButton = GetNode<Button>("Panel/Menu/StartGame");
        _exitButton = GetNode<Button>("Panel/Menu/Exit");

        _startButton.Pressed += OnStartGamePressed;
        _exitButton.Pressed += OnExitPressed;
    }

    private void OnStartGamePressed()
    {
        GetTree().ChangeSceneToFile("res://scene/main.tscn");
    }

    private void OnExitPressed()
    {
        GetTree().Quit();
    }
}