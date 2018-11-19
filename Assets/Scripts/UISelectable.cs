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
                UpdateColor();
            }
        }
    }

    private Renderer rend;
    private Color defCol;
    public Color disabledColor;
    public Color pressedColor;
    public Color inProgressColor;

    public GameAction.GameActionType actionType;
    private bool isMultiStep;
    public bool IsMultiStep { get { return isMultiStep; } }
    private bool inProgress = false;
    public bool InProgress {
        get { return inProgress; }
        set {
            if (inProgress != value) {
                inProgress = value;
                UpdateColor();
            }
        }
    }

    private void UpdateColor() {
        if (inProgress)
            rend.material.color = inProgressColor;
        else if (interactable)
            rend.material.color = defCol;
        else
            rend.material.color = disabledColor;
    }

    private Hoverable hoverable;

    private void Awake() {
        rend = GetComponent<Renderer>();
        hoverable = GetComponent<Hoverable>();
        defCol = rend.material.color;
        isMultiStep = GameAction.IsMultiStep(actionType);
    }

    public void Press() {
        if (isMultiStep) inProgress = true;
        StartCoroutine(ButtonPressed());
    }

    private IEnumerator ButtonPressed() {
        rend.material.color = pressedColor;
        yield return new WaitForSeconds(.2f);
        UpdateColor();
    }
}
