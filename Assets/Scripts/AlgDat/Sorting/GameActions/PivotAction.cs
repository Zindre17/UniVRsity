
public class PivotAction : GameAction {

    public int pivotIndex;

    public PivotAction(int _index) {
        type = GameActionType.Pivot;
        pivotIndex = _index;
    }

    public override bool EqualTo(GameAction other) {
        if (other == null) return false;
        if (other.type == type) {
            PivotAction o = (PivotAction)other;
            return pivotIndex == o.pivotIndex;
        }
        return false;
    }
}
