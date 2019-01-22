using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public Transform spawnpoint;

    private List<StructureItem> structure = new List<StructureItem>();
    private int size = 0;

    public void Push(int value) {
        size++;
    }

    public int Pop() {
        if(size == 0)
            return -1;
        size--;
        int a = structure[size].Value;
        structure.Remove(structure[size]);
        return a;
    }
}
