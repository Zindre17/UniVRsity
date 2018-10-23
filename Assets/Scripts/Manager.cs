using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour {

    public static Manager instance;

    public int maxInFocus = 2;
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

    private int inFocus = 0;
    private List<GameObject> focusedObjects = new List<GameObject>();


    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        elements = new List<GameObject>(arraySize);
        arrayToSort = new int[arraySize];
        sortedArray = new int[arraySize];

        for (int i = 0; i < arraySize; i++) {
            int num = Random.Range(1, 16);
            arrayToSort[i] = num;
            sortedArray[i] = num;
        }

        System.Array.Sort(sortedArray);
    }

    void SpawnElement() {
        Debug.Log("spawning element");

        for (int i = 0; i < spawnedElements; i++) {
            elements[i].transform.Translate(new Vector3(-distanceBetweenElements / 2, 0, 0));
        }
        GameObject newE = Instantiate(elementPrefab, spawnCenter);
        elements[spawnedElements] = newE;
        newE.transform.localScale = new Vector3(1, arrayToSort[spawnedElements], 1) * scale;
        newE.transform.Translate(new Vector3(spawnedElements * distanceBetweenElements / 2, newE.transform.localScale.y / 2, 0));

        spawnedElements++;
    }

    public bool CanFocus() {
        return inFocus < maxInFocus;
    }

    public void AddInFocus(GameObject obj) {
        if (CanFocus()) {
            inFocus++;
            
            focusedObjects.Add(obj);
            if(maxInFocus == inFocus) {
                
                Swap();
            }
        }
        Debug.Log(inFocus);
    }
    public void SubInFocus(GameObject obj) {
        if(inFocus > 0) {
            inFocus--;
            focusedObjects.Remove(obj);
        }
        Debug.Log(inFocus);
    }

    private void Swap() {
        GameObject o1 = focusedObjects[0];
        GameObject o2 = focusedObjects[1];

        Vector3 o1Origin = o1.transform.position;
        Vector3 o2Origin = o2.transform.position;

        Movable m = o1.GetComponent<Movable>();
        if (m != null) {
            Vector3 forward = -o1.transform.forward;
            Vector3 side = -o2.transform.forward*2 + o2Origin;
            Vector3 back = o2Origin;
            m.SetPath(new Vector3[] { forward, side, back });
        }
        m.Move();
        m = o2.GetComponent<Movable>();
        if(m != null) {
            Vector3 forward = -o2.transform.forward;
            Vector3 side = -o1.transform.forward + o1Origin;
            Vector3 back = o1Origin;
            m.SetPath(new Vector3[] { forward, side, back });

        }
        m.Move();
    }

}
