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
    private float interval = .4f;

    private List<SortingElement> arrayToSort;
    private SortingElement stored;
    private SortingElement prevPivot;

    private Coroutine spawnRoutine = null;

    public int[] Array {
        get;
        private set;
    }

    public int Size {
        get { return arrayLength; }
    }

    public void New() {
        if (arrayToSort == null) arrayToSort = new List<SortingElement>(arrayLength);
        GenerateRandomArray();
        if (spawnedElements < arrayLength)
            SpawnArray();
        if (spawnRoutine != null) {
            StopCoroutine(spawnRoutine);
        }
        spawnRoutine = StartCoroutine(EditArray());
    }

    public void Restart() {
        if (spawnedElements < arrayLength)
            SpawnArray();
        if (spawnRoutine != null) {
            StopCoroutine(spawnRoutine);
        }
        spawnRoutine = StartCoroutine(EditArray());
    }

    public void Swap(SwapAction action) {
        SortingElement s1, s2;
        s1 = arrayToSort[action.index1];
        s2 = arrayToSort[action.index2];
        if (s2!=null) {
            SortingElement temp = arrayToSort[s1.Index];
            arrayToSort[s1.Index] = arrayToSort[s2.Index];
            arrayToSort[s2.Index] = temp;

            s1.Swap(s2);
            s1.Correct = s1.Size == sortedArray[s1.Index];
            s2.Correct = s2.Size == sortedArray[s2.Index];
        } else {
            s1.Swap();
        }
    }

    public string Compare(CompareAction action) {
        SortingElement s1, s2;
        s1 = arrayToSort[action.index1];
        s2 = arrayToSort[action.index2];
        for (int i = 0; i < arrayToSort.Count; i++) {
            if (arrayToSort[i].Compared && arrayToSort[i].Index != s1.Index && arrayToSort[i].Index != s2.Index)
                arrayToSort[i].Compared = false;
        }

        s1.Compared = true;
        s2.Compared = true;

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

    public void Store(StoreAction action) {
        SortingElement s = arrayToSort[action.index];
        if (stored != null) {
            //Deselect(stored);
            Destroy(stored.gameObject);
        }
        stored = s;
        arrayToSort[s.Index] = CloneSortingElement(s);
        s.Store(storeCenter.position);
    }

    public void CopyTo(SortingElement source, SortingElement target) {
        if (source.Index == -1) {
            stored = CloneSortingElement(source);
            arrayToSort[target.Index] = source;
        } else {
            arrayToSort[source.Index] = CloneSortingElement(source);
            arrayToSort[target.Index] = source;
        }
        //Deselect(target);
        source.CopyTo(target);

    }

    public void Pivot(PivotAction action) {
        SortingElement s = arrayToSort[action.pivotIndex];
        if (prevPivot != null)
            prevPivot.Depivot();
        prevPivot = s;
        prevPivot.Pivot();
    }

    public void Hint(int index) {
        SortingElement s;
        if(index == -1) {
            s = stored;
        } else {
            s = arrayToSort[index];
        }
    }

    private SortingElement CloneSortingElement(SortingElement s) {
        GameObject o = Instantiate(elementPrefab);
        SortingElement c = o.GetComponent<SortingElement>();
        c.Index = s.Index;
        c.Size = s.Size;
        c.ArrayPos = s.ArrayPos;
        c.transform.position = s.ArrayPos;
        c.Correct = s.Correct;
        return c;
    }

    private IEnumerator EditArray() {
        int i;
        for(i = 0; i < spawnedElements; i++) {
            arrayToSort[i].Set(i, Array[i], Array[i] == sortedArray[i]);
            arrayToSort[i].transform.position = GetPosition(i);
            arrayToSort[i].gameObject.SetActive(false);
        }
        for (i = 0; i < spawnedElements; i++) {
            arrayToSort[i].gameObject.SetActive(true);
            yield return new WaitForSeconds(interval);
        }
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
        SortingElement e = Instantiate(elementPrefab).GetComponent<SortingElement>();
        e.Index = spawnedElements;
        e.Size = Array[spawnedElements];


        e.transform.position = GetPosition(spawnedElements);

        // add to array of all sorting elements
        arrayToSort.Add(e);

        e.Correct = e.Size == sortedArray[e.Index];
        e.ArrayPos = e.transform.position;
        spawnedElements++;
        e.gameObject.SetActive(false);
        if (spawnedElements == arrayLength) {
            //ArrayLoaded();
        }
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
