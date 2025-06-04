using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public InputActionMap inputActions;
    public InputAction upAction;
    public InputAction downAction;
    public InputAction leftAction;
    public InputAction rightAction;

    private void Awake()
    {
        GetInputActions();
    }

    public void GetInputActions()
    {
        var inputActions = InputSystem.actions;

        upAction = inputActions.FindAction("Up");
        downAction = inputActions.FindAction("Down");
        leftAction = inputActions.FindAction("Left");
        rightAction = inputActions.FindAction("Right");
    }
}
