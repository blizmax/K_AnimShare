using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputEvent : MonoSingleton<InputEvent>
{
    PlayerInput input;

    public delegate void InputV2(Vector2 v2);
    //move
    public event InputV2 moveStarted;
    public event InputV2 movePerformed;
    public event InputV2 moveCanceled;
    
    public delegate void InputFloat(float f);
    //shift/ L1
    public event InputFloat shiftStarted;
    public event InputFloat shiftPerformed;
    public event InputFloat shiftCanceled;
    
    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Awake()
    {
        input = new PlayerInput();

        //move
        input.PlayerControl.Move.started += arg =>
        {
            moveStarted?.Invoke(arg.ReadValue<Vector2>());
        };
        input.PlayerControl.Move.performed += arg =>
        {
            movePerformed?.Invoke(arg.ReadValue<Vector2>());
            
        };
        input.PlayerControl.Move.canceled += arg =>
        {
            moveCanceled?.Invoke(arg.ReadValue<Vector2>());
        };
        
        //shift
        input.PlayerControl.Shift.started += arg =>
        {
            shiftStarted?.Invoke(arg.ReadValue<float>());
        };
        input.PlayerControl.Shift.performed += arg =>
        {
            shiftPerformed?.Invoke(arg.ReadValue<float>());
        };
        input.PlayerControl.Shift.canceled += arg =>
        {
            shiftCanceled?.Invoke(arg.ReadValue<float>());
        };
        
    }
}
