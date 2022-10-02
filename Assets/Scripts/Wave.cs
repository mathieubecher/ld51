using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Wave : MonoBehaviour
{
    private float m_timer;
    private Animator m_animator;
    
    public struct WaveMessage : NetworkMessage
    {}

    public void SendWave()
    {
        WaveMessage msg = new WaveMessage();
        NetworkServer.SendToAll(msg);
    }

    public void SetupClient()
    {
        Debug.Log("Setup Wave");
        m_animator = GetComponent<Animator>();
        NetworkClient.RegisterHandler<WaveMessage>(OnScore);
    }

    public void OnScore(WaveMessage msg)
    {
        Debug.Log("OnWave");
        m_animator.SetTrigger("SendWave");
    }

    void Start()
    {
#if UNITY_SERVER
        m_timer = 0f;
#else
        SetupClient();
#endif
    }
    
    void Update()
    {
#if UNITY_SERVER
        m_timer += Time.deltaTime;
        if (m_timer > 10.0f)
        {
            m_timer -= 10.0f;
            SendWave();

        }
#endif
    }
}
