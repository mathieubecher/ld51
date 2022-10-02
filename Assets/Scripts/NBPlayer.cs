using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NBPlayer : MonoBehaviour
{
    private TextMeshPro m_text;
    // Start is called before the first frame update
    void Start()
    {
        m_text = GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        m_text.text = Target.NB_PLAYER.ToString();
    }
}
