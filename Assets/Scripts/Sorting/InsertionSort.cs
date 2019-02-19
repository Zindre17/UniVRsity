
using System.Collections.Generic;

public class InsertionSort :ISortingAlgorithm {

    private readonly string[] pseudo = new string[] {
        "for j = 1 to A.length -1 \n" +
        "   key = A[j] \n" +
        "   i = j - 1 \n" +
        "   while i > -1 and A[i] > key \n" +
        "       A[i+1] = A[i] \n" +
        "       i = i - 1 \n" +
        "   A[i+1] = key"
    };

    private readonly string state =
        "i = {0} \n" +
        "j = {1} \n" +
        "key = {2} \n" +
        "completed steps: {3}";

    private int steps = 0;

    private int arrayLength;
    private int[] arrayToSort;
    private bool complete = false;

    private struct State {
        public int i, j, key;
        public State(int _i, int _j, int _key) {
            i = _i;
            j = _j;
            key = _key;
        }
    }

    private List<GameAction> actions;
    private List<State> states;

    public InsertionSort(int _arrayLength, int[] array) {
        arrayLength = _arrayLength;
        arrayToSort = (int[])array.Clone();
        actions = new List<GameAction>();
        states = new List<State>();
        GenerateActions();
    }

    private void GenerateActions() {
        int[] ar = (int[])arrayToSort.Clone();
        int i = -1;
        int key = -1;
        int j;
        for (j = 1; j < arrayLength; j++) {
            states.Add(new State(i, j, key));
            key = arrayToSort[j];
            actions.Add(new StoreAction(j));
            i = j - 1;
            while(i > -1 ) {
                states.Add(new State(i, j, key));
                actions.Add(new CompareAction(-1, i));
                if (arrayToSort[i] < key)
                    break;
                states.Add(new State(i, j, key));
                actions.Add(new MoveAction(i, i + 1));
                arrayToSort[i + 1] = arrayToSort[i];
                i--;
            }
            states.Add(new State(i, j, key));
            actions.Add(new MoveAction(-1, i + 1));
            arrayToSort[i + 1] = key;
        }
        states.Add(new State(i, j, key));
    }

    public bool CorrectAction(GameAction action) {
        return actions[steps].EqualTo(action);
    }

    public string[] GetPseudo() {
        return pseudo;
    }

    public string GetState() {
        State s = states[steps];
        return s.key ==-1?string.Format(state, s.i, s.j, "empty", steps): string.Format(state, s.i, s.j, s.key, steps);
    }

    public void Next() {
        if (complete) return;
        steps++;
        if (steps == actions.Count) {
            EventManager.AlgorithmCompleted();
            complete = true;
        }
    }

    
    
    public void Prev() {
        throw new System.NotImplementedException();
    }

    public GameAction GetAction() {
        return actions[steps];
    }

    public GameAction GetAction(int step) {
        return actions[step];
    }

    public void Restart() {
        steps = 0;
        complete = false;
    }

    public string GetName() {
        return "Insertion sort";
    }
}
