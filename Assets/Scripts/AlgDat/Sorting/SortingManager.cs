using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingManager : MonoBehaviour
{
    public Arrays arrays;
    public StateManager state;
    public TitleManager title;
    public MessagesManager message;
    public ActionManager actions;
    public MenuManager menu;
    public Comparison comparison;
    public AlgoControlManager algoManager;
    public AlgSelectManager algSelectManager;

    private SortingAlgorithm alg;

    private bool demo = false;
    private bool performingAction = false;
    private bool partialAction = false;

    private GameAction action;

    private List<Selectable> selected;

    private void Awake() {
        selected = new List<Selectable>();
        EventManager.OnSelection += Selection;
        EventManager.OnActionCompleted += ActionComplete;
    }

    private void Selection(Selectable s) {
        if (demo||performingAction) return;
        if (partialAction) {
            if (selected.Contains(s)) return;
            s.Selected = true;
            selected.Add(s);
            ((PartialGameAction)action).SecondPart(s.Index);
            actions.PartialActionComplete();
            DoAction();
        } else {
            if (selected.Contains(s)) {
                selected.Remove(s);
                s.Selected = false;
            } else {
                if(selected.Count > 0) {
                    if(!selected[0].GetType().Equals(s.GetType())) {
                        ClearSelections();
                    }
                }
                s.Selected = true;
                selected.Add(s);
            }
        }
        UpdateActions();
    }

    private void ClearSelections() {
        for(int i = selected.Count-1; i > -1; i--) {
            selected[i].Selected = false;
            selected.Remove(selected[i]);
        }
    }

    private void Start() {
        StartCoroutine(LateStart());
    }

    private IEnumerator LateStart() {
        yield return new WaitForSeconds(.01f);
        ClearUI();
        UpdateActions();
        UpdateMenu();
        UpdateAlgo();
        message.Intro();
    }

    private void OnDestroy() {
        EventManager.OnActionCompleted -= ActionComplete;
        EventManager.OnSelection -= Selection;
    }

    // algorithm selection
    public void Bubble() {
        arrays.New();
        alg = new BubbleSort(arrays.Size, arrays.Array);
        algSelectManager.Bubble();
        Setup();
    }

    public void Insertion() {
        arrays.New();
        alg = new InsertionSort(arrays.Size, arrays.Array);
        algSelectManager.Insertion();
        Setup();
    }

    public void Quick() {
        arrays.New();
        alg = new QuickSort(arrays.Size, arrays.Array);
        algSelectManager.Quick();
        Setup();
    }

   public void MergeSort() {
        arrays.New();
        alg = new MergeSort(arrays.Size, arrays.Array);
        algSelectManager.Merge();
        Setup();
    }

    // actions
    public void Compare() {
        action = new CompareAction(selected[0].Parent, selected[0].Index, selected[1].Parent, selected[1].Index);
        DoAction();
    }

    public void Swap() {
        if (selected.Count > 1)
            action = new SwapAction(selected[0].Index, selected[1].Index);
        else
            action = new SwapAction(selected[0].Index, selected[0].Index);
        DoAction();
    }

    public void Pivot() {
        action = new PivotAction(selected[0].Index);
        DoAction();
    }

    public void Store() {
        action = new StoreAction(selected[0].Index);
        DoAction();
    }

    public void CopyTo() {
        action = new MoveAction(selected[0].Parent, selected[0].Index, -1);
        partialAction = true;
        UpdateActions();
    }

    public void Split() {
        action = new SplitAction(selected[0].Index);
        DoAction();
    }

    public void Merge() {
        PartialArray p1 = (PartialArray)selected[0], p2 = (PartialArray)selected[1];
        action = new MergeAction(p1.Index, p2.Index);
        DoAction();
    }
    

    // menu

    public void Demo() {
        demo = !demo;
        UpdateAlgo();
        UpdateAlgSelection();
        algoManager.Demo(demo);
        if (demo) {
            if (performingAction || partialAction) return;
            DoStep();
        }
    }

    public void Next() {
        performingAction = true;
        UpdateAlgo();
        UpdateAlgSelection();
        DoStep();
    }

    public void Prev() {
        UndoStep();
    }

    public void NewArray() {
        arrays.New();
        alg.Update(arrays.Array);
        Setup();
    }

    public void Restart() {
        alg.Restart();
        Setup();
    }

    // Helper functions

    private void Setup() {
        arrays.Restart();
        arrays.ReNameStorage(alg.GetStorageName());
        ResetState();
        ClearSelections();
        UpdateActions();
        UpdateAlgo();
        UpdateMenu();
        ResetUI();
    }

    private void ResetState() {
        demo = false;
        algoManager.Demo(demo);
        performingAction = false;
        partialAction = false;
    }

    private void ResetUI() {
        title.SetTitle(alg.GetName());
        message.SetMessage("");
        comparison.Clear();
        state.SetState(alg.GetState());
        string[] pseudo = alg.GetPseudo();
        bool extra = alg.NeedsExtraSpace();
        if (pseudo.Length > 1)
        {
            state.SetCode2(pseudo[1], extra);
        }
        else
            state.SetCode2("", extra);
        state.SetCode1(pseudo[0]);
    }

    private void UpdateUI() {
        state.SetState(alg.GetState());
    }

    private void ClearUI() {
        title.SetTitle("Sorting room");
        message.SetMessage("");
        comparison.Clear();
        state.SetState("");
        state.SetCode1("");
        state.SetCode2("", false);
    }

    private void UpdateActions() {
        if (partialAction || demo || performingAction) {
            actions.UpdateButtons(ActionManager.State.Busy);
            return;
        }
        int count = selected.Count;
        if (count == 0) {
            actions.UpdateButtons(ActionManager.State.None);
        } else if (count == 1) {
            if (selected[0].GetType() == typeof(ArrayManager) || selected[0].GetType() == typeof(PartialArray))
                actions.UpdateButtons(ActionManager.State.Array);
            else
                actions.UpdateButtons(ActionManager.State.Single);
        } else if (count == 2) {
            if (selected[0].GetType() == typeof(PartialArray))
                actions.UpdateButtons(ActionManager.State.Arrays);
            else
                actions.UpdateButtons(ActionManager.State.Double);
        } else {
            if (selected[0].GetType() == typeof(PartialArray))
                actions.UpdateButtons(ActionManager.State.ManyA);
            else
                actions.UpdateButtons(ActionManager.State.Many);
        }
    }

    private void DoAction() {
        if (alg.CorrectAction(action)) {
            performingAction = true;
            UpdateAlgo();
            UpdateAlgSelection();
            comparison.Clear();
            switch (action.type) {
                case GameAction.GameActionType.Compare:
                    comparison.Compare((SortingElement)selected[0], (SortingElement)selected[1],alg.step);
                    break;
                case GameAction.GameActionType.Swap:
                    arrays.Swap((SwapAction)action);
                    break;
                case GameAction.GameActionType.Store:
                    arrays.Store((StoreAction)action);
                    break;
                case GameAction.GameActionType.Pivot:
                    arrays.Pivot((PivotAction)action);
                    break;
                case GameAction.GameActionType.Move:
                    arrays.CopyTo((MoveAction)action);
                    break;
                case GameAction.GameActionType.Split:
                    arrays.Split((SplitAction)action);
                    break;
                case GameAction.GameActionType.Merge:
                    arrays.Merge((MergeAction)action);
                    break;
            }
            ClearSelections();
        } else {
            partialAction = false;
            performingAction = false;
            ClearSelections();
            UpdateActions();
            Hint();
        }
    }

    private void ActionComplete(bool reverse) {
        if (undoMergeInProgress) {
            UndoStep();
            return;
        }
        if (alg.Complete)
            arrays.UnComplete();
        if(!reverse)
            alg.Next();
        else
            alg.Prev();
        partialAction = false;
        performingAction = false;
        UpdateUI();
        if (alg.Complete)
        {
            arrays.Complete();
            comparison.Clear();
            actions.UpdateButtons(ActionManager.State.Busy);
            if (demo)
                algoManager.Demo(demo = false);
            algoManager.UpdateAlgoButtons(AlgoControlManager.State.Finished);
            return;
        }
        UpdateActions();
        UpdateAlgo();
        UpdateAlgSelection();
        if (demo)
        {
            DoStep();
        }
        
    }

    private void DoStep() {
        action = alg.GetAction();
        if (action != null)
        {
            performingAction = true;
            StartCoroutine(DoStepRoutine());
        }
        else if (demo)
            Demo();
    }

    private bool undoMergeInProgress = false;
    private void UndoStep() {
        performingAction = true;
        UpdateAlgo();
        UpdateAlgSelection();
        GameAction a = alg.GetAction(alg.step - 1);
        if(alg.GetType() == typeof(MergeSort) && !undoMergeInProgress) {
            MergeSort s = (MergeSort)alg;
            if (s.ReverseMerge()) {
                undoMergeInProgress = true;
                arrays.UndoMerge();
                return;
            }
        }
        undoMergeInProgress = false;
        //reverse this action
        switch (a.type) {
            case GameAction.GameActionType.Compare:
                comparison.Reverse(alg.step);
                return;
            case GameAction.GameActionType.Swap:
                arrays.Swap((SwapAction)a, true);
                break;
            case GameAction.GameActionType.Pivot:
                arrays.Pivot((PivotAction)a, true);
                break;
            case GameAction.GameActionType.Store:
                arrays.Store((StoreAction)a,true);
                break;
            case GameAction.GameActionType.Move:
                arrays.CopyTo((MoveAction)a,true);
                break;
            case GameAction.GameActionType.Split:
                arrays.Unsplit();
                break;
            case GameAction.GameActionType.Merge:
                arrays.Unmerge();
                break;
        }
        comparison.LoadPrev(alg.step);
    }

    private IEnumerator DoStepRoutine() {
        yield return new WaitForSeconds(.4f);
        if(selected.Count != 0) {
            ClearSelections();
            yield return new WaitForSeconds(.3f);
        }
        message.SetMessage(GetStepString());
        float interval = .3f;
        Selectable s1, s2;
        switch (action.type) {
            case GameAction.GameActionType.Compare:
                CompareAction c = (CompareAction)action;
                s1 = arrays.GetElement(c.index1, c.array1);
                s1.Selected = true;
                selected.Add(s1);
                yield return new WaitForSeconds(interval);
                s2 = arrays.GetElement(c.index2, c.array2);
                s2.Selected = true;
                selected.Add(s2);
                yield return new WaitForSeconds(interval);
                actions.Press(c.type);
                break;
            case GameAction.GameActionType.Pivot:
                PivotAction p = (PivotAction)action;
                s1 = arrays.GetElement(p.pivotIndex);
                s1.Selected = true;
                selected.Add(s1);
                yield return new WaitForSeconds(interval);
                actions.Press(p.type);
                yield return new WaitForSeconds(interval);
                break;
            case GameAction.GameActionType.Store:
                StoreAction st = (StoreAction)action;
                s1 = arrays.GetElement(st.index);
                s1.Selected = true;
                selected.Add(s1);
                yield return new WaitForSeconds(interval);
                actions.Press(st.type);
                break;
            case GameAction.GameActionType.Swap:
                SwapAction sw = (SwapAction)action;
                s1 = arrays.GetElement(sw.index1);
                s1.Selected = true;
                selected.Add(s1);
                yield return new WaitForSeconds(interval);
                s2 = arrays.GetElement(sw.index2);
                s2.Selected = true;
                selected.Add(s2);
                yield return new WaitForSeconds(interval);
                actions.Press(sw.type);
                break;
            case GameAction.GameActionType.Move:
                MoveAction m = (MoveAction)action;
                s1 = arrays.GetElement(m.source, m.array);
                s1.Selected = true;
                selected.Add(s1);
                yield return new WaitForSeconds(interval);
                actions.Press(m.type);
                yield return new WaitForSeconds(interval);
                s2 = arrays.GetElement(m.target);
                s2.Selected = true;
                selected.Add(s2);
                ((PartialGameAction)action).SecondPart(s2.Index);
                actions.PartialActionComplete();
                DoAction();
                break;
            case GameAction.GameActionType.Split:
                SplitAction sp = (SplitAction)action;
                s1 = arrays.GetArray(sp.array);
                s1.Selected = true;
                selected.Add(s1);
                yield return new WaitForSeconds(interval);
                actions.Press(sp.type);
                break;
            case GameAction.GameActionType.Merge:
                MergeAction me = (MergeAction)action;
                s1 = arrays.GetArray(me.a1);
                s1.Selected = true;
                selected.Add(s1);
                yield return new WaitForSeconds(interval);
                s2 = arrays.GetArray(me.a2);
                s2.Selected = true;
                selected.Add(s2);
                yield return new WaitForSeconds(interval);
                actions.Press(me.type);
                break;
        }
    }

    private string GetStepString() {
        string s = "";
        switch (action.type) {
            case GameAction.GameActionType.Compare:
                CompareAction c = (CompareAction)action;
                string left, right;
                if (c.array1 < 0)
                {
                    left = c.index1 == -1 ? "storage" : "A[" + c.index1 + "]";
                    right = c.index2 == -1 ? "storage" : "A[" + c.index2 + "]";
                }
                else
                {
                    left = c.array1 % 2 == 0 ? "L[" + c.index1 + "]" : "R[" + c.index1 + "]";
                    right = c.array2 % 2 == 0 ? "L[" + c.index2 + "]" : "R[" + c.index2 + "]";
                }
                s = string.Format("Comparing {0} and {1}", left, right);
                break;
            case GameAction.GameActionType.Pivot:
                PivotAction p = (PivotAction)action;
                s = "Setting A[" + p.pivotIndex + "] as pivot";
                break;
            case GameAction.GameActionType.Store:
                StoreAction st = (StoreAction)action;
                s = "Storing A[" + st.index + "]";
                break;
            case GameAction.GameActionType.Swap:
                SwapAction sw = (SwapAction)action;
                s = "Swapping A[" + sw.index1 + "] and A[" + sw.index2 + "]";
                break;
            case GameAction.GameActionType.Move:
                MoveAction m = (MoveAction)action;
                string from, to;
                if(m.target == -2) {
                    to = "merge";
                } else {
                    to = "A[" + m.target + "]";
                }
                if (m.array==-1) {
                    if (m.source == -1)
                        from = "storage";
                    else
                        from = "A[" + m.source + "]";
                } else if(m.array%2 == 0) {
                    from = "L[" + m.source + "]";
                } else {
                    from = "R[" + m.source + "]";
                }

                s = "Copying from " + from + " to " + to;
                break;
            case GameAction.GameActionType.Split:
                SplitAction sp = (SplitAction)action;
                if (sp.array == -1)
                    s = "Splitting Array";
                else if (sp.array % 2 == 0)
                    s = "Splitting left sub-array";
                else
                    s = "Splitting right sub-array";
                break;
            case GameAction.GameActionType.Merge:
                s = "Merging sub-arrays";
                break;
        }
        return s;
    }

    private void Hint() {
        GameAction a = alg.GetAction();
        if (a == null) return;
        arrays.Hint(a);
        actions.Hint(a.type);
    }

    private void UpdateAlgSelection()
    {
        if(performingAction || partialAction || demo)
        {
            algSelectManager.UpdateStates(false);
        }
        else
        {
            algSelectManager.UpdateStates(true);
        }
    }

    private void UpdateAlgo() {
        if (alg == null)
            algoManager.UpdateAlgoButtons(AlgoControlManager.State.Inactive);
        else if (demo)
            algoManager.UpdateAlgoButtons(AlgoControlManager.State.Demo);
        else if (performingAction || partialAction)
            algoManager.UpdateAlgoButtons(AlgoControlManager.State.Inactive);
        else if (alg.step == 0)
            algoManager.UpdateAlgoButtons(AlgoControlManager.State.Active);
        else if (alg.Complete)
            algoManager.UpdateAlgoButtons(AlgoControlManager.State.Finished);
        else
            algoManager.UpdateAlgoButtons(AlgoControlManager.State.InProgress);
    }

    private void UpdateMenu() {
        if (alg == null)
            menu.UpdateMenu(MenuManager.State.Idle);
        else
            menu.UpdateMenu(MenuManager.State.Started);
    }
}
