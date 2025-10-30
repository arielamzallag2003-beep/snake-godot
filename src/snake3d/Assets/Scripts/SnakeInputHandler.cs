using UnityEngine;
using Elysium.Foundation.Serpentis.Core.Engine;
using Elysium.Foundation.Serpentis.Core.Domain;
using UnityEngine.InputSystem;
using CoreInputAction = Elysium.Foundation.Serpentis.Core.Domain.InputAction;
using CoreInputUnity = UnityEngine.InputSystem.InputAction;

public class SnakeInputHandler : MonoBehaviour
{
    private SnakeGame _engine;
    private SnakeControls _controls;

    public void Init(SnakeGame engine, SnakeView snakeView)
    {
        _engine = engine;
    }

    private void Awake()
    {
        _controls = new SnakeControls();

        _controls.Gameplay.TurnUp.performed += OnTurnUp;
        _controls.Gameplay.TurnDown.performed += OnTurnDown;
        _controls.Gameplay.TurnLeft.performed += OnTurnLeft;
        _controls.Gameplay.TurnRight.performed += OnTurnRight;
        _controls.Gameplay.Restart.performed += ctx =>
        {
            if (_engine == null) return;
            _engine.HandleInput(CoreInputAction.Restart);
        };
    }

    private void OnEnable()
    {
        _controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        _controls.Gameplay.Disable();
    }

    private void OnTurnUp(CoreInputUnity.CallbackContext context)
    {
        if (_engine == null) return;
        Debug.Log("[INPUT] Touche Z pressee -> Envoi TurnUp au moteur");
        _engine.HandleInput(CoreInputAction.TurnUp);
    }

    private void OnTurnDown(CoreInputUnity.CallbackContext context)
    {
        if (_engine == null) return;
        Debug.Log("[INPUT] Touche S pressee -> Envoi TurnDown au moteur");
        _engine.HandleInput(CoreInputAction.TurnDown);
    }

    private void OnTurnLeft(CoreInputUnity.CallbackContext context)
    {
        if (_engine == null) return;
        Debug.Log("[INPUT] Touche Q pressee -> Envoi TurnLeft au moteur");
        _engine.HandleInput(CoreInputAction.TurnLeft);
    }

    private void OnTurnRight(CoreInputUnity.CallbackContext context)
    {
        if (_engine == null) return;
        Debug.Log("[INPUT] Touche D pressee -> Envoi TurnRight au moteur");
        _engine.HandleInput(CoreInputAction.TurnRight);
    }
}