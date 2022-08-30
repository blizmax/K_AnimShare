using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//默认移动状态 shift或L1切换
public enum MoveDefault
{
    walk,
    run,
}
public class InputManager : MonoSingleton<InputManager>
{
    public bool isMoving = false;
    public bool shiftDown = false;

    private void Start()
    {
        
    }
}
