using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternRep : MonoBehaviour
{
    public Transform bottomLeft;

    private int resolution;

    private float spacing, size, elevation;
    private List<Renderer> image;
    public GameObject pixelPrefab;

    private ColorManager cm;

    private void UpdateMeasurements() {
        float max = 1;
        float margin = max / 20;
        spacing = (max - 2 * margin) / (resolution * 10 - 2);
        size = 8 * spacing;
        elevation = -size;
        int pixels = resolution * resolution;
        int existingPixels = image.Count;
        for (int i = 0; i < pixels; i++) {
            GameObject o;
            if (i < existingPixels) {
                o = image[i].gameObject;
                image[i].material.color = cm.lightColor;
            } else {
                o = Instantiate(pixelPrefab, bottomLeft);
                Renderer r = o.GetComponent<Renderer>();
                r.material.color = cm.lightColor;
                image.Add(r);
            }
            float xp, yp, zp;
            int x = i % resolution;
            int y = Mathf.FloorToInt(i / resolution);
            xp = margin + size / 2 + x * size + (x * spacing * 2 - spacing);
            yp = margin + size / 2 + y * size + (y * spacing * 2 - spacing);
            zp = elevation;
            o.transform.localPosition = new Vector3(xp, yp, zp);
            o.transform.localScale = new Vector3(size, size, size);
        }
    }

    public void Pattern(int index) {
        if(index > 0 && index < image.Count)
            image[index].material.color = cm.darkColor;
    }

    private void Awake() {
        image = new List<Renderer>();
        cm = ColorManager.instance;
    }

    public void Restart(int _resolution) {
        resolution = _resolution;
        UpdateMeasurements();
    }

    private void ClearPattern() {
        foreach (Renderer r in image) {
            r.material.color = cm.lightColor;
        }
    }

    public void Seed(int index) {
        ClearPattern();
        Pattern(index);
    }

    private void OnEnable() {
        
    }
    private void OnDisable() {
        
    }
}
