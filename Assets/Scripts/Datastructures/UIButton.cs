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

    public void Press() {
        if (onButtonPressed == null) return;
        onButtonPressed.Invoke(this);
    }
}
