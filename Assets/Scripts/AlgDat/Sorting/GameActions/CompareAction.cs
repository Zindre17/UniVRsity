
public class CompareAction : GameAction {

    public int index1, index2;
    public int array1 , array2 ;

    public CompareAction(int _array1, int _index1, int _array2, int _index2) {
        type = GameActionType.Compare;
        array1 = _array1;
        index1 = _index1;
        array2 = _array2;
        index2 = _index2;
    }

    public CompareAction(int _index1, int _index2) {
        type = GameActionType.Compare;
        index1 = _index1;
        index2 = _index2;
        array1 = -2;
        array2 = -2;
    }

    public override bool EqualTo(GameAction other) {
        if (other == null) return false;
        if (other.type == type) {
            CompareAction o = (CompareAction)other;
            return (o.index1== index1 && o.index2 == index2) || (o.index1 == index2 && o.index2 == index1);
        }
        return false;
    }
}
