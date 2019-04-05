using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public ActionButton compare, swap, pivot, store, copy;

    public enum State {
        Busy,
        None,
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
                UpdateButtons(false, false, false, false, false);
                break;
            case State.Busy:
                UpdateButtons(false, false, false, false, false);
                break;
            case State.Single:
                UpdateButtons(false, true, true, true, true);
                break;
            case State.Double:
                UpdateButtons(true, true, false, false, false);
                break;
            case State.Many:
                UpdateButtons(false, false, false, false, false);
                break;
        }
    }

    public void PartialActionComplete() {
        copy.InProgress = false;
    }

    private void UpdateButtons(bool com, bool sw, bool p, bool st, bool cop) {
        compare.Active = com;
        swap.Active = sw;
        pivot.Active = p;
        store.Active = st;
        copy.Active = cop;
    }
}
