using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class UIButton :MonoBehaviour
{
    [Serializable]
    public class ButtonEvent: UnityEvent<UIButton> { }

    public ButtonEvent onButtonPressed;
    public Renderer rend;

    internal ColorManager colorManager;
    internal Color defaultColor;

    private Vector3 origin;

    private void Start() {
        colorManager = ColorManager.instance;
        defaultColor = colorManager.button;
        rend.material.color = defaultColor;
        origin = transform.localPosition;
    }

    public virtual void Press(bool pause = false,Action function = null) {
        if (onButtonPressed == null) return;
        StartCoroutine(PressAnimation(pause,function:function));
    }

    public void Select() {
        rend.material.color = colorManager.toggleButtonOn;
    }

    public void Deselect() {
        rend.material.color = defaultColor;
    }

    private IEnumerator PressAnimation(bool pause,Action function = null) {
        if (pause) yield return new WaitForSeconds(.6f);
        onButtonPressed.Invoke(this);
        float duration = 0.3f;
        float elapsed = 0f;
        Vector3 path = Vector3.forward*.09f;
        Vector3 end = origin + path;
        float part1 = duration / 2;
        while(elapsed < duration) {
            if(elapsed < part1) {
                transform.localPosition = origin + path*(elapsed / part1);
            } else {
                transform.localPosition = end - path * ((elapsed - part1 / duration - part1));
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = origin;
        if (function != null) function();
    }
}
