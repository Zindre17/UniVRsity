using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DStack : MonoBehaviour
{
    public Transform spawnpoint;
    public GameObject itemPrefab;

    private List<StructureItem> structure = new List<StructureItem>();
    private int size = 0;
    private readonly int limit = 10;
    private bool isAnimating = false;

    private Vector3 origSize;

    public void Push(int value) {
        if (size == limit)
            return;
        size++;
        structure.Add(Spawn(value));
    }

    public void Pop() {
        if (size == 0)
            return;
        size--;
        StructureItem i = structure[size];
        int a = i.Value;
        structure.Remove(i);
        StartCoroutine(PopAnimation(i.gameObject));
    }

    private StructureItem Spawn(int value) {
        GameObject o = Instantiate(itemPrefab, spawnpoint.position + size * new Vector3(0, 0.06f, 0), spawnpoint.rotation, transform);
        StructureItem i = o.GetComponent<StructureItem>();
        origSize = o.transform.localScale;
        isAnimating = true;
        if(i != null) {
            i.Value = value;
        }
        StartCoroutine(PushAnimation(o));
        return i;
    }

    private IEnumerator PopAnimation(GameObject o) {
        int max = 50;
        for (int i = max; i > -1; i--) {
            o.transform.localScale = origSize * i / max;
            yield return null;
        }
        isAnimating = false;
        Destroy(o);
    }

    private IEnumerator PushAnimation(GameObject o) {
        int max = 50;
        for(int i = 0; i < max; i++) {
            o.transform.localScale = origSize * i / max;
            yield return null;  
        }
        isAnimating = false;
    }

    private void ShowMessage() {

    }
}
