using System.Collections.Generic;
using UnityEngine;

public class ImageRep : MonoBehaviour
{
    private int resolution;
    public int Resolution {
        get { return resolution; }
        set {
            if (resolution != value) {
                resolution = value;
                UpdateMeasurements();
            }
        }
    }

    private float spacing, size, elevation;
    public Transform bottomLeft;

    private void UpdateMeasurements() {
        float max = 1;
        float margin = max / 20;
        spacing = (max - 2 * margin) / (resolution * 10 - 2);
        size = 8 * spacing;
        elevation = size;
        int pixels = resolution * resolution;
        int existingPixels = image.Count;
        for (int i = 0; i < pixels; i++) {
            GameObject o;
            if (i < existingPixels) {
                o = image[i].gameObject;
            } else {
                o = Instantiate(pixelPrefab, bottomLeft);
                Renderer r = o.GetComponent<Renderer>();
                r.material.color = ColorManager.instance.unvisitedColor;
                image.Add(r);
            }
            float xp, yp, zp;
            int x = i % resolution;
            int y = Mathf.FloorToInt(i / resolution);    
            xp = margin + size / 2 + x * size + (x * spacing * 2 - spacing);
            yp = margin + size / 2 + y * size + (y * spacing * 2 - spacing);
            zp = elevation;
            o.transform.localPosition = new Vector3(xp, zp, yp);
            o.transform.localScale = new Vector3(size, size, size);
        }
    }

    private List<Renderer> image;
    public GameObject pixelPrefab;

    private void Start() {
        image = new List<Renderer>();
    }

    public void Visit(int index, bool pattern) {
        ColorManager m = ColorManager.instance;
        image[index].material.color = pattern?m.patternColor:m.visitedColor;
    }


}
