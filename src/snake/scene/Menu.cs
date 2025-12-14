using Godot;

public partial class Menu : Control
{
    private Button _startButton;
    private Button _exitButton;
    private AudioStreamPlayer _menuMusic;
    private Button _loadButton;
    private FileDialog _fileDialog;
    private CheckButton _aiToggle;
    public override void _Ready()
    {
        
        _startButton = GetNode<Button>("Panel/Menu/StartGame");
        _exitButton = GetNode<Button>("Panel/Menu/Exit");

        _startButton.Pressed += OnStartGamePressed;
        _exitButton.Pressed += OnExitPressed;

        _loadButton = GetNode<Button>("Panel/Menu/Charger");
        _fileDialog = GetNode<FileDialog>("Panel/Menu/Charger/FileDialog");
        _aiToggle = GetNode<CheckButton>("Panel/Menu/Ai");

      
        _loadButton.Pressed += OnLoadButtonPressed;
        _fileDialog.FileSelected += OnFileSelected;
        if (_aiToggle != null)
        {
            _aiToggle.ButtonPressed = GameData.UseAI;
            _aiToggle.Toggled += OnAiToggled;
        }
    }

    private void OnStartGamePressed()
    {
        GetTree().ChangeSceneToFile("res://scene/main.tscn");
    }

    private void OnExitPressed()
    {
        GetTree().Quit();
    }

    private void OnLoadButtonPressed()
    {
        _fileDialog.PopupCentered();
    }

    private void OnFileSelected(string path)
    {
        GD.Print($"Fichier sélectionné : {path}");

        GameData.DatasetPath = path;
    }
    private void OnAiToggled(bool isPressed)
    {
        GD.Print($"IA activée : {isPressed}");
        GameData.UseAI = isPressed;
    }
}