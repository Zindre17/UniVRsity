using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Hoverable))]
public class UISelectable : MonoBehaviour {

    private bool interactable = true;
    public bool Interactable {
        get { return interactable; }
        set {
            if (value != interactable) {
                interactable = value;
                if (interactable) {
                    rend.material.color = defCol;
                    hoverable.Enable();
                } else {
                    rend.material.color = disabledColor;
                    hoverable.Disable();
                }
            }
        }
    }

    private Renderer rend;
    private Color defCol;
    public Color disabledColor;
    public Color pressedColor;
    public GameAction.GameActionType actionType;

    private Hoverable hoverable;

    private void Awake() {
        rend = GetComponent<Renderer>();
        hoverable = GetComponent<Hoverable>();
        defCol = rend.material.color;
    }

    public void Press() {
        StartCoroutine(ButtonPressed());
    }

    private IEnumerator ButtonPressed() {
        rend.material.color = pressedColor;
        yield return new WaitForSeconds(.2f);
        if (interactable)
            rend.material.color = defCol;
        else
            rend.material.color = disabledColor;
    }
}
