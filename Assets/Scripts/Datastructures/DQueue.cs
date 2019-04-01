using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DQueue : MonoBehaviour
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
        StructureItem i = structure[0];
        int a = i.Value;
        structure.Remove(i);
        StartCoroutine(PopAnimation(i.gameObject));
    }

    private StructureItem Spawn(int value) {
        GameObject o;
        /*
        if (size == 1)
            o = Instantiate(itemPrefab, spawnpoint.position, spawnpoint.rotation, transform);
        else
            o = Instantiate(itemPrefab, structure[size-2].transform.position - new Vector3(0.06f, 0, 0), spawnpoint.rotation, transform);
        */
        o = Instantiate(itemPrefab, spawnpoint);
        origSize = o.transform.localScale;
        StructureItem i = o.GetComponent<StructureItem>();
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
        if(size > 0) {
            Vector3 toMove = new Vector3(0.03f, 0, 0);
            for (int i = 0; i < max; i++) {
                foreach (StructureItem item in structure) {
                    item.transform.position += toMove / max;
                }
                yield return null;
            }
        }
        isAnimating = false;
        Destroy(o);
    }

    private IEnumerator PushAnimation(GameObject o) {
        int max = 50;
        for (int i = 0; i < max; i++) {
            o.transform.localScale = origSize * i / max;
            yield return null;
        }
        if (size > 1) {
            Vector3 toMove = new Vector3(0.03f, 0, 0);
            for (int i = 0; i < max; i++) {
                foreach (StructureItem item in structure) {
                    item.transform.position += toMove / max;
                }
                yield return null;
            }
        }
        isAnimating = false;
    }

}
