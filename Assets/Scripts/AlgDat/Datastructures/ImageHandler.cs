using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageHandler : MonoBehaviour
{
    private int resolution;
    
    private float spacing;
    [Range(1,5)]
    public int width = 4;
    public GameObject pixelPrefab;

    private List<Pixel> pixels;
    private float scale;

    private Pixel lastSelection;
    public Pixel Seed { get; private set; }

    public Pixel GetSelectedPixel() {
        return lastSelection;
    }

    private void Awake() {
        pixels = new List<Pixel>();
    }

    private void OnEnable() {
        EventManager.OnPixelSelected += SelectPixel;
    }

    private void OnDisable() {
        EventManager.OnPixelSelected -= SelectPixel;
    }

    private void UpdateMeasurement() {
        int pieces = resolution * 4 + resolution - 1;
        spacing = (float)width/pieces;
        scale = spacing * 4;
        PlacePixels();
        SetPattern();
    }

    private void SelectPixel(Pixel p) {
        if (lastSelection != null)
            lastSelection.Selected = false;
        p.Selected = true;
        lastSelection = p;
    }

    public void DemoSelectPixel(int index, Action function = null) {
        Pixel p = pixels[index];
        StartCoroutine(DemoSelect(p, function: function));
    }

    private IEnumerator DemoSelect(Pixel p, Action function = null) {
        yield return new WaitForSeconds(.6f);
        SelectPixel(p);
        //yield return new WaitForSeconds(.3f);
        if (function != null) function();
    }

    public void Restart(int resolution) {
        this.resolution = resolution;
        if (lastSelection != null) lastSelection.Selected = false;
        lastSelection = null;
        if (Seed != null) Seed.Seed = false;
        UpdateMeasurement();
    }
    
    public void SetPattern() {
        bool seed = false;
        int i = 1;
        Pixel lastDark = null;
        foreach (Pixel p in pixels) {
            p.Dark = UnityEngine.Random.value < 0.5f;
            if (p.Dark && !seed) {
                if (UnityEngine.Random.value > 0.9f) {
                    p.Seed = true;
                    seed = true;
                    Seed = p;
                } else {
                    lastDark = p;
                }
            }
            i++;
        }
        if (!seed && lastDark!=null) {
            lastDark.Seed = true;
            Seed = lastDark;
        }
        EventManager.ChangeSeed(Seed.index);
    }

    public void Hint(int index, Action function = null) {
        pixels[index].Hint(function:function);
    }

    private void PlacePixels() {
        int n = resolution * resolution;
        float x, y;
        for(int i = 0; i < n; i++) {
            x = i % resolution;
            y = Mathf.FloorToInt(i / resolution);
            Vector3 relPos = new Vector3(x * scale+ x * spacing + scale/2, y * scale + spacing*y + scale/2, 0);
            if (pixels.Count == i) {
                GameObject o = Instantiate(pixelPrefab, transform);
                o.transform.localScale = new Vector3(scale, scale, scale);
                o.transform.localPosition = relPos;
                Pixel p = o.GetComponent<Pixel>();
                p.index = i;
                pixels.Add(p);
            } else {
                pixels[i].transform.localPosition = relPos;
                pixels[i].transform.localScale = new Vector3(scale, scale, scale);
            }
        }
    }
}
