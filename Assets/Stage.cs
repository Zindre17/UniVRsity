using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public GameObject stack;
    public GameObject queue;
    public GameObject linkedList;

    private List<GameObject> structures;

    private void Start() {
        structures = new List<GameObject> {
            stack,
            queue,
            linkedList
        };
    }

    public enum Mode {
        stack = 0,
        queue = 1,
        linked = 2
    }

    public void ChangeMode(int mode) {
        for(int i = 0; i < structures.Count; i++) {
            if (i == mode)
                structures[i].SetActive(true);
            else
                structures[i].SetActive(false);
        }
    }
}
