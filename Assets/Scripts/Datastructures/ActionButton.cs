using System;
using System.Collections;
using UnityEngine;

public class ActionButton : UIButton {

    public Hoverable hoverable;
    private bool active = true;
    public bool Active {
        get { return active; }
        set {
            if (value != active) {
                active = value;
                hoverable.Legal = active;
                UpdateColor();
            }
        }
    }
    public bool multistep;
    private bool inProgress = false;
    public bool InProgress {
        get { return inProgress; }
        set {
            if(value != inProgress) {
                inProgress = value;
                UpdateColor();
            }
        }
    }

    public override void Press(bool pause = false, Action function = null) {
        if (active) {
            if (multistep)
                InProgress = true;
            base.Press(pause, function);
        }
    }


    public void Hint() {
        StartCoroutine(HintAnimation());
    }

    private void UpdateColor() {
        if (inProgress)
            rend.material.color = colorManager.inProgress;
        else if (active)
            rend.material.color = colorManager.button;
        else
            rend.material.color = colorManager.inactive;
    }

    private IEnumerator HintAnimation() {
        Color hint = colorManager.hint;
        yield return new WaitForSeconds(.1f);
        rend.material.color = hint;
        yield return new WaitForSeconds(.3f);
        rend.material.color = defaultColor;
        yield return new WaitForSeconds(.3f);
        rend.material.color = hint;
        yield return new WaitForSeconds(.3f);
        rend.material.color = defaultColor;
    }
}
