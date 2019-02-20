using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageHandler : MonoBehaviour
{
    [Range(4,16)]
    public int resolution = 8;

    public GameObject pixelPrefab;

    private List<Pixel> pixels;
    private float scale;

    private void Awake() {
        scale = transform.localScale.x;
    }

    private void Start() {
        SpawnImage();
    }

    private void SpawnImage() {
        int n = resolution * resolution;
        float x, y;
        for(int i = 0; i < n; i++) {
            x = i % resolution;
            y = Mathf.FloorToInt(i / resolution);
            Vector3 relPos = new Vector3(x * scale + Mathf.Min(0, -resolution / 2 * scale), y * scale, 0);
            GameObject o = Instantiate(pixelPrefab, relPos, Quaternion.identity, transform);
            pixels.Add(o.GetComponent<Pixel>());
        }
    }
}
