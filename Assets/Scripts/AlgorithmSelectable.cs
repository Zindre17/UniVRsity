using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlgorithmSelectable : MonoBehaviour {

    private Renderer rend;
    private Color defCol;
    public Color pressedColor;
    public Color toggledColor;
    public SortManager.SortingAlgorithm option;
    private bool toggled = false;

    private void Awake() {
        rend = GetComponent<Renderer>();
        defCol = rend.material.color;
    }

    public void Press() {
        if (!toggled) {
            toggled = true;
            EventManager.AlgorithmChanged(option);
            EventManager.OnAlgorithmChanged += Detoggle;
            StartCoroutine(ButtonPressed());
        }
    }

    private void Detoggle(SortManager.SortingAlgorithm alg) {
        toggled = false;
        UpdateColor();
        EventManager.OnAlgorithmChanged -= Detoggle;
    }

    private IEnumerator ButtonPressed() {
        rend.material.color = pressedColor;
        yield return new WaitForSeconds(.2f);
        UpdateColor();
    }

    private void UpdateColor() {
        if (toggled)
            rend.material.color = toggledColor;
        else
            rend.material.color = defCol;
    }
}
