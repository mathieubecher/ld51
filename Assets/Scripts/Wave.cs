using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Wave : MonoBehaviour
{
    private float m_timer;
    private Animator m_animator;
    [SerializeField] private SpriteRenderer m_waveLine;
    [SerializeField] private SpriteRenderer m_wave;
    [SerializeField] private SpriteRenderer m_wetSand;

    [SerializeField]  private Texture m_sandRenderer;

    [Serializable] struct WaveSpriteSet
    {
        public Sprite m_waveLine;
        public Sprite m_wave;
        public Sprite m_wetSand;
    }

    [SerializeField] private List<WaveSpriteSet> m_sprites;
    
    public struct WaveMessage : NetworkMessage
    {}

    public void SendWave()
    {
        WaveMessage msg = new WaveMessage();
        NetworkServer.SendToAll(msg);
    }

    public void SetupClient()
    {
        m_animator = GetComponent<Animator>();
        NetworkClient.RegisterHandler<WaveMessage>(OnWave);
    }

    public void OnWave(WaveMessage msg)
    {
        if (m_sprites.Count > 0)
        {
            int i = (int)math.floor(Random.value * m_sprites.Count);
            m_wave.sprite = m_sprites[i].m_wave;
            m_waveLine.sprite = m_sprites[i].m_waveLine;
            m_wetSand.sprite = m_sprites[i].m_wetSand;
        }

        transform.localScale = new Vector3(-transform.localScale.x ,transform.localScale.y ,transform.localScale.z);
        
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
