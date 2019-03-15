using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{

    public List<GameObject> playButtons;
    public List<GameObject> useCaseButtons;

    private Stage.Mode mode;
    public Stage.Mode Mode {
        get { return mode; }
        set {
            if (mode != value) {
                mode = value;
                ChangeMode();
            }
        }
    }

    private void OnEnable() {
        EventHandler.OnModeChanged += ChangeMode;
    }

    private void OnDisable() {
        EventHandler.OnModeChanged -= ChangeMode;
    }

    private void ChangeMode(Stage.Mode mode) {
        Mode = mode;
    }
    private void ChangeMode() {
        if(mode == Stage.Mode.Play) {
            ChangePlayButtons(true);
            ChangeUseCaseButtons(false);
        } else {
            ChangePlayButtons(false);
            ChangeUseCaseButtons(true);
        }
    }

    private void ChangePlayButtons(bool b) {
        foreach (GameObject o in playButtons) {
            o.SetActive(b);
        }
    }

    private void ChangeUseCaseButtons(bool b) {
        foreach (GameObject o in useCaseButtons) {
            o.SetActive(false);
        }
    }
    
}
