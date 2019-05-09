using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlgoControlManager : MonoBehaviour
{
    public UIButton prev, next, demo;

    public enum State {
        Inactive,
        Active,
        InProgress,
        Finished,
        Demo
    }

    public void UpdateAlgoButtons(State state) {
        switch (state) {
            case State.Inactive:
                UpdateButtons();
                break;
            case State.Active:
                UpdateButtons(d: true, n: true);
                break;
            case State.InProgress:
                UpdateButtons(d: true, p: true, n: true);
                break;
            case State.Finished:
                UpdateButtons(p: true);
                break;
            case State.Demo:
                UpdateButtons(d: true);
                break;
        }
    }

    public void Demo(bool b) {
        demo.Toggled = b;
    }

    private void UpdateButtons(bool p= false, bool n = false, bool d = false) {
        prev.Active = p;
        next.Active = n;
        demo.Active = d;
    }

}
