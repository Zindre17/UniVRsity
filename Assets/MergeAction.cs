public class MergeAction : GameAction {
    public int a1, a2;
    public MergeAction(int index1, int index2) {
        a1 = index1;
        a2 = index2;
        type = GameActionType.Merge;
    }
    public override bool EqualTo(GameAction other) {
        if (other == null) return false;
        if(other.type == type) {
            MergeAction o = (MergeAction)other;
            return (a1 == o.a1 && a2 == o.a2) || (a1 == o.a2 && a2 == o.a1);
        }
        return false;
    }
}