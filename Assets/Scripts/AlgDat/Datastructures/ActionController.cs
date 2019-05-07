using System;
using UnityEngine;

public class ActionController : MonoBehaviour
{
    public ActionButton add;
    public ActionButton remove;
    public ActionButton check;

    public enum State {
        Selected,
        Empty
    }

    public void UpdateLabels(bool queue)
    {
        if (queue)
        {
            add.SetLabel("Enqueue");
            remove.SetLabel("Dequeue");
        }
        else
        {
            add.SetLabel("Push");
            remove.SetLabel("Pop");
        }
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
        add.Active = pu;
        remove.Active = po;
        check.Active = ch;
    }

    public void Hint(ImageAction.ActionType type) {
        switch (type) {
            case ImageAction.ActionType.Check:
                check.Hint();
                break;
            case ImageAction.ActionType.Push:
                add.Hint();
                break;
            case ImageAction.ActionType.Pop:
                remove.Hint();
                break;
        }
    }

    public void Press(ImageAction.ActionType type, Action function=null) {
        switch (type) {
            case ImageAction.ActionType.Check:
                check.Press(pause:true, demo:true, function:function);
                break;
            case ImageAction.ActionType.Push:
                add.Press(pause:true, demo:true, function:function);
                break;
            case ImageAction.ActionType.Pop:
                remove.Press(pause:true, demo: true, function:function);
                break;
        }
    }

    public ActionButton Get(ImageAction.ActionType type) {
        switch (type) {
            case ImageAction.ActionType.Check:
                return check;
            case ImageAction.ActionType.Push:
                return add;
            case ImageAction.ActionType.Pop:
                return remove;
        }
        return null;
    }
}
