
public class MoveAction : GameAction {
    public int source, target;

    public MoveAction(int _source, int _target) {
        target = _target;
        source = _source;
        type = GameActionType.Move;
    }

    public override bool EqualTo(GameAction other) {
        if (other == null) return false;
        if(other.type == type) {
            MoveAction o = (MoveAction)other;
            return o.target == target && o.source == source;
        }
        return false;
    }
}
