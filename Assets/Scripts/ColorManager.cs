using UnityEngine;

[System.Serializable]
public class ColorManager : MonoBehaviour {
    public static ColorManager instance;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this.gameObject);
        }
    }

    [Header("Interactables")]
    public Color inactive;
    public Color selected;
    public Color hint;

    [Header("UIButtons")]
    public Color button;
    public Color inProgress;
    public Color toggleButtonOn;
    public Color toggleButtonOff;

    [Header("Datastructures")]
    public Color white;
    public Color black;
    public Color visited;
    public Color seed;
    public Color pattern;
    public Color unvisited;
    public Color added;
    public Color next;

    [Header("Sorting")]
    public Color element;
    public Color pivot;
    public Color correct;
    public Color box;
}
