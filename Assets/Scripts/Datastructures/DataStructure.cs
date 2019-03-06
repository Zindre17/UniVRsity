using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class DataStructure : MonoBehaviour
{
    public Transform spawnPoint;
    public TMP_Text text;
    public GameObject dataItemPrefab;

    public Transform popSpawn;

    private List<GameObject> items;
    private List<Pixel> data;

    private Stage.Data mode;

    private float itemThickness;
    private readonly float spacing = 0.05f;

    private Pixel poppedP;
    private GameObject poppedI;

    private void Start() {
        items = new List<GameObject>();
        data = new List<Pixel>();
        itemThickness = dataItemPrefab.transform.localScale.y;
    }

    public void SetMode(Stage.Data mode) {
        this.mode = mode;
        switch (mode) {
            case Stage.Data.Stack:
                text.text = "stack";
                break;
            case Stage.Data.Queue:
                text.text = "queue";
                break;
            case Stage.Data.LinkedList:
                text.text = "linked list";
                break;

        }
    }

    public void Push(Pixel p) {
        data.Add(p);
        Spawn(p.transform.position);
    }

    public Pixel Pop() {
        if (data.Count == 0) return null;
        switch (mode) {
            case Stage.Data.Stack:
                return StackPop();
            case Stage.Data.Queue:
                return QueuePop();
        }
        return null;
    }

    private Pixel StackPop() {
        poppedP = data[data.Count - 1];
        data.Remove(poppedP);
        poppedI = items[items.Count - 1];
        items.Remove(poppedI);
        StartCoroutine(PopAnimation(poppedI,false));
        return poppedP;
    }

    private Pixel QueuePop() {
        poppedP = data[0];
        data.Remove(poppedP);
        poppedI = items[0];
        items.Remove(poppedI);
        StartCoroutine(PopAnimation(poppedI,true));
        return poppedP;
    }

    private void Spawn(Vector3 origin) {
        GameObject o = Instantiate(dataItemPrefab, origin, Quaternion.identity, transform);
        items.Add(o);
        StartCoroutine(PushAnimation(o));
    }

    private Vector3 GetNextPos() {
        int i = items.Count - 1;
        return GetPos(i);
    }

    private Vector3 GetPos(int index) {
        return spawnPoint.position + index * new Vector3(0f, itemThickness, 0f) + (index + 1) * new Vector3(0f, spacing, 0f);
    }


    private IEnumerator PopAnimation(GameObject o,bool queue) {
        float duration = .7f;
        float elapsed = 0f;
        float part1 = .35f;
        Vector3 start = o.transform.position;
        Vector3 end = popSpawn.position;
        Vector3 mid = new Vector3(end.x, start.y, end.z);
        Vector3 path1 = mid - start;
        Vector3 path2 = end - mid;
        while (elapsed < duration) {
            if(elapsed < part1) {
                o.transform.position = start + path1 * (elapsed / part1);
            } else {
                o.transform.position = mid + path2 * (elapsed - part1) / (duration - part1);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        o.transform.position = end;
        if (queue) {
            int i = 0;
            float interval = 0.1f;
            foreach (GameObject ob in items) {
                Vector3 pos = GetPos(i);
                ob.transform.position = pos;
                i++;
                yield return new WaitForSeconds(interval);
            }
        }
    }


    private IEnumerator PushAnimation(GameObject o) {
        float duration = .7f;
        float elapsed = 0f;
        float part1 = .25f;
        Vector3 origSize = o.transform.localScale;
        Vector3 finalPos = GetNextPos();
        Vector3 startPos = o.transform.position;
        Vector3 midPos = startPos - Vector3.forward;
        Vector3 trajectory = finalPos - midPos;
        while (elapsed < duration) {
            if(elapsed < part1) {
                o.transform.localScale = origSize * elapsed / part1;
                o.transform.position = startPos - Vector3.forward * elapsed/ part1;
            } else {
                o.transform.position = midPos + trajectory * (elapsed - part1) / (duration - part1);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        o.transform.position = finalPos;
    }
}
