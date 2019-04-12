using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DStack : MonoBehaviour
{
    public Transform spawnpoint;
    public Transform restpoint;
    public GameObject itemPrefab;
    public TMPro.TextMeshPro text;
    public TMPro.TextMeshPro message;

    private List<StructureItem> structure = new List<StructureItem>();
    private int size = 0;
    private readonly int limit = 10;
    private bool queue = false;
    public bool Queue {
        get { return queue; }
        set {
            if(value != queue) {
                queue = value;
                if (queue)
                    text.text = "Queue";
                else
                    text.text = "Stack";
                Restart();
            }
        }
    }
    private float width;
    private Vector3 origSize;
    private Vector3 difference;

    private void Awake() {
        difference = restpoint.localPosition - spawnpoint.localPosition;
    }

    private void Restart() {
        size = 0;
        for(int i = structure.Count-1; i> -1; i--) {
            StructureItem a = structure[i];
            structure.Remove(a);
            Destroy(a.gameObject);
        }
    }

    public void Push(int value) {
        if (size == limit) {
            Spawn(value, true);
            ShowMessage(string.Format("Error: {0} Overflow\nPushing to a full {1} causes the {1} to overflow.",queue? "Queue": "Stack", queue? "queue":"stack"));
            return;
        }
        ShowMessage();
        size++;
        structure.Add(Spawn(value));
    }


    public void Pop() {
        if (size == 0) {
            ShowMessage(string.Format("Error: {0} Underflow\nCalling pop on an empty {1} causes the the {1} to underflow.",queue?"Queue":"Stack", queue ? "queue" : "stack"));
            return;
        }
        ShowMessage();
        size--;
        StructureItem i;
        if (queue)
            i = structure[0];
        else
            i = structure[size];
        int a = i.Value;
        structure.Remove(i);
        StartCoroutine(PopAnimation(i));
    }

    private StructureItem Spawn(int value, bool fake=false) {
        //GameObject o = Instantiate(itemPrefab, spawnpoint.position + size * new Vector3(0, 0.06f, 0), spawnpoint.rotation, transform);
        GameObject o = Instantiate(itemPrefab, spawnpoint);
        StructureItem i = o.GetComponent<StructureItem>();
        //o.transform.localPosition = GetPos(size,i.Width);
        if (width == 0) {
            origSize = o.transform.localScale;
            width = i.Width;
        }
        if(i != null) {
            i.Value = value;
        }
        StartCoroutine(PushAnimation(i,fake));
        return i;
    }

    private void ShowMessage(string message="") {
        this.message.text = message;
    }

    private IEnumerator PopAnimation(StructureItem item) {
        float duration = .6f;
        float elapsed = 0;
        GameObject o = item.gameObject;
        while(elapsed < duration) {
            float percent = elapsed / duration;
            o.transform.localScale = origSize * (1-percent);
            elapsed += Time.deltaTime;
            yield return null;
        }
        o.transform.localScale = origSize;
        if (queue) {
            int i = 0;
            float interval = 0.1f;
            while (i < structure.Count) {
                GameObject ob = structure[i].gameObject;
                Vector3 old = ob.transform.localPosition;
                Vector3 pos = GetPos(i+1);
                Vector3 travel = pos - old;
                float elapsed2 = 0;
                while(elapsed2 < interval) {
                    float percent2 = elapsed2 / interval;
                    ob.transform.localPosition = old + (travel*percent2);
                    elapsed2 += Time.deltaTime;
                    yield return null;
                }
                ob.transform.localPosition = pos;
                i++;
                yield return null;
            }
        }
        Destroy(o);
    }

    private Vector3 GetPos(int i) {
        return difference + new Vector3(0, width * 3 * i, 0);
    }

    private IEnumerator PushAnimation(StructureItem i, bool f) {
        float duration = .6f;
        float elapsed = 0;
        float part1 = .3f;
        Vector3 pos = GetPos(size + (f?1:0));
        while(elapsed < duration) {
            float percent = elapsed / part1;
            if (elapsed < part1) {
                i.transform.localScale = origSize * percent;
            } else {
                percent = (elapsed - part1) / (duration - part1);
                i.transform.localPosition = pos * percent;
            }
            elapsed += Time.deltaTime;
            yield return null;  
        }
        i.transform.localPosition = pos;
        i.transform.localScale = origSize;
        if (f) {
            elapsed = 0;
            while (elapsed < part1) {
                i.transform.localScale = origSize * (1 - (elapsed / part1));
                elapsed += Time.deltaTime;
                yield return null;
            }
            Destroy(i.gameObject);
        } 
    }
}
