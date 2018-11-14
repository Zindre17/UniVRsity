
public class StoreAction : GameAction {

    public int index;

    public StoreAction(int _index) {
        type = GameActionType.Store;
        index = _index;
    }
}
