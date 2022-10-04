using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClickReader : MonoBehaviour
{
    public delegate void Click(bool _click);
    public static event Click OnClick;
    public void OnClickEvent(InputAction.CallbackContext _context)
    {
        if (_context.started)
            OnClick?.Invoke(true);
        else if(_context.canceled)
            OnClick?.Invoke(false);
    }
}
