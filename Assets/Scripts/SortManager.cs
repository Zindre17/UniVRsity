using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SortManager : MonoBehaviour {

    public enum SortingAlgorithm {
        Bubble,

    }

    public static SortManager instance;

    public SortingAlgorithm alg;
    private ISortingAlgorithm sortingAlgorithm;

    public int arraySize = 8;
    private int spawnedElements = 0;

    public float distanceBetweenElements = 0.5f;
    public float scale = 0.2f;

    public GameObject elementPrefab;
    public Transform spawnCenter;

    public float spawnInterval = 0.5f;
    private float timeSinceLastSpawn = 0f;

    private List<GameObject> elements;

    private int[] arrayToSort;
    private int[] sortedArray;
    
    private int movingObjects = 0;

    private Move move;

    public Text j;
    public Text i;
    public Text pseudo;
    public Text message;

    private bool demo = false;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        sortingAlgorithm = GetSelectedAlgorithm();
        move = new Move();
        elements = new List<GameObject>(arraySize);
        arrayToSort = new int[arraySize];
        sortedArray = new int[arraySize];
        movingObjects = 0;

        for (int i = 0; i < arraySize; i++) {
            int num = UnityEngine.Random.Range(1, 16);
            arrayToSort[i] = num;
            sortedArray[i] = num;
        }

        Array.Sort(sortedArray);

        pseudo.text = sortingAlgorithm.GetPseudo();
        i.text = sortingAlgorithm.I.ToString();
        j.text = sortingAlgorithm.J.ToString();
    }
    

    private ISortingAlgorithm GetSelectedAlgorithm() {
        switch (alg) {
            case SortingAlgorithm.Bubble:
                return new BubbleSort(arraySize);
        }
        return null;
    }

    void Update() {
        if (arraySize > spawnedElements) {
            timeSinceLastSpawn += Time.deltaTime;
            //Debug.Log(timeSinceLastSpawn);
            if (timeSinceLastSpawn >= spawnInterval) {
                SpawnElement();
                timeSinceLastSpawn = 0;
            }
        }
    }

    void SpawnElement() {
        Debug.Log("spawning element");

        for (int i = 0; i < spawnedElements; i++) {
            elements[i].transform.Translate(new Vector3(-distanceBetweenElements / 2, 0, 0));
        }
        GameObject newE = Instantiate(elementPrefab, spawnCenter);
        Element e = newE.GetComponent<Element>();
        e.Index = spawnedElements;
        e.Size = arrayToSort[spawnedElements];
        Debug.Log("index: " + e.Index + "| size: " + e.Size);
        elements.Add(newE);
        CheckForCorrectPosition(e);
        newE.transform.localScale = new Vector3(1, arrayToSort[spawnedElements], 1);
        newE.transform.Translate(new Vector3(spawnedElements * distanceBetweenElements / 2, 0, 0));

        spawnedElements++;
        if (spawnedElements == arraySize)
            OnSpawnComplete();
    }

    private void OnSpawnComplete() {
       
    }

    public bool CanFocus() {
        return move.GetSelectionCount() < sortingAlgorithm.RequiredSelections() && movingObjects == 0 && spawnedElements >= arraySize;
    }

    public void AddSelection(Element e) {
        if (e == null) return;
        if(move.GetSelectionCount() < sortingAlgorithm.RequiredSelections()) {
            move.AddSelection(e);
        }
        if(move.GetSelectionCount() == sortingAlgorithm.RequiredSelections()) {
            DoMove();
        }
    }

    public void RemoveSelection(Element e) {
        if (e == null) return;
        if(move.GetSelectionCount() > 0) {
            move.RemoveSelection(e);
        }
    }

    private void DoMove() {
        if (sortingAlgorithm.CorrectMove(move)) {
            if (NeedsSwap()) {
                Swap();
            } else {
                Keep();
            }
            sortingAlgorithm.Next();
            i.text = sortingAlgorithm.I.ToString();
            j.text = sortingAlgorithm.J.ToString();
            if (sortingAlgorithm.Complete()) {
                ReStart();
                //dostuff;
            }

        } else {
            Keep();
            //TODO: add some indicator to let user know the last move was incorrect
            ShowWrongMoveMessage();
        }
    }

    private void ShowWrongMoveMessage() {
        StartCoroutine(ShowMessage("wrong selctions, try again!", 5));
    }

    IEnumerator ShowMessage(string _message, float seconds) {
        message.text = _message;
        message.enabled = true;
        yield return new WaitForSeconds(seconds);
        message.enabled = false;
        
    }

    private void ReStart() {
        spawnedElements = 0;
        foreach(GameObject go in elements) {
            Destroy(go);
        }
        Start();
    }

    private bool NeedsSwap() {
        Element e1 = move.GetFirstSelection();
        Element e2 = move.GetSecondSelection();
        return (e1.Index < e2.Index &&  e1.Size > e2.Size) || (e1.Index > e2.Index && e1.Size < e2.Size);
    }

    //public void AddInFocus(GameObject obj) {
    //    Element e = obj.GetComponent<Element>();
    //    if (CanFocus()&& e != null) {
    //        inFocus++;
    //        focusedElements.Add(e);
    //        Debug.Log("index: "+e.Index + "| size: " + e.Size);
    //        if(maxInFocus == inFocus) {
    //            if ((e.Index < focusedElements[0].Index && e.Size > focusedElements[0].Size) || (e.Index > focusedElements[0].Index && e.Size < focusedElements[0].Size)) {
    //                Swap();
    //            } else {
    //                Keep();
    //            }
    //        }
    //    }
    //}
    //public void SubInFocus(GameObject obj) {
    //    Element e = obj.GetComponent<Element>();
    //    if(inFocus > 0 && e != null) {
    //        inFocus--;
    //        focusedElements.Remove(e);
    //    }
    //}

    public void AddMovingObject() {
        movingObjects++;
    }
    public void RemoveMovingObject() {
        movingObjects--;
        if(movingObjects == 0) {
            RemoveSelections();
        }
    }

    private void RemoveSelections() {
        move.GetSecondSelection().DeSelect();
        move.GetFirstSelection().DeSelect();
        move = new Move();
    }

    private void Keep() {
        Movable m = move.GetFirstSelection().GetComponent<Movable>();
        if (m != null) {
            Vector3 back = new Vector3(m.transform.position.x, m.transform.position.y, m.transform.position.z);
            Vector3 forward = new Vector3(m.transform.position.x, m.transform.position.y, m.transform.position.z - 2 * scale);
            m.SetPath(new Vector3[] { forward, back });
        }
        m = move.GetSecondSelection().GetComponent<Movable>();
        if (m != null) {
            Vector3 back = new Vector3(m.transform.position.x, m.transform.position.y, m.transform.position.z);
            Vector3 forward = new Vector3(m.transform.position.x, m.transform.position.y, m.transform.position.z - 2 * scale);
            m.SetPath(new Vector3[] { forward, back });
        }
        //Selectable s = move.GetSecondSelection().GetComponentInChildren<Selectable>();
        //if (s != null) {
        //    s.Toggle();
        //}
        //s = move.GetFirstSelection().GetComponentInChildren<Selectable>();
        //if (s != null) {
        //    s.Toggle();
        //}
    }

    private void Swap() {

        Element e1 = move.GetFirstSelection();
        Element e2 = move.GetSecondSelection();

        Vector3 e1Origin = e1.transform.position;
        Vector3 e2Origin = e2.transform.position;

        Movable m = e1.GetComponent<Movable>();
        if (m != null) {
            Vector3 forward = new Vector3(e1Origin.x, e1Origin.y, e1Origin.z - 2 * scale);
            Vector3 side = new Vector3(e2Origin.x, e2Origin.y, forward.z);
            Vector3 back = new Vector3(e2Origin.x, e2Origin.y, e2Origin.z);
            m.SetPath(new Vector3[] { forward, side, back });
        }
        //m.Move();
        m = e2.GetComponent<Movable>();
        if(m != null) {
            Vector3 forward =  new Vector3(e2Origin.x, e2Origin.y, e2Origin.z - 4*scale);
            Vector3 side = new Vector3(e1Origin.x, e1Origin.y, forward.z);
            Vector3 back = new Vector3(e1Origin.x, e1Origin.y, e1Origin.z);
            m.SetPath(new Vector3[] { forward, side, back });

        }
        //m.Move();


        GameObject temp = elements[e1.Index];
        elements[e1.Index] = elements[e2.Index];
        elements[e2.Index] = temp;

        int tmp = arrayToSort[e1.Index];
        arrayToSort[e1.Index] = arrayToSort[e2.Index];
        arrayToSort[e2.Index] = tmp;

        tmp = e1.Index;
        e1.Index = e2.Index;
        e2.Index = tmp;

        CheckForCorrectPosition(e1);
        CheckForCorrectPosition(e2);

        //Selectable f1 = e1.GetComponentInChildren<Selectable>();
        //Selectable f2 = e2.GetComponentInChildren<Selectable>();
        //if(f2!=null)
        //    f2.Toggle();
        //if(f1!=null)
        //    f1.Toggle();
    }

    private void CheckForCorrectPosition(Element e) {
        Selectable f = e.GetComponentInChildren<Selectable>();
        if (f != null) {
            if (e.Size == sortedArray[e.Index]) {
                f.SetCorrect(true);
            } else {
                f.SetCorrect(false);
            }
        }
    }


}
