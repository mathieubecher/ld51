using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Server : MonoBehaviour
{
    NetworkManager manager;
    // Start is called before the first frame update
    void Awake()
    {
        manager = GetComponent<NetworkManager>();
    }

    void Start()
    {
        
        if (!NetworkClient.active)
        {
            // Server + Client
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                manager.StartServer();
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
