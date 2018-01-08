using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickMove : MonoBehaviour {
    private Transform mTransform;
    public JoystickController mJoystickCL;
    public delegate void MoveDelegate();
    public static MoveDelegate moveEnd;
    public static MoveDelegate moveStart;
    public static JoystickMove instance;

    private void Awake()
    {
        instance = this;
        mTransform = transform;
    }

    private void OnMoveEnd()
    {
        
    }
}
