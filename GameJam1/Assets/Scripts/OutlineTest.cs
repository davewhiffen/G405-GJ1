using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineTest : MonoBehaviour
{
    public Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();

        rend.material.shader = Shader.Find("Custom/Outline");
    }

    // Update is called once per frame
    void Update()
    {
        float outline = Mathf.Lerp (1.1f, 1.5f, Mathf.PingPong(Time.time * 2, 1.0f));
        rend.material.SetFloat("_OutlineWidth", outline);
    }
}
