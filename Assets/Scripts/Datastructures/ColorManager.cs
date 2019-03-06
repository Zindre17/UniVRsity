using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public static ColorManager instance;

    private void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(this.gameObject);
        }
    }

    public Color
        lightColor,
        darkColor,
        selectedColor,
        visitedColor,
        seedColor,
        patternColor,
        unvisitedColor,
        nextColor;
}
