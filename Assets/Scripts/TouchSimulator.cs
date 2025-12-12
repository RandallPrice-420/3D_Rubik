using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class TouchSimulator : MonoBehaviour
{
    public event Action<bool>                               OnTapInput       = delegate { };
    public event Action<bool>                               OnTapSecondInput = delegate { };
    public event Action<Vector2>                            OnDeltaInput     = delegate { };
    public event Action<Vector2>                            OnPositionInput  = delegate { };
    public event Action<Vector2, Vector2, Vector2, Vector2> OnPinchInput     = delegate { };



    [Header("One Touch")]
    public Transform touch;

    [Header("Pinch")]
    public float     pinchInitialRadius = 100;
    public float     pinchInitialAngle  = 0;

    [Header("Pinch Components")]
    public Transform pinchPrimary;
    public Transform pinchSecondary;
    public bool      isPinchPrimaryTapping;
    public bool      isPinchSecondaryTapping;



    private Vector2 mouseLastPos;
    private Vector2 primaryLastPos;
    private Vector2 secondaryLastPos;
    private bool    isCursorWarped;
    private bool    isTapping;
    private bool    isTappingSecond;

    Vector2 GetMousePosOffset()  => Mouse.current.position.ReadValue() - new Vector2(Screen.width / 2, Screen.height / 2);



    private void Awake()
    {
#if !UNITY_EDITOR
        gameObject.SetActive(false);
#endif
            
        pinchPrimary   .localPosition    = new Vector2(0,  pinchInitialRadius);
        pinchSecondary .localPosition    = new Vector2(0, -pinchInitialRadius);
        touch.transform.localEulerAngles = new Vector3(0, 0, pinchInitialAngle);
    }


    private void Update()
    {
        HandleTapInput();
        HandleTapSecondInput();
        HandleDeltaInput();
        HandlePositionInput();
        HandlePinch();
    }


    private void HandleTapInput()
    {
        if (Mouse.current.leftButton.isPressed && !isTapping)
        {
            isTapping = true;
            OnTapInput?.Invoke(true);
        }
        else if (!Mouse.current.leftButton.isPressed && isTapping)
        {
            isTapping = false;
            OnTapInput?.Invoke(false);
        }
    }


    private void HandleTapSecondInput()
    {
        if (Mouse.current.rightButton.isPressed && !isTappingSecond)
        {
            isTappingSecond = true;
            OnTapSecondInput?.Invoke(true);
        }
        else if (!Mouse.current.rightButton.isPressed && isTappingSecond)
        {
            isTappingSecond = false;
            OnTapSecondInput?.Invoke(false);
        }
    }


    private void HandleDeltaInput()
    {
        OnDeltaInput?.Invoke(Mouse.current.delta.ReadValue());
    }


    private void HandlePositionInput()
    {
        OnPositionInput?.Invoke(Mouse.current.position.ReadValue());
    }


    private void HandlePinch()
    {
        isPinchPrimaryTapping   = Mouse.current.leftButton .isPressed;
        isPinchSecondaryTapping = Mouse.current.rightButton.isPressed;

        if (isPinchPrimaryTapping && isPinchSecondaryTapping)
        {
            if (!isCursorWarped)
            {
                WarpCursor();
            }
            else
            {
                pinchPrimary  .localPosition = GetMousePosOffset() - (Vector2)touch.localPosition;
                pinchSecondary.localPosition = -pinchPrimary.localPosition;

                OnPinchInput?.Invoke(
                    pinchPrimary  .position,
                    pinchSecondary.position,
                    primaryLastPos,
                    secondaryLastPos);
            }

            primaryLastPos   = pinchPrimary  .position;
            secondaryLastPos = pinchSecondary.position;
        }
        else
        {
            touch.localPosition = new Vector2(
                Mouse.current.position.ReadValue().x - Screen.width / 2,
                Mouse.current.position.ReadValue().y - Screen.height / 2
                );

            ResetUI();
        }

        mouseLastPos = Mouse.current.position.ReadValue();
    }


    private void ResetUI()
    {
        pinchPrimary  .localPosition = new Vector2(0,  pinchInitialRadius);
        pinchSecondary.localPosition = new Vector2(0, -pinchInitialRadius);
        touch.localEulerAngles       = Vector3.zero;
        isCursorWarped               = false;
    }


    private void WarpCursor()
    {
        isCursorWarped = true;
        var warpPos = touch.localPosition + pinchPrimary.localPosition + new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Mouse.current.WarpCursorPosition(warpPos);
    }


}   // class TouchSimulator
