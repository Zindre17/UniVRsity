public delegate void Complete();
public interface ISortingAlgorithm {

    void Next();
    void Prev();
    string GetPseudo();
    string GetState();
    bool CorrectAction(GameAction action);
    GameAction GetAction();
    GameAction GetAction(int step);
}
