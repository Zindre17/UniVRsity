using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWalls : MonoBehaviour
{
    public GameObject structureSelect;
    public GameObject actions;

    private void OnEnable() {
        EventManager.OnModeChanged += ChangeMode;
    }

    private void OnDisable() {
        EventManager.OnModeChanged -= ChangeMode;
    }

    public void ChangeMode(Stage.Mode mode) {
        switch (mode) {
            case Stage.Mode.Play:
                StartPlayMode();
                break;
            case Stage.Mode.UseCase:
                StartUseCaseMode();
                break;
        }
    }

    private void StartPlayMode() {
        structureSelect.SetActive(true);
        actions.SetActive(false);
    }

    private void StartUseCaseMode() {
        structureSelect.SetActive(false);
        actions.SetActive(true);
    }
}
