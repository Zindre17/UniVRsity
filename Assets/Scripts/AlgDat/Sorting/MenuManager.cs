using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public UIButton demo, newArray, restart, back;

    public enum State {
        Idle,
        Started
    }

    public void UpdateMenu(State state) {
        switch (state) {
            case State.Idle:
                demo.Active = newArray.Active = restart.Active = false;
                back.Active = true;
                break;
            case State.Started:
                demo.Active = newArray.Active = restart.Active = back.Active = true;
                break;
        }
    }

    public void Demo(bool b) {
        demo.Toggled = b;
    }
}

