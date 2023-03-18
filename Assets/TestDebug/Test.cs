using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Material material;

    void Start()
    {
        material = GetComponent<Renderer>().material;    
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            StartCoroutine(ColorLerp(Color.black, new Color(1.0f, 0.8f, 0f, 1f), 5.0f));
        }
    }

    IEnumerator ColorLerp(Color startColor, Color endColor, float duration)
    {
        float time = 0;
        while (time <= duration)
        {
            Color value = Color.Lerp(startColor, endColor, time / duration);
            material.SetColor("_EmissiveColorLDR", value);
            time += Time.deltaTime;
            yield return null;
        }
    }
}
