using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    private static InputController instance;

    public static InputController GetInstance()
    {
        if (!instance) instance = FindObjectOfType<InputController>();
        return instance;
    }
    
    private Inputs m_playerInput;
    
    public delegate void Interact();
    public static event Interact OnInteract;
    
    public delegate void Jump(bool _pressed);
    public static event Jump OnJump;
    
    public delegate void Down(bool _pressed);
    public static event Down OnDown;

#if UNITY_EDITOR
    public delegate void DrawDebug();
    public static event DrawDebug OnDrawDebug;
#endif

    private void Awake()
    {
        instance = this;
    }

    public float moveDirection { get; set; }

    public void ReadMoveDirection(InputAction.CallbackContext _context)
    {
        float moveInput = _context.ReadValue<float>();
        moveDirection = math.abs(moveInput) < 0.3f ? 0.0f : moveInput;
    }
    
    public void ReadInteractAction(InputAction.CallbackContext _context)
    {
        if (_context.performed)
            OnInteract?.Invoke();
    }
    
    public void ReadDownAction(InputAction.CallbackContext _context)
    {
        if (_context.performed || _context.canceled)
        {
            OnDown?.Invoke(_context.performed);
        }
    }
    
    public void ReadJumpAction(InputAction.CallbackContext _context)
    {
        if (_context.performed)
            OnJump?.Invoke(true);
        if (_context.canceled)
            OnJump?.Invoke(false);
    }

    public void ReadDrawDebugAction(InputAction.CallbackContext _context)
    {
#if UNITY_EDITOR
        if (_context.performed)
            OnDrawDebug?.Invoke();
#endif
    }
}
