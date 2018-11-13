
public abstract class GameAction {
    public enum GameActionType {
        Swap,
        Compare,
        Keep,
        Pivot,
        Store,
        Move
    }

    public GameActionType type;

    //public abstract override bool Equals(object obj);

}

