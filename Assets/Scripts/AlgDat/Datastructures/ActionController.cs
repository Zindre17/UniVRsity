using System;
using UnityEngine;

public class ActionController : MonoBehaviour
{
    public ActionButton push;
    public ActionButton pop;
    public ActionButton check;

    public enum State {
        Selected,
        Empty
    }

    public void UpdateState(State state) {
        switch (state) {
            case State.Selected:
                UpdateButtons(pu: true, po: true, ch: true);
                return;
            case State.Empty:
                UpdateButtons();
                return;
        }
    }

    private void UpdateButtons(bool pu = false, bool po = false, bool ch = false) {
        push.Active = pu;
        pop.Active = po;
        check.Active = ch;
    }

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
                check.Press(pause:true, function:function);
                break;
            case ImageAction.ActionType.Push:
                push.Press(pause:true,function:function);
                break;
            case ImageAction.ActionType.Pop:
                pop.Press(pause:true,function:function);
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
