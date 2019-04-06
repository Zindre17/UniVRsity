using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public ActionButton compare, swap, pivot, store, copy, merge, split;

    public enum State {
        Busy,
        None,
        Array,
        Arrays,
        ManyA,
        Single,
        Double,
        Many
    }

    public void Hint(GameAction.GameActionType type) {
        switch (type) {
            case GameAction.GameActionType.Compare:
                compare.Hint();
                break;
            case GameAction.GameActionType.Move:
                copy.Hint();
                break;
            case GameAction.GameActionType.Pivot:
                pivot.Hint();
                break;
            case GameAction.GameActionType.Store:
                store.Hint();
                break;
            case GameAction.GameActionType.Swap:
                swap.Hint();
                break;
        }
    }

    public void UpdateButtons(State state) {
        switch (state) {
            case State.None:
                UpdateButtons();
                break;
            case State.Busy:
                UpdateButtons();
                break;
            case State.Single:
                UpdateButtons(sw: true, p: true, cop: true, st: true);
                break;
            case State.Double:
                UpdateButtons(com: true, sw: true);
                break;
            case State.Many:
                UpdateButtons();
                break;
            case State.Array:
                UpdateButtons(sp: true);
                break;
            case State.Arrays:
                UpdateButtons(m: true);
                break;
            case State.ManyA:
                UpdateButtons();
                break;
        }
    }

    public void PartialActionComplete() {
        copy.InProgress = false;
    }

    private void UpdateButtons(bool com = false, bool sw=false, bool p=false, bool st=false, bool cop=false, bool m = false, bool sp = false) {
        compare.Active = com;
        swap.Active = sw;
        pivot.Active = p;
        store.Active = st;
        copy.Active = cop;
        split.Active = sp;
        merge.Active = m;
    }
}
