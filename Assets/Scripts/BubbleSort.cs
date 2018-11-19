using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSort : ISortingAlgorithm {

    private bool complete = false;
    private int arrayLength;
    private int[] arrayToSort;
    private int steps = 0;

    private struct State {
        public int i;
        public int j;
        public State(int _i, int _j) {
            i = _i;
            j = _j;
        }
    }

    private readonly string pseudo =
        "for i = 0 to i = A.length - 2  \n" +
        "   for j = A.length - 1 down to i + 1 \n" +
        "       if A[j] < A[j-1]) \n" +
        "           exchange A[j] with A[j-1]"
        ;

    private readonly string state =
        "i = {0} \n " +
        "j = {1} \n " +
        "steps completed: {2} \n";

    private List<GameAction> actions;
    private List<State> states;

    public BubbleSort(int _arrayLength, int[] array) {
        arrayLength = _arrayLength;
        arrayToSort = (int[])array.Clone();
        actions = new List<GameAction>();
        states = new List<State>();
        GenerateActions();
    }


    private void GenerateActions() {
        int[] ar = (int[])arrayToSort.Clone();
        GameAction a;
        int i = 0;
        int j = arrayLength - 1;
        for(i = 0; i < arrayLength-1; i++) {
            for(j = arrayLength - 1; j > i; j--) {
                states.Add(new State(i, j));
                a = new CompareAction(j, j-1);
                actions.Add(a);
                if(arrayToSort[j] < arrayToSort[j - 1]) {
                    states.Add(new State(i, j));
                    a = new SwapAction(j, j - 1);
                    actions.Add(a);
                    int tmp = arrayToSort[j];
                    arrayToSort[j] = arrayToSort[j - 1];
                    arrayToSort[j - 1] = tmp;
                }
            }
        }
        i = arrayLength - 2;
        j = arrayLength - 1;
        states.Add(new State(i, j));
        arrayToSort = ar;
    }
    public event Complete OnComplete;

    public bool Complete() {
        return complete;
    }

    public bool CorrectAction(GameAction action) {
        return actions[steps].EqualTo(action);
    }

    public string GetPseudo() {
        return pseudo;
    }

    public string GetState() {
        State s = states[steps];
        return string.Format(state, s.i, s.j, steps);
    }

    public void Next() {
        if (complete) return;
        steps++;
        if (steps == actions.Count) {
            complete = true;
            EventManager.AlgorithmCompleted();
        }
    }

    private void Swap(int i1, int i2) {
        int temp = arrayToSort[i1];
        arrayToSort[i1] = arrayToSort[i2];
        arrayToSort[i2] = temp;
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

}
