using System.Collections.Generic;
using UnityEngine;

public class SortingManager : MonoBehaviour
{
    public ArrayManager array;
    public StateManager state;
    public TitleManager title;
    public MessagesManager message;
    public SortingToggles toggles;
    public ActionManager actions;

    private ISortingAlgorithm alg;

    private bool demo = false;
    private bool performingAction = false;
    private bool partialAction = false;

    private GameAction action;

    private List<SortingElement> selected;

    private void Awake() {
        selected = new List<SortingElement>();
        EventManager.OnSelect += Select;
    }

    private void OnDestroy() {
        EventManager.OnSelect -= Select;
    }

    // selection
    public void Select(SortingElement s) {
        if (demo || performingAction) return;
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
                s.Selected = true;
                selected.Add(s);
            }
        }
        UpdateActions();
    }

    private void UpdateActions() {
        if(partialAction || demo || performingAction) {
            actions.UpdateButtons(ActionManager.State.Busy);
            return;
        }
        int count = selected.Count;
        if (count == 0) {
            actions.UpdateButtons(ActionManager.State.None);
        } else if (count == 1) {
            actions.UpdateButtons(ActionManager.State.Single);
        } else if (count == 2) {
            actions.UpdateButtons(ActionManager.State.Double);
        } else {
            actions.UpdateButtons(ActionManager.State.Many);
        }
    }

    // algorithm selection
    public void Bubble() {
        array.New();
        toggles.Bubble();
        alg = new BubbleSort(array.Size, array.Array);
        Setup();
    }

    public void Insertion() {
        array.New();
        toggles.Insertion();
        alg = new InsertionSort(array.Size, array.Array);
        Setup();
    }

    public void Quick() {
        array.New();
        toggles.Quick();
        alg = new QuickSort(array.Size, array.Array);
        Setup();
    }

    

    // actions
    public void Compare() {
        action = new CompareAction(selected[0].Index, selected[1].Index);
        if (alg.CorrectAction(action)) {
            state.SetCompare(array.Compare((CompareAction)action));
            DoAction();
        } else {
            Hint();
        }
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
        action = new MoveAction(selected[0].Index);
        partialAction = true;
        UpdateActions();
    }

    private void DoAction() {
        performingAction = true;
    }

    private void Hint() {
        GameAction a = alg.GetAction();
        switch (a.type) {
            case GameAction.GameActionType.Compare:
                CompareAction c = (CompareAction)a;
                array.Hint(c.index1);
                array.Hint(c.index2);
                break;
            case GameAction.GameActionType.Move:
                MoveAction m = (MoveAction)a;
                array.Hint(m.source);
                array.Hint(m.target);
                break;
            case GameAction.GameActionType.Pivot:
                PivotAction p = (PivotAction)a;
                array.Hint(p.pivotIndex);
                break;
            case GameAction.GameActionType.Store:
                StoreAction t = (StoreAction)a;
                array.Hint(t.index);
                break;
            case GameAction.GameActionType.Swap:
                SwapAction s = (SwapAction)a;
                array.Hint(s.index1);
                array.Hint(s.index2);
                break;
        }
        actions.Hint(a.type);
    }

    // menu

    public void Demo() {
        demo = !demo;
    }

    public void NewArray() {
        ClearSelections();
        UpdateActions();
        array.New();
        alg.Update(array.Array);
        ResetUI();
    }

    public void Setup() {
        ClearSelections();
        UpdateActions();
        alg.Restart();
        array.Restart();
        ResetUI();
    }

    private void ClearSelections() {
        foreach (SortingElement i in selected) {
            i.Selected = false;
        }
        selected.Clear();
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
}
