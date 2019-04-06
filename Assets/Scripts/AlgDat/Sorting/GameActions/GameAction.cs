
public abstract class GameAction {
    public enum GameActionType {
        Swap,
        Compare,
        Keep,
        Pivot,
        Store,
        Move,
        Merge, 
        Split
    }

    public GameActionType type;

    public abstract bool EqualTo(GameAction other);

    public static bool IsMultiStep(GameActionType type) {
        return type == GameActionType.Move;
    }
}

