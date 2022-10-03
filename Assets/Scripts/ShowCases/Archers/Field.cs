using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Field : MonoBehaviour
{
    [SerializeField] public int length = 10;
    [SerializeField] public int width = 1;
    [SerializeField] public Color color = Color.green;
    public float LeftBorder => RightBorder * -1;
    public float RightBorder => length / 2f;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(length, width, 1);
        var renderer = GetComponent<Renderer>();
        renderer.material.SetColor("_Color", color);
    }
}
