using UnityEngine;

public class ColorManager : MonoBehaviour {
    public static ColorManager instance;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this.gameObject);
        }
    }

    public Color
        light,
        dark,
        selected,
        visited,
        seed,
        pattern,
        unvisited,
        added,
        next,
        hint;

    public Color
        button,
        toggleButtonOn,
        toggleButtonOff;
}
