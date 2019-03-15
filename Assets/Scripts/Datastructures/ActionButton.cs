using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButton : UIButton { 
    
    public void Hint() {
        StartCoroutine(HintAnimation());
    }

    private IEnumerator HintAnimation() {
        Color hint = colorManager.hintColor;
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
