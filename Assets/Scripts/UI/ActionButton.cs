using System;
using System.Collections;
using UnityEngine;

public class ActionButton : UIButton {

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
        if (multistep)
            InProgress = true;
        base.Press(pause, function);
    }


    public void Hint() {
        StartCoroutine(HintAnimation());
    }

    internal override void UpdateColor() {
        if (inProgress)
            rend.material.color = colorManager.inProgress;
        else
            base.UpdateColor();
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
