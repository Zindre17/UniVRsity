using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DStack : MonoBehaviour
{
    public Transform spawnpoint;
    public Transform restpoint;
    public Transform inSpawn;
    public Transform outPos;
    public GameObject itemPrefab;
    public TMPro.TextMeshPro text;
    public TMPro.TextMeshPro message;
    public TMPro.TextMeshPro instructions;
    public TMPro.TextMeshPro add, remove, usecase;
    public PlayController controller;

    private List<StructureItem> structure = new List<StructureItem>();
    private StructureItem inItem = null;
    private StructureItem outItem = null, oldOutItem = null;
    private int size = 0;
    private readonly int limit = 10;
    private bool queue = false;
    private bool animating = false;
    private Coroutine routine = null;
    public bool Animating {
        get { return animating; }
        set {
            animating = value;
            controller.UpdateStatus(animating);
        }
    }
    public bool Queue {
        get { return queue; }
        set {
            if(value != queue) {
                queue = value;
            }
            Restart();
        }
    }
    private float width;
    private Vector3 origSize;
    private Vector3 difference;

    private void Awake() {
        difference = restpoint.localPosition - spawnpoint.localPosition;
        inItem = Spawn();
    }

    private void Restart() {
        size = 0;
        if (animating)
            StopCoroutine(routine);
        if (outItem != null)
            Destroy(outItem.gameObject);
        if (oldOutItem != null)
            Destroy(oldOutItem.gameObject);
        inItem.transform.localScale = origSize;
        Animating = false;
        UpdateTexts();
        for(int i = structure.Count-1; i> -1; i--) {
            StructureItem a = structure[i];
            structure.Remove(a);
            Destroy(a.gameObject);
        }
    }

    private void UpdateTexts()
    {
        string t = "<u>{1}</u>: add element to the {0}\n\n" +
                "<u>{2}</u>: remove element from the {0}\n\n" +
                "<u>{3}</u>: shows a demonstration of how a {0} can be used in a real scenario";
        string _text, _instructons, _add, _remove, _usecase;
        if (queue)
        {
            _text = "Queue";
            _add = "Enqueue";
            _remove = "Dequeue";
            _usecase = "Use case";
            _instructons = string.Format(t, "queue", _add, _remove, _usecase);
        }
        else
        {
            _text = "Stack";
            _add = "Push";
            _remove = "Pop";
            _usecase = "Use case";
            _instructons = string.Format(t, "stack", _add, _remove, _usecase);
        }
        text.text = _text;
        instructions.text = _instructons;
        add.text = _add;
        remove.text = _remove;
        usecase.text = _usecase;
        ShowMessage("");
    }

    public void Push() {
        Animating = true;
        size++;
        StructureItem i = inItem;
        inItem = Spawn();
        structure.Add(i);
        if (size == limit) {

            StartCoroutine(PushAnimation(i, true));
            //Spawn(value, true);
            ShowMessage(string.Format("Error: Overflow\n{1} a full {0} causes the {0} to overflow.", queue? "queue":"stack", queue?"Enqueueing on":"Pushing to"));
            return;
        }
        ShowMessage();
        routine = StartCoroutine(PushAnimation(i, false));
    }


    public void FakePushComplete()
    {
        StructureItem i = structure[structure.Count - 1];
        structure.Remove(i);
        Destroy(i.gameObject);
    }

    public void Pop() {
        if (size == 0) {
            ShowMessage(string.Format("Error: Underflow\nCalling {1} on an empty {0} causes the the {0} to underflow.", queue ? "queue" : "stack", queue? "Dequeue":"Pop"));
            return;
        }
        Animating = true;
        ShowMessage();
        size--;
        StructureItem i;
        if (queue)
            i = structure[0];
        else
            i = structure[size];
        if (outItem != null)
            oldOutItem = outItem;
        outItem = i;
        structure.Remove(i);
        routine = StartCoroutine(PopAnimation(outItem));
    }

    private void PopComplete(StructureItem i)
    {
        if (oldOutItem != null)
            Destroy(oldOutItem.gameObject);
    }

    private StructureItem Spawn() {
        GameObject o = Instantiate(itemPrefab, spawnpoint);
        StructureItem i = o.GetComponent<StructureItem>();
        o.transform.position = inSpawn.transform.position;
        if (width == 0) {
            origSize = o.transform.localScale;
            width = i.Width;
        }
        return i;
    }

    private void ShowMessage(string message="") {
        this.message.text = message;
    }

    private IEnumerator PopAnimation(StructureItem item) {
        float duration = .6f;
        float elapsed = 0;
        int parts = 2;
        int part = 0;
        float partialDuration = duration / (float)parts;
        Vector3 startpos = item.transform.position;
        Vector3 endpos = outPos.transform.position;
        Vector3 midpos = new Vector3(endpos.x, startpos.y, startpos.z);
        Vector3 path1 = midpos - startpos;
        Vector3 path2 = endpos - midpos;
        float percent;
        while (part < parts)
        {
            while (elapsed < partialDuration)
            {
                percent = elapsed / partialDuration;
                if(part == 0)
                {
                    item.transform.position = startpos + path1 * percent;
                    if (oldOutItem != null)
                        oldOutItem.transform.localScale = origSize * (1 - percent);
                }
                if(part == 1)
                {
                    item.transform.position = midpos + path2 * percent;
                }
                elapsed += Time.deltaTime;
                yield return null;
            }
            elapsed = 0f;
            part++;
        }
        item.transform.position = endpos;
        if (queue) {
            int i = 1;
            float interval = 0.1f;
            while (i < structure.Count) {
                GameObject ob = structure[i].gameObject;
                Vector3 old = ob.transform.position;
                Vector3 pos = GetPos(i);
                Vector3 travel = pos - old;
                float elapsed2 = 0;
                while(elapsed2 < interval) {
                    float percent2 = elapsed2 / interval;
                    ob.transform.position = old + (travel*percent2);
                    elapsed2 += Time.deltaTime;
                    yield return null;
                }
                ob.transform.position = pos;
                i++;
                yield return null;
            }
        }
        PopComplete(item);
        Animating = false;
    }

    private Vector3 GetPos(int i) {
        return restpoint.transform.position + new Vector3(0, width * 1.5f * i, 0);
    }

    private IEnumerator PushAnimation(StructureItem i, bool f) {
        float duration = .6f;
        float elapsed = 0;
        int parts = 2;
        int part = 0;
        float partialDuration = duration / (float)parts;
        float percent;
        Vector3 endpos = GetPos(size + (f?1:0));
        Vector3 startpos = i.transform.position;
        Vector3 midpos = new Vector3(startpos.x, endpos.y, startpos.z);
        Vector3 path1 = midpos - startpos;
        Vector3 path2 = endpos - midpos;
        while (part < parts)
        {
            while (elapsed < partialDuration)
            {
                percent = elapsed / partialDuration;

                // move up to correct height
                if (part == 0)
                {
                    i.transform.position = startpos + path1 * percent;
                }
                
                if (part == 1)
                {
                    inItem.transform.localScale = origSize * percent;
                    i.transform.position = midpos + path2 * percent;
                }
                elapsed += Time.deltaTime;
                yield return null;
            }
            elapsed = 0f;
            part++;
        }
        i.transform.position = endpos;
        i.transform.localScale = origSize;
        if (f) {
            elapsed = 0f;
            while (elapsed < partialDuration) {
                i.transform.localScale = origSize * (1 - (elapsed / partialDuration));
                elapsed += Time.deltaTime;
                yield return null;
            }
            FakePushComplete();
        }
        Animating = false;
    }
}
