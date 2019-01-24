using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    public enum Structure {
        Stack = 0,
        Queue = 1,
        LinkedList = 2,
    }

    public List<GameObject> stages = new List<GameObject>(); 

    private Structure mode = Structure.Stack;

    public Structure Mode {
        get { return mode; }
        set {
            if(mode != value) {
                ChangeMode(value);
            }
        }
    }

    private void ChangeMode(Structure m) {
        mode = m;

    }
}
