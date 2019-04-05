
public class StoreAction : GameAction {

    public int index;

    public StoreAction(int _index) {
        type = GameActionType.Store;
        index = _index;
    }

    public override bool EqualTo(GameAction other) {
        if (other == null) return false;
        if (other.type == type) {
            StoreAction o = (StoreAction)other;
            return o.index == index;
        }
        return false;
    }
    
}
