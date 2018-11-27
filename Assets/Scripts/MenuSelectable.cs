using System.Collections;
using UnityEngine;

public class MenuSelectable : MonoBehaviour {

    private Renderer rend;
    private Color defCol;
    public Color pressedColor;
    public Color toggledColor;

    public enum MenuOption { Demo, Restart, New, Back}

    public MenuOption option;
    private bool toggled = false;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        defCol = rend.material.color;
    }

    public void Press()
    {
        if(option == MenuOption.Demo) {
            toggled = !toggled;
            if (toggled)
                EventManager.OnAlgorithmCompleted += Detoggle;
        }
        StartCoroutine(ButtonPressed());
    }

    private void Detoggle() {
        toggled = false;
        EventManager.OnAlgorithmCompleted -= Detoggle;
        UpdateColor();
    }

    private IEnumerator ButtonPressed()
    {
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
