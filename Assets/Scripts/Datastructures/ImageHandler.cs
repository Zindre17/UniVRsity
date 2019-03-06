using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageHandler : MonoBehaviour
{
    [Range(4,16)]
    public int resolution = 8;
    [Range(0,1)]
    public float spacing = 0.1f;

    public GameObject pixelPrefab;

    private List<Pixel> pixels;
    private float scale;

    private Pixel lastSelection;

    public Pixel GetSelectedPixel() {
        return lastSelection;
    }

    private void Awake() {
        scale = transform.localScale.x;
        pixels = new List<Pixel>();
    }

    private void Start() {
        SpawnImage();
        SetPattern();
    }

    private void OnEnable() {
        EventHandler.OnPixelSelected += SelectPixel;
    }

    private void OnDisable() {
        EventHandler.OnPixelSelected -= SelectPixel;
    }
    private void SelectPixel(Pixel p) {
        if (lastSelection != null)
            lastSelection.Selected = false;
        p.Selected = true;
        lastSelection = p;
    }

    public void SetPattern() {
        bool seed = false;
        int i = 1;
        foreach (Pixel p in pixels) {
            p.Dark = Random.value < 0.5f;
            if (p.Dark && !seed) {
                if (i == pixels.Count)
                    p.Seed = true;
                else {
                    if (Random.value > 0.9f) {
                        p.Seed = true;
                        seed = true;
                    }
                }
            }
            i++;
        }
    }

    private void SpawnImage() {
        int n = resolution * resolution;
        float x, y;
        for(int i = 0; i < n; i++) {
            x = i % resolution;
            y = Mathf.FloorToInt(i / resolution);
            Vector3 relPos = new Vector3(x * scale+ x * spacing + Mathf.Min(0, -resolution / 2 * scale), y * scale + spacing*y, 0);

            GameObject o = Instantiate(pixelPrefab, relPos+transform.position, Quaternion.identity, transform);
            Pixel p = o.GetComponent<Pixel>();
            p.index = i;
            pixels.Add(p);
        }
    }
}
