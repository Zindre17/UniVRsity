using System;
using UnityEngine;

public class ActionController : MonoBehaviour
{
    public ActionButton push;
    public ActionButton pop;
    public ActionButton check;

    public void Hint(ImageAction.ActionType type) {
        switch (type) {
            case ImageAction.ActionType.Check:
                check.Hint();
                break;
            case ImageAction.ActionType.Push:
                push.Hint();
                break;
            case ImageAction.ActionType.Pop:
                pop.Hint();
                break;
        }
    }

    public void Press(ImageAction.ActionType type, Action function) {
        switch (type) {
            case ImageAction.ActionType.Check:
                check.Press(true,function);
                break;
            case ImageAction.ActionType.Push:
                push.Press(true,function);
                break;
            case ImageAction.ActionType.Pop:
                pop.Press(true,function);
                break;
        }
    }

    public ActionButton Get(ImageAction.ActionType type) {
        switch (type) {
            case ImageAction.ActionType.Check:
                return check;
            case ImageAction.ActionType.Push:
                return push;
            case ImageAction.ActionType.Pop:
                return pop;
        }
        return null;
    }
}
