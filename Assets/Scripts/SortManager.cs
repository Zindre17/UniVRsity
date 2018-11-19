﻿using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(EventManager))]
public class SortManager : MonoBehaviour {

    #region variables

    //the singleton instance
    public static SortManager instance;

    // A transform to use as the center of the spawning area
    public Transform spawnCenter;

    // the prefab to instantiate
    public GameObject elementPrefab;

    //UI buttons for actions
    public UISelectable compare, swap, keep, pivot, move, store;

    //The current attempted action and previuosly completed action
    private GameAction prevAction;
    private GameAction action;

    // Lists of SortingElements. One for all the elements, and one for the currently selected elements;
    private List<SortingElement> arrayToSort, selected;
    private SortingElement stored = null;
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

    // UI Text elements for representing the algorithms state, pseudo code, and temporary messages/feedback
    public Text state;
    public Text pseudo;
    public Text message;
    
    public Transform storeCenter;

    private bool inMultiStep = false;
    private UISelectable lastPress;

    private bool demo = false;
    private bool doStep = false;
    #endregion

    #region event subscription and unsubscrption
    //Subscribe to all events
    private void OnEnable()
    {
        EventManager.OnStartHover += TryHover;
        EventManager.OnEndHover += EndHover;
        EventManager.OnSelect += TrySelect;
        EventManager.OnUISelect += TryUISelect;
        EventManager.OnMovementStarted += AddInMotion;
        EventManager.OnMovementFinished += RemoveInMotion;
        EventManager.OnMenuSelect += HandleMenuSelect;
        EventManager.OnActionCompleted += ActionCompleted;
        EventManager.OnAlgorithmCompleted += AlgorithmCompleted;
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
        EventManager.OnMenuSelect -= HandleMenuSelect;
        EventManager.OnActionCompleted -= ActionCompleted;
        EventManager.OnAlgorithmCompleted -= AlgorithmCompleted;
        //EventManager.OnActionAccepted -= CompleteAction;
        //EventManager.OnActionRejected -= RejectAction;
    }

    #endregion

    #region Awake, Start, Update

    private void Awake()
    {
        if (instance != null)
            Destroy(this);
        else
            instance = this;
    }

    private void Start()
    {

        arrayToSort = new List<SortingElement>(arrayLength);
        selected = new List<SortingElement>(arrayLength);
        GenerateRandomArray();

        switch (alg)
        {
            case SortingAlgorithm.Bubble:
                sortingAlgorithm = new BubbleSort(arrayLength, randomArray);
                break;
            case SortingAlgorithm.Insertion:
                sortingAlgorithm = new InsertionSort(arrayLength, randomArray);
                break;
            default:
                sortingAlgorithm = new BubbleSort(arrayLength, randomArray);
                break;
        }
        UpdateAvailableActions();
        if (state != null) state.text = sortingAlgorithm.GetState();
        if (pseudo != null) pseudo.text = sortingAlgorithm.GetPseudo();
    }

    private void Update()
    {
        if (spawnedElements < arrayLength && timeBetweenSpawns <= Time.time - lastSpawnTime)
        {
            lastSpawnTime = Time.time;
            SpawnElement();
        }
        //UpdateAvailableActions();
        if (demo && doStep) {
            StartCoroutine(DoStep());
        }
    }

    #endregion

