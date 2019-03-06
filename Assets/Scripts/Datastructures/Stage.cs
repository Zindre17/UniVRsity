using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public GameObject stack;
    public GameObject queue;
    public GameObject linkedList;
    public GameObject useCaseObject;
    public UIWalls uiWalls;

    private UseCase useCase;

    private List<GameObject> structures;
    private Data data;
    private Mode mode;

    private void Start() {
        structures = new List<GameObject> {
            stack,
            queue,
            linkedList
        };

        data = Data.Stack;
        mode = Mode.Play;

        if (useCaseObject != null) {
            useCase = useCaseObject.GetComponent<UseCase>();
        }
    }

    public enum Data {
        Stack = 0,
        Queue = 1,
        LinkedList = 2
    }

    public enum Mode { 
        UseCase,
        Play
    }


    public void ChangeDataModel(UIButton button) {
        if (!(button is ModeButton)) return;
        data = ((ModeButton)button).model;
        ChangeDataModel((int)data);
    }

    private void ChangeDataModel(int mode) {
        for(int i = 0; i < structures.Count; i++) {
            if (i == mode)
                structures[i].SetActive(true);
            else
                structures[i].SetActive(false);
        }
    }

    public void GoToUseCase() {
        mode = Mode.UseCase;
        ChangeDataModel(-1); //no data model active
        useCaseObject.SetActive(true);
        useCase.Data = data;
        uiWalls.ChangeMode(mode);
    }

    public void GoToPlayMode() {
        mode = Mode.Play;
        useCaseObject.SetActive(false);
        uiWalls.ChangeMode(mode);
        ChangeDataModel((int)data);

    }
}
