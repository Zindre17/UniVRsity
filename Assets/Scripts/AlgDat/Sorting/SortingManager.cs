using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingManager : MonoBehaviour
{
    public ArrayManager array;
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

    private List<SortingElement> selected;

    private void Awake() {
        selected = new List<SortingElement>();
        EventManager.OnSelect += Select;
        EventManager.OnActionCompleted += ActionComplete;
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
        EventManager.OnSelect -= Select;
        EventManager.OnActionCompleted -= ActionComplete;
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

    // algorithm selection
    public void Bubble() {
        array.New();
        //toggles.Bubble();
        alg = new BubbleSort(array.Size, array.Array);
        Setup();
    }

    public void Insertion() {
        array.New();
        //toggles.Insertion();
        alg = new InsertionSort(array.Size, array.Array);
        Setup();
    }

    public void Quick() {
        array.New();
        //toggles.Quick();
        alg = new QuickSort(array.Size, array.Array);
        Setup();
    }

    

    // actions
    public void Compare() {
        action = new CompareAction(selected[0].Index, selected[1].Index);
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
        action = new MoveAction(selected[0].Index);
        partialAction = true;
        UpdateActions();
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
        array.New();
        alg.Update(array.Array);
        Setup();
    }

    public void Restart() {
        alg.Restart();
        array.Restart();
        Setup();
    }

    // Helper functions

    private void Setup() {
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
            actions.UpdateButtons(ActionManager.State.Single);
        } else if (count == 2) {
            actions.UpdateButtons(ActionManager.State.Double);
        } else {
            actions.UpdateButtons(ActionManager.State.Many);
        }
    }

    private void DoAction() {
        if (alg.CorrectAction(action)) {
            performingAction = true;
            switch (action.type) {
                case GameAction.GameActionType.Compare:
                    state.SetCompare(array.Compare((CompareAction)action));
                    break;
                case GameAction.GameActionType.Swap:
                    ClearSelections();
                    array.Swap((SwapAction)action);
                    break;
                case GameAction.GameActionType.Store:
                    ClearSelections();
                    array.Store((StoreAction)action);
                    break;
                case GameAction.GameActionType.Pivot:
                    ClearSelections();
                    array.Pivot((PivotAction)action);
                    break;
                case GameAction.GameActionType.Move:
                    ClearSelections();
                    array.CopyTo((MoveAction)action);
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
    }

    private void DoStep() {
        action = alg.GetAction();
        if (action != null)
            DoAction();
        else
            Demo();
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

    private void UpdateMenu() {
        if (alg == null)
            menu.UpdateMenu(MenuManager.State.Idle);
        else
            menu.UpdateMenu(MenuManager.State.Started);
    }
}
