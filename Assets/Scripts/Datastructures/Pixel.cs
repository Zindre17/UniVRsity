using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pixel : MonoBehaviour
{
    private Renderer rend;

    public Color lightColor, darkColor;

    private void Awake() {
        rend = GetComponentInChildren<Renderer>();
    }

    public void SetColor(bool b) {
        rend.material.color = b ? lightColor : darkColor;
    }
}
