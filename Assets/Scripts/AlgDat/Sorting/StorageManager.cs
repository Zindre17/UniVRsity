using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageManager : MonoBehaviour
{
    public Transform center;
    public GameObject elementPrefab;

    private SortingElement stored;

    private Coroutine routine;

    public void Stop() {
        if (routine != null) {
            StopCoroutine(routine);
            routine = null;
        }
        if (stored != null)
            stored.gameObject.SetActive(false);
    }

    public SortingElement Get() {
        if (stored == null)
            Spawn();
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
