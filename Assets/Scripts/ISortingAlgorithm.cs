public delegate void Complete();
public interface ISortingAlgorithm {

    void Next();
    void Prev();
    string GetPseudo();
    string GetState();
    event Complete OnComplete;
    bool CorrectAction(GameAction action);
}
