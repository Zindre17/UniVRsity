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

    private Stack<Change> changes;

    private class Change {
        public int pixel;
        public Color start;
        public Color end;
        public Change(int p, Color s, Color e) {
            pixel = p;
            start = s;
            end = e;
        }
    }

    private void Do(Change c) {
        image[c.pixel].material.color = c.end;
    }

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
        changes = new Stack<Change>();
        colorManager = ColorManager.instance;
    }

    private void ClearRep() {
        foreach(Renderer r in image) {
            r.material.color = colorManager.unvisited;
        }
        changes.Clear();
    }

    public void Undo() {
        Change prev = changes.Pop();
        Undo(prev);
    }

    private void Undo(Change c) {
        image[c.pixel].material.color = c.start;
    }

    public void Visit(int index, bool pattern) {
        Change c = new Change(index, image[index].material.color, pattern ? colorManager.pattern : colorManager.visited);
        Do(c);
        changes.Push(c);
    }

    public void Next(int index) {
        Change c = new Change(index, image[index].material.color, colorManager.next);
        Do(c);
        changes.Push(c);
    }

    public void Added(int index) {
        Change c = new Change(index, image[index].material.color, colorManager.added);
        Do(c);
        changes.Push(c);
    }

    public void Seed(int index) {
        if (index < 0 || index > image.Count) return;
        ClearRep();
        Change c = new Change(index, image[index].material.color, colorManager.seed);
        Do(c);
        changes.Push(c);
    }

    public void Restart(int _resolution) {
        resolution = _resolution;
        UpdateMeasurements();
        ClearRep();
    }
}
