using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageManager : MonoBehaviour
{
    public Transform center;
    public GameObject elementPrefab;

    private SortingElement stored;

    private Coroutine routine;

    private void Awake()
    {
        Spawn();
    }

    public void Stop() {
        if (routine != null) {
            StopCoroutine(routine);
            routine = null;
        }
        stored.gameObject.SetActive(false);
    }

    public void SetName(string name)
    {
        stored.label.text = name;
    }

    public SortingElement Get() {
        return stored;
    }


    private void Spawn() {
        stored = Instantiate(elementPrefab, transform).GetComponent<SortingElement>();
        stored.gameObject.SetActive(false);
        stored.Index = -1;
        stored.Parent = -1;
    }

    public Vector3 GetCenter() {
        return center.position;
    }

    public void Hint() {
        stored.Hint();
    }

}