    private IEnumerator DoStep() {
        doStep = false;
        action = sortingAlgorithm.GetAction();
        float timeBetweenAction = 1f;
        switch (action.type) {
            case GameAction.GameActionType.Compare:
                CompareAction a = (CompareAction)action;
                if(selected.Count > 0) {
                    for(int i = selected.Count-1; i>-1; i--) {
                        if(selected[i].Index != a.index1 && selected[i].Index != a.index2) {
                            Deselect(selected[i]);
                            yield return new WaitForSeconds(timeBetweenAction);
                        }
                    }
                }
                if (a.index1 == -1 && !selected.Contains(stored)) {
                    Select(stored);
                    yield return new WaitForSeconds(timeBetweenAction);
                } else if (!selected.Contains(arrayToSort[a.index1])) {
                    Select(arrayToSort[a.index1]);
                    yield return new WaitForSeconds(timeBetweenAction);
                }
                if (a.index2 == -1 && !selected.Contains(stored)) {
                    Select(stored);
                    yield return new WaitForSeconds(timeBetweenAction);
                } else if (!selected.Contains(arrayToSort[a.index2])) {
                    Select(arrayToSort[a.index2]);
                    yield return new WaitForSeconds(timeBetweenAction);
                }
                break;
            case GameAction.GameActionType.Move:
                
                MoveAction m = (MoveAction)action;
                if (selected.Count > 0) {
                    for (int i = selected.Count - 1; i > -1; i--) {
                        if (i==0 && selected[0].Index != m.source) {
                            Deselect(selected[0]);
                            yield return new WaitForSeconds(timeBetweenAction);
                        } else {
                            Deselect(selected[0]);
                            yield return new WaitForSeconds(timeBetweenAction);
                        }
                    }
                }
                if (m.source == -1 && !selected.Contains(stored)) {
                    Select(stored);
                    yield return new WaitForSeconds(timeBetweenAction);
                } else if (!selected.Contains(arrayToSort[m.source])) {
                    Select(arrayToSort[m.source]);
                    yield return new WaitForSeconds(timeBetweenAction);
                }
                Select(arrayToSort[m.target]);
                break;
            case GameAction.GameActionType.Store:
                StoreAction s = (StoreAction)action;
                if (selected.Count > 0) {
                    for (int i = selected.Count - 1; i > -1; i--) {
                        if (selected[i].Index != s.index) {
                            Deselect(selected[i]);
                            yield return new WaitForSeconds(timeBetweenAction);
                        }
                    }
                }
                if (!selected.Contains(arrayToSort[s.index])) {
                    Select(arrayToSort[s.index]);
                    yield return new WaitForSeconds(timeBetweenAction);
                }
                break;
            case GameAction.GameActionType.Swap:
                SwapAction p = (SwapAction)action;
                if (selected.Count > 0) {
                    for (int i = selected.Count - 1; i > -1; i--) {
                        if (selected[i].Index != p.index1 && selected[i].Index != p.index2) {
                            Deselect(selected[i]);
                            yield return new WaitForSeconds(timeBetweenAction);
                        }
                    }
                }
                if (!selected.Contains(arrayToSort[p.index1])) {
                    Select(arrayToSort[p.index1]);
                    yield return new WaitForSeconds(timeBetweenAction);
                }
                if (!selected.Contains(arrayToSort[p.index2])) {
                    Select(arrayToSort[p.index2]);
                    yield return new WaitForSeconds(timeBetweenAction);
                }
                break;

        }
        ActionAccepted();
    }

    #region Menu interaction
    //handle menu interaction events
    private void HandleMenuSelect(MenuSelectable s)
    {
        switch (s.option)
        {
            case MenuSelectable.MenuOption.Back:
                Back();
                break;
            case MenuSelectable.MenuOption.Demo:
                Demo();
                break;
            case MenuSelectable.MenuOption.New:
                New();
                break;
            case MenuSelectable.MenuOption.Restart:
                Restart();
                break;
        }
    }

    //Change to Previous scene (in hierarchy)
    private void Back()
    {

    }

    //initiate demonstration of sorting algorithm
    private void Demo()
    {
        demo = true;
        doStep = true;
    }

    //Restart with new random array
    private void New()
    {
        selected = null;
        for (int i = arrayToSort.Count - 1; i > -1; i--)
        {
            SortingElement se = arrayToSort[i];
            arrayToSort.Remove(arrayToSort[i]);
            Destroy(se.gameObject);
        }
        spawnedElements = 0;

        Start();
    }

    //restart with the same array
    private void Restart()
    {
        selected = null;
        for(int i = arrayToSort.Count-1; i > -1; i--)
        {
            SortingElement se = arrayToSort[i];
            arrayToSort.Remove(arrayToSort[i]);
            Destroy(se.gameObject);
        }
        spawnedElements = 0;
        lastSpawnTime = 0;
    }

    #endregion

