public delegate void Complete();
public interface ISortingAlgorithm {

    void Next();
    void Prev();
    void Restart();
    string[] GetPseudo();
    string GetState();
    string GetName();
    bool CorrectAction(GameAction action);
    GameAction GetAction();
    GameAction GetAction(int step);
}
