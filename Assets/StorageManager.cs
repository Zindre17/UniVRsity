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
    }

    public Vector3 GetCenter() {
        return center.position;
    }

    public void Hint() {
        stored.Hint();
    }

    #region Store
    public void Store(SortingElement s) {
        if (stored == null) {
            //Deselect(stored);
            //Destroy(stored.gameObject);
            Spawn();
        }
        routine = StartCoroutine(StoreAnimation(s));
    }

    private IEnumerator StoreAnimation(SortingElement s) {
        stored.gameObject.SetActive(true);
        stored.Size = s.Size;
        float duration = .7f;
        float elapsed = 0;
        float part1 = .35f;
        float prevTime = Time.time;
        Vector3 mid = new Vector3(s.transform.position.x, s.transform.position.y, center.position.z);
        Vector3 path1 = mid - s.transform.position;
        Vector3 path2 = center.transform.position - mid;
        while (duration > elapsed) {
            float percent;
            if (elapsed < part1) {
                percent = elapsed / part1;
                stored.transform.position = s.transform.position + percent * path1;
            } else {
                percent = (elapsed - part1) / (duration - part1);
                stored.transform.position = mid + percent * path2;
            }
            float time = Time.time;
            elapsed += (time - prevTime);
            prevTime = time;
            yield return null;
        }
        stored.transform.position = center.transform.position;
        routine = null;
        EventManager.ActionCompleted();
    }

    #endregion

}
