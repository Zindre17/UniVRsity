using System.Collections.Generic;
using UnityEngine;

public class ImageRep : MonoBehaviour
{
    private List<Renderer> image;
    public GameObject pixelPrefab;

    private ColorManager colorManager;

    private int resolution;
    
    private float spacing, size, elevation;
    public Transform bottomLeft;

    private void UpdateMeasurements() {
        if (colorManager == null) colorManager = ColorManager.instance;
        if (image == null) image = new List<Renderer>();
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
                image[i].material.color = colorManager.unvisited;
            } else {
                o = Instantiate(pixelPrefab, bottomLeft);
                Renderer r = o.GetComponent<Renderer>();
                r.material.color = colorManager.unvisited;
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



    private void Awake() {
        image = new List<Renderer>();
        colorManager = ColorManager.instance;
    }

    private void ClearRep() {
        foreach(Renderer r in image) {
            r.material.color = colorManager.unvisited;
        }
    }

    public void Visit(int index, bool pattern) {
        
        image[index].material.color = pattern?colorManager.pattern:colorManager.visited;
    }

    public void Next(int index) {
        image[index].material.color = colorManager.next;
    }

    public void Added(int index) {
        image[index].material.color = colorManager.added;
    }

    public void Seed(int index) {
        if (index < 0 || index > image.Count) return;
        ClearRep();
        image[index].material.color = colorManager.seed;
    }

    public void Restart(int _resolution) {
        resolution = _resolution;
        UpdateMeasurements();
    }
}
