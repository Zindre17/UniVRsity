﻿using System.Collections;
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
        Debug.Log(string.Format("Element: index = {0}, parent = {1}",s.Index,s.Parent));
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
    }

    private void OnDestroy() {
        EventManager.OnActionCompleted -= ActionComplete;
        EventManager.OnSelection -= Selection;
    }

    // algorithm selection
    public void Bubble() {
        arrays.New();
        alg = new BubbleSort(arrays.Size, arrays.Array);
        Setup();
    }

    public void Insertion() {
        arrays.New();
        alg = new InsertionSort(arrays.Size, arrays.Array);
        Setup();
    }

    public void Quick() {
        arrays.New();
        alg = new QuickSort(arrays.Size, arrays.Array);
        Setup();
    }

   public void MergeSort() {
        arrays.New();
        alg = new MergeSort(arrays.Size, arrays.Array);
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
        action = new MergeAction(selected[0].Index, selected[1].Index);
        DoAction();
    }
    

    // menu

    public void Demo() {
        demo = !demo;
        menu.Demo(demo);
        if (demo) {
            if (performingAction || partialAction) return;
            DoStep();
        }
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
        ResetState();
        ClearSelections();
        UpdateActions();
        UpdateMenu();
        ResetUI();
    }

    private void ResetState() {
        demo = false;
        menu.Demo(demo);
        performingAction = false;
        partialAction = false;
    }

    private void ResetUI() {
        title.SetTitle(alg.GetName());
        message.SetMessage("");
        state.SetCompare("");
        state.SetState(alg.GetState());
        string[] pseudo = alg.GetPseudo();
        if (pseudo.Length > 1)
            state.SetCode2(pseudo[1]);
        else
            state.SetCode2("");
        state.SetCode1(pseudo[0]);
    }

    private void UpdateUI() {
        state.SetState(alg.GetState());
    }

    private void ClearUI() {
        title.SetTitle("Sorting room");
        message.SetMessage("");
        state.SetCompare("");
        state.SetState("");
        state.SetCode1("");
        state.SetCode2("");
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
            if(action.type != GameAction.GameActionType.Compare) {
                ClearSelections();
                state.SetCompare("");
            }
            //todo: this can be replaced by using GameActions as parameter in the functions...
            switch (action.type) {
                case GameAction.GameActionType.Compare:
                    state.SetCompare(arrays.Compare((CompareAction)action));
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
        } else {
            partialAction = false;
            performingAction = false;
            Hint();
        }
    }

    private void ActionComplete() {
        alg.Next();
        partialAction = false;
        performingAction = false;
        UpdateUI();
        if (demo) {
            DoStep();
        } else {
            UpdateActions();
        }
        if (alg.complete)
            arrays.Complete();
    }

    private void DoStep() {
        action = alg.GetAction();
        if (action != null)
            StartCoroutine(DoStepRoutine());
        else
            Demo();
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
                Debug.Log(string.Format("move: array = {0}, source = {1}, target = {2}", m.array, m.source, m.target));
                s1 = arrays.GetElement(m.source, m.array);
                Debug.Log(string.Format("element: array = {0}, index = {1}", s1.Parent, s1.Index));
                s1.Selected = true;
                selected.Add(s1);
                yield return new WaitForSeconds(interval);
                actions.Press(m.type);
                yield return new WaitForSeconds(interval);
                s2 = arrays.GetElement(m.target);
                Debug.Log(string.Format("element: array = {0}, index = {1}", s2.Parent, s2.Index));
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
                string left = c.index1 == -1 ? "storage" : "A[" + c.index1 + "]";
                string right = c.index2 == -1 ? "storage" : "A[" + c.index2 + "]";
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
                if (m.array==-2) {
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

    private void UpdateMenu() {
        if (alg == null)
            menu.UpdateMenu(MenuManager.State.Idle);
        else
            menu.UpdateMenu(MenuManager.State.Started);
    }
}
