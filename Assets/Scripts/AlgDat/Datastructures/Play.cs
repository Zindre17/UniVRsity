using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Play : MonoBehaviour
{
    public DStack Stack;
    public GameObject LinkedList;
    public GameObject scene;
    public TMPro.TextMeshPro instructions;

    private readonly static string defInstructions = "Play around with the different data structures to see how they work";
    private Stage.Data model;

    public void ChangeDataModel(Stage.Data model) {
        this.model = model;
        UpdateScene();
        if (model == Stage.Data.None) 
            instructions.text = defInstructions;
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
        UpdateScene();
    }

    private void UpdateModel(bool stack, bool queue, bool linked) {
        Stack.gameObject.SetActive(stack||queue);
        if (stack)
            Stack.Queue = false;
        if (queue)
            Stack.Queue = true;
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
