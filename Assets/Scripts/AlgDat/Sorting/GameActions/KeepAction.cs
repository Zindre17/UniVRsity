
public class KeepAction : GameAction {

    public int index1, index2;

    public KeepAction(int _index1, int _index2) {
        type = GameActionType.Keep;
        index1 = _index1;
        index2 = _index2;
    }

    public override bool EqualTo(GameAction other) {
        if (other == null) return false;
        if (other.type == type) {
            SwapAction o = (SwapAction)other;
            return (o.index1 == index1 && o.index2 == index2) || (o.index1 == index2 && o.index2 == index1);
        }
        return false;
    }
}
