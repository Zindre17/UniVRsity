using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class DataStructure : MonoBehaviour
{
    public Transform spawnPoint;
    public TMP_Text text;
    public GameObject dataItemPrefab;
    private Vector3 dataItemScale;
    public Transform popSpawn;
    public StaticLaser laser;

    private List<GameObject> items;
    private Stack<GameObject> poppedItems;
    private List<Pixel> data;
    private Stack<Pixel> poppedData;

    private Stage.Data mode;

    private float itemThickness = 0;
    private readonly float spacing = 0.05f;

    private void Start() {
        dataItemScale = new Vector3(.6f, .6f, .6f);
        StructureItem i = dataItemPrefab.GetComponent<StructureItem>();
        itemThickness = i.Width;
        laser.SetPosition(popSpawn.position + Vector3.up * 0.3f);
        laser.Hide();
    }

    public void SetMode(Stage.Data mode) {
        this.mode = mode;
        switch (mode) {
            case Stage.Data.Stack:
                text.text = "Stack";
                break;
            case Stage.Data.Queue:
                text.text = "Queue";
                break;
            case Stage.Data.LinkedList:
                text.text = "Linked list";
                break;
        }
    }

    public void Push(Pixel p) {
        data.Add(p);
        Spawn(p.transform.position);
    }

    public void UnPush() {
        if (data.Count == 0) return;
        Pixel p = data[data.Count - 1];
        GameObject o = items[items.Count - 1];
        data.Remove(p);
        items.Remove(o);
        StartCoroutine(UnPushRoutine(p, o));
    }

    private IEnumerator UnPushRoutine(Pixel p, GameObject o) {
        float duration = .6f;
        float elapsed = 0f;
        float prevTime = Time.time;
        Vector3 start = o.transform.localScale;
        float percent;
        while(elapsed < duration) {
            percent = elapsed / duration;
            o.transform.localScale = start * (1 - percent);
            float time = Time.time;
            elapsed += time - prevTime;
            prevTime = time;
            yield return null;
        }
        Destroy(o);
        EventManager.ActionCompleted();
    }

    public Pixel Pop() {
        if (data.Count == 0) return null;
        if(poppedData.Count!=0) {
            laser.Hide();
            //Destroy(poppedI.gameObject);
            poppedItems.Peek().SetActive(false);
        } 
        switch (mode) {
            case Stage.Data.Stack:
                return StackPop();
            case Stage.Data.Queue:
                return QueuePop();
        }
        return null;
    }

    public void UnPop() {
        if (poppedData.Count == 0) return;
        StartCoroutine(mode == Stage.Data.Stack ? StackUnPopRoutine() : QueueUnPopRoutine());
    }

    private IEnumerator QueueUnPopRoutine() {
        Pixel p = poppedData.Pop();
        GameObject o = poppedItems.Pop();
        laser.Hide();
        float duration = .6f;
        float elapsed = 0f;
        float prevTime = Time.time;
        float percent;
        Vector3 start = o.transform.position;
        Vector3 end = GetPos(0);
        Vector3 mid = new Vector3(start.x, end.y, start.z);
        Vector3 path1 = mid - start;
        Vector3 path2 = end - mid;
        List<Vector3> starts;
        List<Vector3> ends;
        List<Vector3> paths;
        starts = new List<Vector3>(data.Count);
        ends = new List<Vector3>(data.Count);
        paths = new List<Vector3>(data.Count);
        Vector3 startAngle = o.transform.localRotation.eulerAngles;
        Vector3 endAngle = Vector3.zero;
        for (int i = 0; i < data.Count; i++) {
            starts.Add(items[i].transform.position);
            ends.Add(GetPos(i+1));
            paths.Add(ends[i] - starts[i]);
        }
        float part = .3f;
        while (elapsed < duration) {
            if (elapsed < part) {
                percent = elapsed / part;
                o.transform.position = start + path1 * percent;
                o.transform.localRotation = Quaternion.Euler(startAngle-(startAngle * percent));
            } else {
                percent = (elapsed - part) / (duration - part);
                o.transform.position = mid + path2 * percent;
            }
            percent = elapsed / duration;
            for(int i = 0; i < data.Count; i++) {
                items[i].transform.position = starts[i] + paths[i] * percent;
            }
            float time = Time.time;
            elapsed += time - prevTime;
            prevTime = time;
            yield return null;
        }
        o.transform.position = end;
        o.transform.localRotation = Quaternion.Euler(endAngle);
        data.Insert(0, p);
        items.Insert(0, o);
        if(poppedData.Count != 0) {
            poppedItems.Peek().SetActive(true);
            laser.Target(poppedData.Peek().surface.position, true);
        }
        EventManager.ActionCompleted();
    }

    private IEnumerator StackUnPopRoutine() {
        Pixel p = poppedData.Pop();
        GameObject o = poppedItems.Pop();
        data.Add(p);
        items.Add(o);
        float duration = .6f;
        float elapsed = 0f;
        float prevTime = Time.time;
        float percent;
        Vector3 start = o.transform.position;
        Vector3 end = GetNextPos();
        Vector3 mid = new Vector3(start.x, end.y, start.z);
        Vector3 path1 = mid - start;
        Vector3 path2 = end - mid;
        Vector3 startAngle = o.transform.localRotation.eulerAngles;
        Vector3 endAngle = Vector3.zero;
        float part = .3f;
        while(elapsed < duration) {
            if(elapsed < part) {
                percent = elapsed / part;
                o.transform.position = start + path1 * percent;
                o.transform.localRotation = Quaternion.Euler(startAngle - (startAngle * percent));
            } else {
                percent = (elapsed-part) / (duration-part);
                o.transform.position = mid + path2 * percent;
            }
            float time = Time.time;
            elapsed += time - prevTime;
            prevTime = time;
            yield return null;
        }
        o.transform.position = end;
        o.transform.localRotation = Quaternion.Euler(endAngle);
        if (poppedData.Count != 0) {
            poppedItems.Peek().SetActive(true);
            laser.Target(poppedData.Peek().surface.position, true);
        }
        EventManager.ActionCompleted();
    }

    private void Awake() {
        data = new List<Pixel>();
        poppedData = new Stack<Pixel>();
        items = new List<GameObject>();
        poppedItems = new Stack<GameObject>();
    }

    public void Restart() {
        laser.Hide();
        data.Clear();
        poppedData.Clear();
        for (int i = items.Count-1; i > -1; i--) {
            GameObject o = items[i];
            items.Remove(o);
            Destroy(o);
        }
        for(int i = 0; i < poppedItems.Count; i++) {
            Destroy(poppedItems.Pop());
        }
    }

    private Pixel StackPop() {
        Pixel poppedP = data[data.Count - 1];
        data.Remove(poppedP);
        poppedData.Push(poppedP);
        GameObject poppedI = items[items.Count - 1];
        items.Remove(poppedI);
        poppedItems.Push(poppedI);
        StartCoroutine(PopAnimation(poppedI,poppedP,false));
        return poppedP;
    }

    private Pixel QueuePop() {
        Pixel poppedP = data[0];
        data.Remove(poppedP);
        poppedData.Push(poppedP);
        GameObject poppedI = items[0];
        items.Remove(poppedI);
        poppedItems.Push(poppedI);
        StartCoroutine(PopAnimation(poppedI, poppedP,true));
        return poppedP;
    }

    private void Spawn(Vector3 origin) {
        GameObject o = Instantiate(dataItemPrefab, origin, Quaternion.identity, transform);
        o.transform.localScale = dataItemScale;
        if (itemThickness == 0) 
            itemThickness = (o.GetComponent<StructureItem>()).Width * dataItemScale.y;
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


    private IEnumerator PopAnimation(GameObject o, Pixel p,bool queue) {
        float duration = .7f;
        float elapsed = 0f;
        float part1 = .35f;
        Vector3 start = o.transform.position;
        Vector3 end = popSpawn.position + Vector3.up*0.3f;
        Vector3 mid = new Vector3(end.x, start.y, end.z);
        Vector3 path1 = mid - start;
        Vector3 path2 = end - mid;
        Vector3 endAngle = new Vector3(0, 45, 90);
        while (elapsed < duration) {
            float percent;
            if (elapsed < part1) {
                o.transform.position = start + path1 * (elapsed / part1);
            } else {
                percent = (elapsed - part1) / (duration - part1);
                o.transform.position = mid + path2 * percent;
                o.transform.localRotation = Quaternion.Euler(endAngle * percent);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        o.transform.position = end;
        o.transform.localRotation = Quaternion.Euler(endAngle);
        laser.Target(p.GetSurface(), true);
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
        EventManager.ActionCompleted();
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
        o.transform.localScale = origSize;
        o.transform.position = finalPos;
        EventManager.ActionCompleted();
    }
}
