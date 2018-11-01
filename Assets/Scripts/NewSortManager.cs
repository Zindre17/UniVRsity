using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewSortManager : MonoBehaviour {

    public static NewSortManager instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        else
            instance = this;
    }

    public Button swap, keep, compare;

    private Action action;
    private List<SortingElement> arrayToSort, selected;
    private int[] sortedArray, randomArray;

    private ISortingAlgorithm sortingAlgorithm;
    public enum SortingAlgorithm { Bubble }
    public SortingAlgorithm alg = SortingAlgorithm.Bubble;

    [Range(4,10)]
    public int arrayLength = 8;

    private bool canSelect = true;

    [Range(0.1f, 1f)]
    public float movementMagnitude = 0.4f;

    private void OnEnable()
    {
        SelectionHandler.OnSelectableClicked += TrySelecting;
    }

    private void TrySelecting(SortingElement element)
    {

    }
    private void Start()
    {
        switch (alg)
        {
            case SortingAlgorithm.Bubble:
                sortingAlgorithm = new BubbleSort(arrayLength);
                break;
        }
    }

    public void Select(SortingElement element)
    {
        foreach(SortingElement s in selected){
            if (s.Equals(element)) return;
        }
        selected.Add(element);
    }

    public void Deselect(SortingElement element)
    {
        selected.Remove(element);
    }

    //actions

    //swap indexes of the 2 selected elements and animate the swap of physical position
    private void Swap()
    {
        SortingElement s1 = selected[0];
        SortingElement s2 = selected[1];

        int temp = s1.Index;
        s1.Index = s2.Index;
        s2.Index = temp;

        Vector3 s1Pos = new Vector3(s1.transform.position.x, s1.transform.position.y, s1.transform.position.z);
        Vector3 s2Pos = new Vector3(s2.transform.position.x, s2.transform.position.y, s2.transform.position.z);

        Vector3[] s1Path = new Vector3[] { s1Pos - Vector3.forward * movementMagnitude, s2Pos - Vector3.forward * movementMagnitude, s2Pos };
        Vector3[] s2Path = new Vector3[] { s2Pos - Vector3.forward * movementMagnitude * 2, s1Pos - Vector3.forward * movementMagnitude * 2, s1Pos };

        s1.Move(s1Path);
        s2.Move(s2Path);
    }

    //move elements in comparison position back to their original position
    private void Keep()
    {
        foreach(SortingElement s in selected)
        {
            s.Move(s.transform.position + Vector3.forward * movementMagnitude);
        }
    }

    //move the 2 selected elemets forwards (into comparison position)
    private void Compare()
    {
        foreach(SortingElement s in selected)
        {
            s.Move(s.transform.position - Vector3.forward * movementMagnitude);
        }
    }

    //enable actions which are availble with current selections
    private void UpdateAvailableActions()
    {
        if(selected.Count == 2)
        {
            swap.enabled = true;
            keep.enabled = true;
            compare.enabled = true;
        }
        else
        {
            swap.enabled = false;
            keep.enabled = false;
            compare.enabled = false;
        }
    }

    //do the action if it was the correct one
    private void CompleteAction()
    {
        switch (action.type)
        {
            case Action.ActionType.Swap:
                Swap();
                break;
            case Action.ActionType.Keep:
                Keep();
                break;
            case Action.ActionType.Compare:
                Compare();
                break;
        }
    }
    private void Update()
    {
        
    }

}
