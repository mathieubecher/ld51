using System.Numerics;
using Mirror;
using Unity.Mathematics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Target : NetworkBehaviour
{
    private Camera m_mainCam;
    [SerializeField] private TrailRenderer m_trailRenderer;

    private bool m_isDrawing;
    private const int m_refreshFrequency = 15;
    private float m_refreshPosTimer = 0f;

    private Vector3 m_targetPos;
    private Vector3 m_currentVelocity;
    private float m_targetSpeed;
       
    
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
    }

    void HandleMovement()
    {
        if (isLocalPlayer)
        {
            transform.position = (Vector2)m_mainCam.ScreenToWorldPoint(Input.mousePosition);
            
            if (Input.GetMouseButtonDown(0))
            {
                SetIsDrawing(true, transform.position);
                m_refreshPosTimer = 0f;
            }
            else if(Input.GetMouseButtonUp(0))
            {
                SetIsDrawing(false, transform.position);
                m_refreshPosTimer = 0f;
            }
            m_refreshPosTimer += Time.deltaTime;
            if (m_refreshPosTimer > 1f / m_refreshFrequency)
            {
                m_refreshPosTimer = 0f;
                if(m_isDrawing) SetPosition(transform.position);
            }
        }

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
        HandleMovement();
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
