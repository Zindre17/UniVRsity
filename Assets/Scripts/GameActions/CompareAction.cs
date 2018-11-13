
public class CompareAction : GameAction {

    public int index1, index2;
    public bool store;

    public CompareAction(int _index1, int _index2) {
        type = GameActionType.Compare;
        index1 = _index1;
        index2 = _index2;
        store = false;
    }
    public CompareAction(int _index1) {
        type = GameActionType.Compare;
        index1 = _index1;
        store = true;
    }
}
