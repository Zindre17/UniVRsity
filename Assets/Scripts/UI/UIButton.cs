using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class UIButton :MonoBehaviour
{

    public Hoverable hoverable;

    [Serializable]
    public class ButtonEvent: UnityEvent<UIButton> { }

    public ButtonEvent onButtonPressed;
    public Renderer rend;

    protected ColorManager colorManager;
    protected Color defaultColor;

    private Vector3 origin;

    protected bool active = true;
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
    protected bool toggled = false;
    public bool Toggled {
        get { return toggled; }
        set {
            if(toggled != value) {
                toggled = value;
                UpdateColor();
            }
        }
    }

    protected virtual void UpdateColor() {
        if (active)
            if (toggled)
                rend.material.color = colorManager.toggleButtonOn;
            else
                rend.material.color = colorManager.button;
        else
            rend.material.color = colorManager.inactive;
    }

    private void Start() {
        colorManager = ColorManager.instance;
        defaultColor = colorManager.button;
        //MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        //mpb.SetColor("_Color", defaultColor);
        //rend.SetPropertyBlock(mpb);
        rend.material.color = defaultColor;
        origin = transform.localPosition;
    }

    public virtual bool Press(bool demo = false, bool pause = false,Action function = null) {
        if ((onButtonPressed == null || !active) && !demo) return false;
        StartCoroutine(PressAnimation(pause,function:function));
        return true;
    }

    public void Toggle() {
        Toggled = !Toggled;
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
