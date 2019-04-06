public class SplitAction : GameAction {
    public int array;

    public SplitAction(int index) {
        array = index;
        type = GameAction.GameActionType.Split;
    }
    public override bool EqualTo(GameAction other) {
        if (other == null) return false;
        if(other.type == type) {
            SplitAction a = (SplitAction)other;
            return a.array == array;
        }
        return false;
    }
}