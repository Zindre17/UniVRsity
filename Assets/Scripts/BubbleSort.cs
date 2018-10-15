using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSort : MonoBehaviour {

    public int arraySize;
    private int spawnedElements = 0;

    public float distanceBetweenElements = 0.5f;
    public float scale = 0.2f;

    public GameObject elementPrefab;
    public Transform spawnCenter;

    public float spawnInterval = 0.5f;
    private float timeSinceLastSpawn = 0f;

    private GameObject[] elements;

    private int[] arrayToSort;
    private int[] sortedArray;

    private readonly string[] pseudo = {
        "for i = 1 to A.length -1",
        "   for j = A.length downto i+1",
        "       if A[j] < A[j-1]",
        "           exchange A[j] with A[j-1]"
    };

	// Use this for initialization
	void Start () {
        elements = new GameObject[arraySize];
        arrayToSort = new int[arraySize];
        sortedArray = new int[arraySize];
        
        for(int i = 0; i < arraySize; i++) {
            int num = Random.Range(1, 16);
            arrayToSort[i] = num;
            sortedArray[i] = num;
        }

        System.Array.Sort(sortedArray);
    }
	
	// Update is called once per frame

	void Update () {
        if(arraySize > spawnedElements) {
            timeSinceLastSpawn += Time.deltaTime;
            //Debug.Log(timeSinceLastSpawn);
            if(timeSinceLastSpawn >= spawnInterval) {
                SpawnElement();
                timeSinceLastSpawn = 0;
            }
        }
	}

    void SpawnElement() {
        Debug.Log("spawning element");
       
        for(int i = 0; i < spawnedElements; i++) {
            elements[i].transform.Translate(new Vector3(-distanceBetweenElements/2, 0, 0));
        }
        GameObject newE = Instantiate(elementPrefab, spawnCenter);
        elements[spawnedElements] = newE;
        newE.transform.localScale = new Vector3(1, arrayToSort[spawnedElements], 1)*scale;
        newE.transform.Translate(new Vector3(spawnedElements*distanceBetweenElements/2, newE.transform.localScale.y/2, 0));
       
        spawnedElements++;
    }

    void Swap(int index1, int index2) {
        int temp = arrayToSort[index1];
        arrayToSort[index1] = arrayToSort[index2];
        arrayToSort[index2] = temp;


    }
}
