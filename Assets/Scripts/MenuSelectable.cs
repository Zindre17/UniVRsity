using System.Collections;
using UnityEngine;

public class MenuSelectable : MonoBehaviour {

    private Renderer rend;
    private Color defCol;
    public Color pressedColor;

    public enum MenuOption { Demo, Restart, New, Back}

    public MenuOption option;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        defCol = rend.material.color;
    }

    public void Press()
    {
        StartCoroutine(ButtonPressed());
    }

    private IEnumerator ButtonPressed()
    {
        rend.material.color = pressedColor;
        yield return new WaitForSeconds(.2f);
        rend.material.color = defCol;
    }
}
