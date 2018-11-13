using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewSortManager : MonoBehaviour {

    //the singleton instance
    public static NewSortManager instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        else
            instance = this;
    }

    // A transform to use as the center of the spawning area
    public Transform spawnCenter;

    // the prefab to instantiate
    public GameObject elementPrefab;

    //UI buttons for actions
    public UISelectable compare, swap, keep, pivot, move, store;

    //The current attempted action
    private GameAction action;

    // Lists of SortingElements. One for all the elements, and one for the currently selected elements;
    private List<SortingElement> arrayToSort, selected;

    // Interger arrays. One for the randomly generated values, and one for the sorted version of the same values;
    private int[] sortedArray, randomArray;

    // The sorting algorithm controller
    private ISortingAlgorithm sortingAlgorithm;

    // Enum for selecting the algoritm to learn/use
    public enum SortingAlgorithm { Bubble, Insertion }
    // The default algo is Bubble sort
    public SortingAlgorithm alg = SortingAlgorithm.Bubble;
    
    //Length of the array to be sorted
    [Range(4,10)]
    public int arrayLength = 8;

    //The distance when moving objects through actions
    [Range(0.1f, 1f)]
    public float movementMagnitude = 0.4f;

    //used to keep count of moving game elements and decide if a user can do a selection
    private int inMotion = 0;

    // used to keep track of how many elements have been spawned
    private int spawnedElements = 0;
    [Range(0,1)]
    public float timeBetweenSpawns = .4f;
    private float lastSpawnTime = 0;

    // used to only enable swap and keep when items are beeing compared, and also disable selections
    private bool comparing = false;

    // UI Text elements for representing the algorithms state and pseudo code
    public Text state;
    public Text pseudo;
    public Text message;

    //Subscribe to all events
    private void OnEnable()
    {
        EventManager.OnStartHover += TryHover;
        EventManager.OnEndHover += EndHover;
        EventManager.OnSelect += TrySelect;
        EventManager.OnUISelect += TryUISelect;
        EventManager.OnMovementStarted += AddInMotion;
        EventManager.OnMovementFinished += RemoveInMotion;
        //EventManager.OnActionAccepted += CompleteAction;
        //EventManager.OnActionRejected += RejectAction;
    }

    //Unsubscribe to all events
    private void OnDisable() {
        EventManager.OnStartHover -= TryHover;
        EventManager.OnEndHover -= EndHover;
        EventManager.OnSelect -= TrySelect;
        EventManager.OnUISelect -= TryUISelect;
        EventManager.OnMovementStarted -= AddInMotion;
        EventManager.OnMovementFinished -= RemoveInMotion;
        //EventManager.OnActionAccepted -= CompleteAction;
        //EventManager.OnActionRejected -= RejectAction;
    }

    // returns false when the user can select an item, otherwise false
    private bool CanSelect() {
        return inMotion == 0 && !comparing;
    }

    // the function subscribed to the OnActionRejected event
    private void RejectAction() {

    }

    // the function subscribed to the OnMovementStarted event
    private void AddInMotion() {
        inMotion++;
        
    }

    // the function subscribed to the OnMovementFinished event
    private void RemoveInMotion() {
        inMotion--;
        
    }

    // the function subscribed to the OnStartHOver event
    private void TryHover(Hoverable h) {
        h.StartHover(CanSelect());
    }

    // the function subscribed to the OnEndHover event
    private void EndHover(Hoverable h) {
        h.EndHover();
    }

    // the function subscribed to the OnUISelect event
    private void TryUISelect(UISelectable s) {
        if (s.Interactable && CanUISelect()) {
            s.Press();
            GenerateAction(s.actionType);
            if (sortingAlgorithm.CorrectAction(action)) {
                ActionAccepted();
            } else {
                ActionRejected();
            }
        }
    }

    private bool CanUISelect() {
        return inMotion == 0;
    }

    private void GenerateAction(GameAction.GameActionType type) {
        switch (type) {
            case GameAction.GameActionType.Compare:
                action = new CompareAction(selected[0].Index, selected[1].Index);
                break;
            case GameAction.GameActionType.Swap:
                action = new SwapAction(selected[0].Index, selected[1].Index);
                break;
            case GameAction.GameActionType.Keep:
                action = new KeepAction(selected[0].Index, selected[1].Index);
                break;
            case GameAction.GameActionType.Pivot:
                action = new PivotAction(selected[0].Index);
                break;
        }
    }

    // the function subscribed to the OnSelect event
    //This function should check if the selection is already selected.
    //If it is, Deselect it; if it isn't, select it if there is room for more selections
    private void TrySelect(NewSelectable s){
        
        if (s != null) {
            if (CanSelect()) {
                if (s.Selected) {
                    Deselect(s);
                    UpdateAvailableActions();
                } else {
                    Select(s);            
                    UpdateAvailableActions();
                }
            }

        }
    }

    private void Deselect(NewSelectable s) {
        SortingElement e = s.GetComponent<SortingElement>();
        s.Selected = false;
        selected.Remove(e);
    }

    private void Select(NewSelectable s) {
        SortingElement e = s.GetComponent<SortingElement>();
        s.Selected = true;
        selected.Add(e);
    }

    private void Start()
    {

        arrayToSort = new List<SortingElement>(arrayLength);
        selected = new List<SortingElement>(arrayLength);
        GenerateRandomArray();

        switch (alg)
        {
            case SortingAlgorithm.Bubble:
                sortingAlgorithm = new NewBubbleSort(arrayLength, randomArray);
                break;
            case SortingAlgorithm.Insertion:
                sortingAlgorithm = new InsertionSort(arrayLength, randomArray);
                break;
            default:
                sortingAlgorithm = new NewBubbleSort(arrayLength, randomArray);
                break;
        }
        UpdateAvailableActions();
        if (state != null) state.text = sortingAlgorithm.GetState();
        if (pseudo != null) pseudo.text = sortingAlgorithm.GetPseudo();
    }


    private void GenerateRandomArray() {
        randomArray = new int[arrayLength];
        sortedArray = new int[arrayLength];
        for (int i = 0; i < arrayLength; i++) {
            int num = UnityEngine.Random.Range(1, 16);
            randomArray[i] = num;
            sortedArray[i] = num;
        }
        Array.Sort(sortedArray);
    }
    
    //actions

    private void ActionRejected() {
        StartCoroutine(ShowMessage("The algorithm expected a different action, try again", 3f));
    }

    private IEnumerator ShowMessage(string _message, float seconds) {
        if (message != null) {
            message.text = _message;
            message.enabled = true;
            yield return new WaitForSeconds(seconds);
            message.enabled = false;
        }
    }

    private void ActionAccepted() {
        switch (action.type) {
            case GameAction.GameActionType.Compare:
                Compare();
                break;
            case GameAction.GameActionType.Keep:
                Keep();
                break;
            case GameAction.GameActionType.Swap:
                Swap();
                break;
            case GameAction.GameActionType.Pivot:
                Pivot();
                break;
            case GameAction.GameActionType.Move:
                Move();
                break;
            case GameAction.GameActionType.Store:
                Store();
                break;
        }
        sortingAlgorithm.Next();
        if (state != null) state.text = sortingAlgorithm.GetState();
    }

    private void Store() {

    }

    private void Move() {

    }

    //swap indexes of the 2 selected elements and animate the swap of physical position
    private void Swap()
    {
        SortingElement s1 = selected[0];
        SortingElement s2 = selected[1];

        int temp = s1.Index;
        s1.Index = s2.Index;
        s2.Index = temp;

        s1.GetComponent<NewSelectable>().Correct = s1.Size == sortedArray[s1.Index];
        s2.GetComponent<NewSelectable>().Correct = s2.Size == sortedArray[s2.Index];

        Vector3 s1Pos = new Vector3(s1.transform.position.x, s1.transform.position.y, s1.transform.position.z);
        Vector3 s2Pos = new Vector3(s2.transform.position.x, s2.transform.position.y, s2.transform.position.z);

        Vector3[] s1Path, s2Path;
        if (comparing) {
            //one moved straigth for the others pos, then in, while the others moves forward, to the others pos, then in
            s1Path = new Vector3[] { s2Pos , s2Pos + Vector3.forward * movementMagnitude};
            s2Path = new Vector3[] { s2Pos - Vector3.forward * movementMagnitude, s1Pos - Vector3.forward * movementMagnitude, s1Pos + Vector3.forward * movementMagnitude };
        } else {
            //one moved two forward, the other 1 forward, then to each others starting point plus the forward, then in
            s1Path = new Vector3[] { s1Pos - Vector3.forward * movementMagnitude, s2Pos - Vector3.forward * movementMagnitude, s2Pos };
            s2Path = new Vector3[] { s2Pos - Vector3.forward * movementMagnitude * 2, s1Pos - Vector3.forward * movementMagnitude *2, s1Pos};
        }

        s1.GetComponent<Movable>().SetPath(s1Path);
        s2.GetComponent<Movable>().SetPath(s2Path);

        //Deselect(s1.GetComponent<NewSelectable>());
        //Deselect(s2.GetComponent<NewSelectable>());
        comparing = false;
        UpdateAvailableActions();
    }

    //move elements in comparison position back to their original position
    private void Keep()
    {
        SortingElement s1 = selected[0];
        SortingElement s2 = selected[1];

        s1.GetComponent<Movable>().SetPath(s1.transform.position + Vector3.forward * movementMagnitude);
        s2.GetComponent<Movable>().SetPath(s2.transform.position + Vector3.forward * movementMagnitude);

        comparing = false;
        UpdateAvailableActions();
    }

    //move the 2 selected elemets forwards (into comparison position)
    private void Compare() {
        comparing = true;
        SortingElement s1 = selected[0];
        SortingElement s2 = selected[1];

        s1.GetComponent<Movable>().SetPath(s1.transform.position - Vector3.forward * movementMagnitude);
        s2.GetComponent<Movable>().SetPath(s2.transform.position - Vector3.forward * movementMagnitude);
        UpdateAvailableActions();
    }

    private void Pivot() {

    }

    //enable actions which are availble with current selections
    private void UpdateAvailableActions()
    {
        if (comparing) {
            if (swap != null) swap.Interactable = true;
            if (keep != null) keep.Interactable = true;
            if (compare != null) compare.Interactable = false;
            if (pivot != null) pivot.Interactable = false;
            if (store != null) store.Interactable = false;
            if (move != null) move.Interactable = true;
        } else {
            if(selected.Count == 1) {
                if (swap != null) swap.Interactable = false;
                if (keep != null) keep.Interactable = false;
                if (compare != null) compare.Interactable = false;
                if (pivot != null) pivot.Interactable = true;
                if (store != null) store.Interactable = false;
                if (move != null) move.Interactable = true;
            }
            else if(selected.Count == 2)
            {
                if (swap != null) swap.Interactable = true;
                if (keep != null) keep.Interactable = false;
                if (compare != null) compare.Interactable = true;
                if (pivot != null) pivot.Interactable = false;
                if (store != null) store.Interactable = false;
                if (move != null) move.Interactable = false;
            }
            else
            {
                if (swap != null) swap.Interactable = false;
                if (keep != null) keep.Interactable = false;
                if (compare != null) compare.Interactable = false;
                if (pivot != null) pivot.Interactable = false;
                if (store != null) store.Interactable = false;
                if (move != null) move.Interactable = false;
            }
        }
    }

   

    private void SpawnElement() {
        GameObject newE = Instantiate(elementPrefab, spawnCenter);
        SortingElement e = newE.GetComponent<SortingElement>();
        e.Index = spawnedElements;
        e.Size = randomArray[spawnedElements];
        arrayToSort.Add(e);
        float i = .5f - arrayLength / 2;
        newE.transform.Translate(new Vector3((spawnedElements+i) * movementMagnitude, 0, 0));
        newE.transform.localScale = new Vector3(1, e.Size, 1);
        newE.GetComponent<NewSelectable>().Correct = e.Size == sortedArray[e.Index];
        spawnedElements++;
        if (spawnedElements == arrayLength) OnSpawnedElementsComplete();
    }

    private void OnSpawnedElementsComplete() {
        UpdateAvailableActions();
    }

    private void Update() {
        if(spawnedElements < arrayLength && timeBetweenSpawns <= Time.time - lastSpawnTime) {
            lastSpawnTime = Time.time;
            SpawnElement();
        }
        
    }

}
