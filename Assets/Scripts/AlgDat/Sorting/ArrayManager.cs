using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayManager : MonoBehaviour
{
    public Transform arrayCenter;
    public Transform storeCenter;

    public GameObject elementPrefab;

    private int spawnedElements = 0;
    private int[] sortedArray;
    private int arrayLength = 7;

    private float movementMagnitude = 0.4f;
    private float interval = .1f;

    private List<SortingElement> arrayToSort;
    private SortingElement stored;
    private SortingElement prevPivot;
    private SortingElement compared1;
    private SortingElement compared2;

    private Coroutine spawnRoutine = null;
    private Coroutine actionRoutine = null;

    public int[] Array {
        get;
        private set;
    }

    public int Size {
        get { return arrayLength; }
    }

    public void New() {
        GenerateRandomArray();
        if (arrayToSort == null)
            arrayToSort = new List<SortingElement>(arrayLength);
        Restart();
    }

    public void Restart() {
        if (compared1 != null) 
            compared1 = compared2 = null;
        if(prevPivot != null) {
            prevPivot.Pivot = false;
            prevPivot = null;
        }
        if(stored != null)
            stored.gameObject.SetActive(false);
        if (spawnedElements < arrayLength)
            SpawnArray();
        if (spawnRoutine != null) 
            StopCoroutine(spawnRoutine);
        if (actionRoutine != null)
            StopCoroutine(actionRoutine);
        spawnRoutine = StartCoroutine(EditArray());
    }

    #region Swap

    public void Swap(SwapAction action) {
        SortingElement s1, s2;
        s1 = arrayToSort[action.index1];
        s2 = arrayToSort[action.index2];
        if(s1.Index == s2.Index) {
            actionRoutine = StartCoroutine(SwapAnimation(s1));
        } else {
            actionRoutine = StartCoroutine(SwapAnimation(s1, s2));
        }
    }

    private IEnumerator SwapAnimation(SortingElement s) {
        float prevTime = Time.time;
        float elapsed = 0;
        float duration = .6f;
        float part = duration / 2f;
        Vector3 start = s.transform.position;
        Vector3 path = -Vector3.forward * movementMagnitude;

        while(elapsed < duration) {
            float percent;
            if(elapsed < part) {
                percent = elapsed / part;
                s.transform.position = start + path * percent;
            } else {
                percent = (elapsed - part) / part;
                s.transform.position = start + path - path * percent;
            }
            float time = Time.time;
            elapsed += time - prevTime;
            prevTime = time;
            yield return null;
        }
        s.transform.position = start;
        actionRoutine = null;
        EventManager.ActionCompleted();
    }

    private IEnumerator SwapAnimation(SortingElement s1, SortingElement s2) {
        float prevTime = Time.time;
        float elapsed = 0;
        float duration = .6f;
        float part = duration/3f;
        
        Vector3 s1Point1, s1Point2, s1Point3, s1Point4;
        Vector3 s2Point1, s2Point2, s2Point3, s2Point4;
        Vector3 s1Path1, s1Path2, s1Path3;
        Vector3 s2Path1, s2Path2, s2Path3;

        s1Point1 = s1.transform.position;
        s2Point1 = s2.transform.position;

        s1Point2 = new Vector3(s1Point1.x, s1Point1.y, s1Point1.z - 1 * movementMagnitude);
        s2Point2 = new Vector3(s2Point1.x, s2Point1.y, s2Point1.z - 2 * movementMagnitude);

        s1Point3 = new Vector3(s2Point1.x, s2Point1.y, s1Point1.z - 1 * movementMagnitude);
        s2Point3 = new Vector3(s1Point1.x, s1Point1.y, s2Point1.z - 2 * movementMagnitude);

        s1Point4 = s2Point1;
        s2Point4 = s1Point1;

        s1Path1 = s1Point2 - s1Point1;
        s1Path2 = s1Point3 - s1Point2;
        s1Path3 = s1Point4 - s1Point3;

        s2Path1 = s2Point2 - s2Point1;
        s2Path2 = s2Point3 - s2Point2;
        s2Path3 = s2Point4 - s2Point3;

        while(elapsed < duration) {
            float percent;
            if(elapsed < part) {
                percent = elapsed / part;
                s1.transform.position = s1Point1 + s1Path1 * percent;
                s2.transform.position = s2Point1 + s2Path1 * percent;
            }else if( elapsed < part * 2) {
                percent = (elapsed - part) / part;
                s1.transform.position = s1Point2 + s1Path2 * percent;
                s2.transform.position = s2Point2 + s2Path2 * percent;
            } else {
                percent = (elapsed - part * 2) / part;
                s1.transform.position = s1Point3 + s1Path3 * percent;
                s2.transform.position = s2Point3 + s2Path3 * percent;
            }
            float time = Time.time;
            elapsed += time - prevTime;
            prevTime = time;
            yield return null;
        }
        
        s1.transform.position = s1Point1;
        s2.transform.position = s2Point1;

        int temp = s1.Size;
        s1.Size = s2.Size;
        s2.Size = temp;

        if (s1.Pivot) {
            prevPivot.Pivot = false;
            s2.Pivot = true;
            prevPivot = s2;
        }else if (s2.Pivot) {
            prevPivot.Pivot = false;
            s1.Pivot = true;
            prevPivot = s1;
        }

        actionRoutine = null;
        EventManager.ActionCompleted();
    }

    #endregion

    #region Compare
    public string Compare(CompareAction action) {
        SortingElement s1, s2;
        if (action.index1 == -1)
            s1 = stored;
        else
            s1 = arrayToSort[action.index1];
        if (action.index2 == -1)
            s2 = stored;
        else
            s2 = arrayToSort[action.index2];

        actionRoutine = StartCoroutine(CompareAnimation(s1, s2));

        if (s1.Equals(stored)) {
            if (s1.Size > s2.Size) {
                return string.Format("stored > A[{0}]", s2.Index);
            } else if (s1.Size == s2.Size) {
                return string.Format("stored == A[{0}]", s2.Index);
            } else {
                return string.Format("stored < A[{0}]", s2.Index);
            }
        } else if (s2.Equals(stored)) {
            if (s2.Size > s1.Size) {
                return string.Format("stored > A[{0}]", s1.Index);
            } else if (s1.Size == s2.Size) {
                return string.Format("stored == A[{0}]", s1.Index);
            } else {
                return string.Format("stored < A[{0}]", s1.Index);
            }
        } else {
            if (s1.Size > s2.Size) {
                return string.Format("A[{0}] > A[{1}]", s1.Index, s2.Index);
            } else if (s1.Size == s2.Size) {
                return string.Format("A[{0}] == A[{1}]", s1.Index, s2.Index);
            } else {
                return string.Format("A[{0}] < A[{1}]", s1.Index, s2.Index);
            }
        }
    }

    private IEnumerator CompareAnimation(SortingElement s1, SortingElement s2) {
        bool decompare1, decompare2, compare1, compare2;
        compare1 = compare2 = true;
        decompare1 = decompare2 = false;
        if (compared1 != null) {
            decompare1 = decompare2 = true;
            if(s1.Index == compared1.Index) {
                decompare1 = false;
                compare1 = false;
            }
            if(s1.Index == compared2.Index) {
                decompare2 = false;
                compare1 = false;
            }
            if(s2.Index == compared1.Index) {
                decompare1 = false;
                compare2 = false;
            }
            if(s2.Index == compared2.Index) {
                decompare2 = false;
                compare2 = false;
            }
        }
        float prevTime = Time.time;
        float duration = .3f;
        float elapsed = 0f;

        Vector3 path = -Vector3.forward * movementMagnitude;
        Vector3 s1Start, s2Start, d1Start, d2Start;
        s1Start = s1.transform.position;
        s2Start = s2.transform.position;
        d1Start = compared1 == null ? Vector3.zero : compared1.transform.position;
        d2Start = compared2 == null ? Vector3.zero : compared2.transform.position;
        
        while(elapsed < duration) {
            float percent = elapsed / duration;
            if (compare1) {
                s1.transform.position = s1Start + path * percent;
            }
            if (compare2) {
                s2.transform.position = s2Start + path * percent;
            }
            if (decompare1) {
                compared1.transform.position = d1Start - path * percent;
            }
            if (decompare2) {
                compared2.transform.position = d2Start - path * percent;
            }
            float time = Time.time;
            elapsed += time - prevTime;
            prevTime = time;
            yield return null;
        }
        if (compare1) {
            s1.transform.position = s1Start + path;
        }
        if (compare2) {
            s2.transform.position = s2Start + path;
        }
        if (decompare1) {
            compared1.transform.position = d1Start - path;
        }
        if (decompare2) {
            compared2.transform.position = d2Start - path;
        }
        compared1 = s1;
        compared2 = s2;
        actionRoutine = null;
        EventManager.ActionCompleted();
    }

    #endregion

    #region Store
    public void Store(StoreAction action) {
        SortingElement s = arrayToSort[action.index];
        if (stored == null) {
            //Deselect(stored);
            //Destroy(stored.gameObject);
            stored = CreateElement();
            stored.Index = -1;
        }
        actionRoutine = StartCoroutine(StoreAnimation(s));
    }

    private IEnumerator StoreAnimation(SortingElement s) {
        stored.gameObject.SetActive(true);
        stored.Size = s.Size;
        float duration = .7f;
        float elapsed = 0;
        float part1 = .35f;
        float prevTime = Time.time;
        Vector3 mid = new Vector3(s.transform.position.x, s.transform.position.y, storeCenter.position.z);
        Vector3 path1 = mid - s.transform.position;
        Vector3 path2 = storeCenter.transform.position - mid;
        while(duration > elapsed) {
            float percent;
            if(elapsed < part1) {
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
        stored.transform.position = storeCenter.transform.position;
        actionRoutine = null;
        EventManager.ActionCompleted();
    }

    #endregion

    #region CopyTo
    public void CopyTo(MoveAction action) {
        SortingElement s1, s2;
        if (action.source == -1)
            s1 = stored;
        else
            s1 = arrayToSort[action.source];
        if (action.target == -1)
            s2 = stored;
        else
            s2 = arrayToSort[action.target];

        actionRoutine = StartCoroutine(CopyToAnimation(s1, s2));
    }

    private IEnumerator CopyToAnimation(SortingElement source, SortingElement target) {
        target.Size = source.Size;
        float prevTime = Time.time;
        float duration = .6f;
        float elapsed = 0f;
        bool fromStore = source.Index == -1;
        float part;
        Vector3 start = source.transform.position;
        Vector3 t = target.transform.position;
        if (fromStore) {
            part = duration / 2f;
            Vector3 mid = new Vector3(t.x, t.y, start.z);
            Vector3 path1 = mid - start;
            Vector3 path2 = t - mid;
            while (elapsed < duration) {
                float percent;
                if(elapsed < part) {
                    percent = elapsed / part;
                    target.transform.position = start + path1 * percent;
                } else {
                    percent = (elapsed - part) / part;
                    target.transform.position = mid + path2 * percent;
                }
                float time = Time.time;
                elapsed += time - prevTime;
                prevTime = time;
                yield return null;
            }
        } else {
            part = duration / 3f;
            Vector3 path1 = -Vector3.forward * movementMagnitude;
            Vector3 p2 = start + path1;
            Vector3 path2 = new Vector3(t.x - start.x, t.y - start.y, 0);
            Vector3 p3 = p2 + path2;
            Vector3 path3 = t - p3;
            while (elapsed < duration) {
                float percent;
                if(elapsed < part) {
                    percent = elapsed / part;
                    target.transform.position = start + path1 * percent;
                }else if(elapsed < part * 2) {
                    percent = (elapsed - part) / part;
                    target.transform.position = p2 + path2 * percent;
                } else {
                    percent = (elapsed - part * 2) / part;
                    target.transform.position = p3 + path3 * percent;
                }
                float time = Time.time;
                elapsed += time - prevTime;
                prevTime = time;
                yield return null;
            }
        }
        target.transform.position = t;
        actionRoutine = null;
        EventManager.ActionCompleted();
    }
    #endregion

    #region Pivot
    public void Pivot(PivotAction action) {
        SortingElement s = arrayToSort[action.pivotIndex];
        if (prevPivot != null)
            prevPivot.Pivot = false;
        prevPivot = s;
        prevPivot.Pivot = true;
        EventManager.ActionCompleted();
    }
    #endregion

    public void Hint(int index) {
        SortingElement s;
        if(index == -1) {
            s = stored;
        } else {
            s = arrayToSort[index];
        }
    }

    private IEnumerator EditArray() {
        int i;
        for(i = 0; i < spawnedElements; i++) {
            arrayToSort[i].Set(i, Array[i], sortedArray[i]);
            arrayToSort[i].transform.position = GetPosition(i);
            arrayToSort[i].gameObject.SetActive(false);
        }
        for (i = 0; i < spawnedElements; i++) {
            arrayToSort[i].gameObject.SetActive(true);
            yield return new WaitForSeconds(interval);
        }
        spawnRoutine = null;
    }

    private void SpawnArray() {
        while(spawnedElements < arrayLength) {
            SpawnElement();
        }
    }

    private Vector3 GetPosition(int index) {
        float i = arrayLength % 2 == 0 ? 0.5f - arrayLength / 2 : -arrayLength / 2;
        return arrayCenter.position + new Vector3((index + i) * movementMagnitude, 0, 0);
    }

    private void SpawnElement() {
        //instatiate new sorting element and set correct size and index
        SortingElement e = CreateElement();
        e.Set(spawnedElements, Array[spawnedElements], sortedArray[spawnedElements]);
        e.transform.position = GetPosition(spawnedElements);

        // add to array of all sorting elements
        arrayToSort.Add(e);

        spawnedElements++;
        e.gameObject.SetActive(false);
    }

    private SortingElement CreateElement() {
        return Instantiate(elementPrefab).GetComponent<SortingElement>();
    }

    private void GenerateRandomArray() {
        Array = new int[arrayLength];
        sortedArray = new int[arrayLength];
        for (int i = 0; i < arrayLength; i++) {
            int num = UnityEngine.Random.Range(1, 16);
            Array[i] = num;
            sortedArray[i] = num;
        }
        System.Array.Sort(sortedArray);
    }
}
