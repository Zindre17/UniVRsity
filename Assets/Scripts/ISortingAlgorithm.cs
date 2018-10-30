
public interface ISortingAlgorithm {

    int J { get; }
    int I { get; }
    void Next();
    void Prev();
    bool CorrectMove(Move move);
    int RequiredSelections();
    string GetPseudo();
    bool Complete();
}