    #region Event handlers

    private void AlgorithmCompleted() {
        if (demo) demo = false;
        StartCoroutine(ShowMessage("algorithm completed!!", 10f));
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
            lastPress = s;
            if (s.IsMultiStep) {
                StartMultiStepAction(s);
            } else {
                TryAction(s);       
            }
        }
    }

    private void TryAction(UISelectable s) {
        //generate action corresponding to selections and ui press
        GenerateAction(s.actionType);
        //check if the generated action is correct and perform/not perform according to evaluation
        if (sortingAlgorithm.CorrectAction(action)) {
            ActionAccepted();
        } else {
            ActionRejected();
        }
        inMultiStep = false;
        s.InProgress = false;
    }

    private void StartMultiStepAction(UISelectable s) {
        s.InProgress = true;
        inMultiStep = true;
    }
    // the function subscribed to the OnSelect event
    //This function should check if the selection is already selected.
    //If it is, Deselect it; if it isn't, select it if there is room for more selections
    private void TrySelect(SortingElement s)
    {

        if (s != null)
        {
            if (CanSelect())
            {
                if (s.Selected)
                {
                    Deselect(s);
                    UpdateAvailableActions();
                }
                else
                {
                    Select(s);
                    UpdateAvailableActions();
                }
                if (inMultiStep) {
                    TryAction(lastPress);
                }
            }

        }
    }

    #endregion

    #region Actions and Action handlers

    private void ActionCompleted() {
        prevAction = action;
        for(int x = 0; x < arrayToSort.Count; x++) {
            arrayToSort[x].Correct = arrayToSort[x].Size == sortedArray[x];
        }

        if (action.type != GameAction.GameActionType.Compare) {
            for (int i = 0; i < arrayToSort.Count; i++) {
                if (arrayToSort[i].Compared) arrayToSort[i].Compared = false;
            }
        }
        // Decpompare if not last action was compare and deselct all selections after swap and store
        if (prevAction != null) {
            if (prevAction.type == GameAction.GameActionType.Swap ||
                prevAction.type == GameAction.GameActionType.Store ||
                prevAction.type == GameAction.GameActionType.Move) {
                for (int i = selected.Count -1; i > -1; i--) {
                    Deselect(selected[i]);
                }
            }
            
        }
        if (demo) {
            doStep = true;
        }
        UpdateAvailableActions();
    }

    private void ActionRejected() {
        StartCoroutine(ShowMessage("The algorithm expected a different action, try again", 3f));
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
        SortingElement s = selected[0];
        if (stored != null) {
            Deselect(stored);
            Destroy(stored.gameObject);
        }
        stored = s;
        arrayToSort[s.Index] = CloneSortingElement(s);
        s.Store(storeCenter.position);
    }

    private void Move() {
        SortingElement source = selected[0];
        SortingElement target = selected[1];
        if (source.Index == -1) {
            stored = CloneSortingElement(source);
            arrayToSort[target.Index] = source;
        } else {
            arrayToSort[source.Index] = CloneSortingElement(source);
            arrayToSort[target.Index] = source;
        }
        Deselect(target);
        source.CopyTo(target);
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

    //swap indexes of the 2 selected elements and animate the swap of physical position
    private void Swap()
    {
        SortingElement s1 = selected[0];
        SortingElement s2 = selected[1];

        SortingElement temp = arrayToSort[s1.Index];
        arrayToSort[s1.Index] = arrayToSort[s2.Index];
        arrayToSort[s2.Index] = temp;
        
        s1.Swap(s2);
        s1.Correct = s1.Size == sortedArray[s1.Index];
        s2.Correct = s2.Size == sortedArray[s2.Index];
        UpdateAvailableActions();

    }

    //move elements in comparison position back to their original position
    private void Keep()
    {
        SortingElement s1 = selected[0];
        SortingElement s2 = selected[1];

        s1.Selected = false;
        s2.Selected = false;
        //s1.GetComponent<Movable>().SetPath(s1.transform.position + Vector3.forward * movementMagnitude);
        //s2.GetComponent<Movable>().SetPath(s2.transform.position + Vector3.forward * movementMagnitude);

        //comparing = false;
        UpdateAvailableActions();
    }

    //move the 2 selected elemets forwards (into comparison position)
    private void Compare() {
        //comparing = true;
        SortingElement s1 = selected[0];
        SortingElement s2 = selected[1];

        for (int i = 0; i < arrayToSort.Count; i++) {
            if (arrayToSort[i].Compared && arrayToSort[i].Index != s1.Index && arrayToSort[i].Index != s2.Index)
                arrayToSort[i].Compared = false;
        }

        s1.Compared = true;
        s2.Compared = true;
        //s1.GetComponent<Movable>().SetPath(s1.transform.position - Vector3.forward * movementMagnitude);
        //s2.GetComponent<Movable>().SetPath(s2.transform.position - Vector3.forward * movementMagnitude);
        UpdateAvailableActions();
    }

    private void Pivot() {

    }

    #endregion

    #region Private helper functions

    //enable actions which are availble with current selections
    private void UpdateAvailableActions()
    {
        if (CanUISelect() && !inMultiStep && !demo)
        {
            if (selected.Count == 1) {
                if (swap != null) swap.Interactable = false;
                if (keep != null) keep.Interactable = false;
                if (compare != null) compare.Interactable = false;
                if (pivot != null) pivot.Interactable = true;
                if (store != null) store.Interactable = true;
                if (move != null) move.Interactable = true;
            } else if (selected.Count == 2) {
                if (swap != null) swap.Interactable = true;
                if (keep != null) keep.Interactable = false;
                if (compare != null) compare.Interactable = true;
                if (pivot != null) pivot.Interactable = false;
                if (store != null) store.Interactable = false;
                if (move != null) move.Interactable = false;
            } else {
                if (swap != null) swap.Interactable = false;
                if (keep != null) keep.Interactable = false;
                if (compare != null) compare.Interactable = false;
                if (pivot != null) pivot.Interactable = false;
                if (store != null) store.Interactable = false;
                if (move != null) move.Interactable = false;
            }
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

    private IEnumerator ShowMessage(string _message, float seconds)
    {
        if (message != null)
        {
            message.text = _message;
            message.enabled = true;
            yield return new WaitForSeconds(seconds);
            message.enabled = false;
        }
    }

    private void SpawnElement()
    {
        //instatiate new sorting element and set correct size and index
        SortingElement e = Instantiate(elementPrefab).GetComponent<SortingElement>();
        e.Index = spawnedElements;
        e.Size = randomArray[spawnedElements];

        float i = arrayLength % 2 == 0 ? 0.5f - arrayLength / 2 : -arrayLength / 2;
        e.transform.position = spawnCenter.position + new Vector3((spawnedElements + i) * movementMagnitude, 0, 0);

        // add to array of all sorting elements
        arrayToSort.Add(e); 
        
        e.Correct = e.Size == sortedArray[e.Index];
        e.ArrayPos = e.transform.position;
        spawnedElements++;
    }

    private void GenerateRandomArray()
    {
        randomArray = new int[arrayLength];
        sortedArray = new int[arrayLength];
        for (int i = 0; i < arrayLength; i++)
        {
            int num = UnityEngine.Random.Range(1, 16);
            randomArray[i] = num;
            sortedArray[i] = num;
        }
        Array.Sort(sortedArray);
    }

    private bool CanUISelect()
    {
        return inMotion == 0;
    }

    private void GenerateAction(GameAction.GameActionType type)
    {
        switch (type)
        {
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
            case GameAction.GameActionType.Store:
                action = new StoreAction(selected[0].Index);
                break;
            case GameAction.GameActionType.Move:
                action = new MoveAction(selected[0].Index, selected[1].Index);
                break;
        }
    }

    private void Deselect(SortingElement s)
    {
        s.Selected = false;
        selected.Remove(s);
    }

    private void Select(SortingElement s)
    {
        s.Selected = true;
        selected.Add(s);
    }

    // returns false when the user can select an item, otherwise false
    private bool CanSelect()
    {
        return inMotion == 0 && !demo;
    }

    #endregion
}
