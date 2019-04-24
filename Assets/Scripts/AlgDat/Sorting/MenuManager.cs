using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public UIButton newArray, restart, back;

    public enum State {
        Idle,
        Started
    }

    public void UpdateMenu(State state) {
        switch (state) {
            case State.Idle:
                newArray.Active = restart.Active = false;
                back.Active = true;
                break;
            case State.Started:
                newArray.Active = restart.Active = back.Active = true;
                break;
        }
    }

    
}

