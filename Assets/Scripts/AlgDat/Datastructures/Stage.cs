using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public Play play;
    public UseCase useCase;

    private Data data;
    private Mode mode;

    private void Start() {
        mode = Mode.Play;
        enabled = true;
    }

    private void Update() {
        if (mode == Mode.Play)
            GoToPlayMode();
        else
            GoToUseCase();
        ChangeDataModel(Data.None);
        enabled = false;
    }

    public enum Data {
        None = -1,
        Stack = 0,
        Queue = 1,
        LinkedList = 2
    }

    public enum Mode { 
        UseCase,
        Play
    }

    private void ChangeDataModel(Data model) {
        data = model;
        play.ChangeDataModel(data);
        useCase.ChangeDataModel(data);
    }

    public void ChangeDataModel(UIButton button) {
        if (!(button is ModeButton)) return;
        ChangeDataModel(((ModeButton)button).model);
    }


    public void GoToUseCase() {
        mode = Mode.UseCase;
        play.ChangeMode(mode);
        useCase.ChangeMode(mode);
    }

    public void GoToPlayMode() {
        mode = Mode.Play;
        play.ChangeMode(mode);
        useCase.ChangeMode(mode);
    }
}
