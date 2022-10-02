using System;
using System.Numerics;
using Mirror;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Target : NetworkBehaviour
{
    public static int NB_PLAYER = 0;
    
    private Camera m_mainCam;
    [SerializeField] private TrailRenderer m_trailRenderer;
    [SerializeField] private GameObject m_cursor;

    private bool m_pressed = false;
    private bool m_isDrawing;
    private const int m_refreshFrequency = 15;
    private float m_refreshPosTimer = 0f;

    private Vector3 m_targetPos;
    private Vector3 m_currentVelocity;
    private float m_targetSpeed;

    private void OnEnable()
    {
        NB_PLAYER++;
    }

    private void OnDisable()
    {
        NB_PLAYER--;
    }

    void Awake()
    {
        m_mainCam = Camera.main;
        m_targetPos = transform.position;
        m_currentVelocity = Vector3.zero;
    }

    void Start()
    {
        if (!isLocalPlayer)
        {
            m_trailRenderer.enabled = true;
        }
        else
        {
            SpawnCursor();
        }
    }

    private void SpawnCursor()
    {
        Instantiate(m_cursor, transform);
#if UNITY_EDITOR
#else
        Cursor.visible = false;
#endif
    }
    [Command] void SetIsDrawing(bool _drawable, Vector3 _position) {SetIsDrawingRPC(_drawable, _position); }
    [ClientRpc] void SetIsDrawingRPC(bool _drawable, Vector3 _position)
    {
        if(isLocalPlayer)
        {
            m_trailRenderer.enabled = _drawable;
        }
        else
        {
            transform.position = _position;
            m_targetPos = _position;
        }

        m_isDrawing = _drawable;
        if(_drawable) m_trailRenderer.Clear();
    }
    
    [Command] void SetPosition(Vector3 _position) { SetPositionRPC(_position); }
    [ClientRpc] void SetPositionRPC(Vector3 _position)
    {
        if (!isLocalPlayer)
        {
            transform.position = m_targetPos;
            m_targetPos = _position;
            m_targetSpeed = (m_targetPos - transform.position).magnitude * m_refreshFrequency;
        }
    }

   

    void Update()
    {
        if (isLocalPlayer)
        {
            Vector3 pos = Vector3.zero;
            bool pressed = false;
            
            Pointer pointer = Pointer.current;
            Pen pen = Pen.current;
            Mouse mouse = Mouse.current;
     
            if (pen.tip.isPressed || pen.inRange.isPressed)
            {
                pos = pen.position.ReadValue();
                pressed = pen.tip.isPressed;
            }
            else if (pointer.press.isPressed)
            {
                pos = pointer.position.ReadValue();
                pressed = true;
            }
            else if (mouse != null) {
                pos = mouse.position.ReadValue();
                pressed = mouse.IsPressed();
            }
            
            transform.position = (Vector2)m_mainCam.ScreenToWorldPoint(pos);

            if (pressed && !m_pressed)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up);

                // If it hits something...
                if (hit.collider != null)
                {
                    if (hit.collider.TryGetComponent<Button>(out Button button))
                    {
                        button.Click();
                    }
                }
                else
                {
                    SetIsDrawing(true, transform.position);
                }

                m_refreshPosTimer = 0f;
                m_pressed = true;

            }
            else if (!pressed && m_pressed)
            {
                SetIsDrawing(false, transform.position);
                m_refreshPosTimer = 0f;
                m_pressed = false;
            }

            m_refreshPosTimer += Time.deltaTime;
            if (m_refreshPosTimer > 1f / m_refreshFrequency)
            {
                m_refreshPosTimer = 0f;
                if (m_isDrawing) SetPosition(transform.position);
            }

            
        }
    }

    void FixedUpdate()
    {
        Vector3 position = transform.position;
        if (!isLocalPlayer && (m_targetPos - position).magnitude > 0.01f)
        {
            Vector3 desiredVelocity = (m_targetPos - position).normalized * m_targetSpeed;
            m_currentVelocity = Vector3.Lerp(m_currentVelocity, desiredVelocity, 0.6f);
            if ((m_targetPos - position).magnitude < m_currentVelocity.magnitude * Time.deltaTime)
            {
                transform.position = m_targetPos;
            }
            else
            {
                transform.position += m_currentVelocity * Time.deltaTime;
            }
        }
    }
}
