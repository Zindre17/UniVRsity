using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Play : MonoBehaviour
{
    public DStack Stack;
    public GameObject Queue;
    public GameObject LinkedList;
    public GameObject scene;

    private Stage.Data model;

    public void ChangeDataModel(Stage.Data model) {
        this.model = model;
        UpdateScene();
    }

    public void ChangeMode(Stage.Mode mode) {
        if (mode == Stage.Mode.Play) {
            scene.SetActive(true);
            Restart();
        } else {
            scene.SetActive(false);
        }
    }

    private void Restart() {
         
    }

    private void UpdateModel(bool stack, bool queue, bool linked) {
        Stack.gameObject.SetActive(stack||queue);
        if (stack)
            Stack.Queue = false;
        if (queue)
            Stack.Queue = true;
        Queue.SetActive(false);
        LinkedList.SetActive(linked);
    }

    private void UpdateScene() {
        switch (model) {
            case Stage.Data.None:
                UpdateModel(false, false, false);
                break;
            case Stage.Data.Stack:
                UpdateModel(true, false, false);
                break;
            case Stage.Data.LinkedList:
                UpdateModel(false, false, true);
                break;
            case Stage.Data.Queue:
                UpdateModel(false, true, false);
                break;
        }
    }
}
