using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSort : ISortingAlgorithm {

    private readonly string[] pseudo = new string[] {
        "Qucksort(Array a, int p, int r) \n" +
        "   if p < r \n" +
        "       pivot = Partition(a, p, r) \n" +
        "       Quicksort(a, p, q-1) \n" +
        "       Quicksort(a, q+1, r) \n",
        "Partition(Array a, int p, int r) \n" +
        "   x = a[r] \n" +
        "   i = p - 1 \n" +
        "   for j = p to r - 1\n" +
        "       if a[j] <= x\n" +
        "           i = i + 1\n" +
        "           swap a[j] with a[i]\n" +
        "   swap a[i+1] with a[r]\n" +
        "   return i+1"
    };

    private readonly string state =
        "p = {0} \n" +
        "r = {1} \n" +
        "i = {2} \n" +
        "j = {3} \n" +
        "x (pivot) = {4}";

    private int steps = 0;

    private int arrayLength;
    private int[] arrayToSort;
    private bool complete = false;

    private int focusPos = -1;
    private List<int> focusPoints = new List<int>();

    private struct State {
        public int p, r, i, j, x;
        public State(int _p, int _r, int _i, int _j, int _x) {
            p = _p; r = _r; i = _i; j = _j; x = _x;
        }
    }

    private List<GameAction> actions;
    private List<State> states;

    public QuickSort(int _arrayLength, int[] array) {
        arrayLength = _arrayLength;
        arrayToSort = (int[])array.Clone();
        actions = new List<GameAction>();
        states = new List<State>();
        GenerateActions();
        CheckForFocusChange();
    }

    private void GenerateActions() {
        int[] ar = (int[])arrayToSort.Clone();
        PerformQuickSort(ar, 0, ar.Length - 1);
    }

    private void PerformQuickSort(int[] a, int p, int r) {
        if (p < r) {
            focusPoints.Add(p);
            focusPoints.Add(r);
            int q = Partition(a, p, r);
            PerformQuickSort(a, p, q - 1);
            PerformQuickSort(a, q + 1, r);
        }
    }

    private int Partition(int[] a, int p, int r) {
        if (states.Count > 0)
            states.Add(new State(p, r, -2, -2, states[states.Count-1].x));
        else
            states.Add(new State(p, r, -2, -2, -2));
        actions.Add(new PivotAction(r));
        int x = a[r];
        int i = p - 1;
        for (int j = p; j < r; j++) {
            states.Add(new State(p, r, i, j, x));
            actions.Add(new CompareAction(j, r));
            if(a[j] <= x) {
                i++;
                states.Add(new State(p, r, i, j, x));
                actions.Add(new SwapAction(i, j));
                int temp = a[i];
                a[i] = a[j];
                a[j] = temp;
            }
        }
        states.Add(new State(p, r, i, -2, x));
        actions.Add(new SwapAction(i + 1, r));
        int tmp = a[i + 1];
        a[i + 1] = a[r];
        a[r] = tmp;
        return i+1;
    }

    public bool CorrectAction(GameAction action) {
        return actions[steps].EqualTo(action);
    }

    public GameAction GetAction() {
        return actions[steps];
    }

    public GameAction GetAction(int step) {
        return actions[step];
    }

    public string GetName() {
        return "Quick sort";
    }

    public string[] GetPseudo() {
        return pseudo;
    }

    public string GetState() {
        State s = states[steps];
        if(s.x == -2) {
            return string.Format(state, s.p, s.r, "empty" ,"empty", "empty");
        }else if(s.i == -2) {
            return string.Format(state, s.p, s.r, "empty", "empty", s.x);
        }else if(s.j == -2) {
            return string.Format(state, s.p, s.r, s.i, "empty", s.x);
        } else {
            return string.Format(state, s.p, s.r, s.i, s.j, s.x);
        }
    }

    private void CheckForFocusChange() {
        if (GetAction().type == GameAction.GameActionType.Pivot) {
            focusPos++;
            EventManager.FocusChanged(focusPoints[focusPos * 2], focusPoints[focusPos * 2 + 1]);
        }
    }

    public void Next() {
        if (complete) return;
        steps++;
        if (steps == actions.Count) {
            EventManager.AlgorithmCompleted();
            complete = true;
            EventManager.FocusChanged(0, arrayLength-1);
            return;
        }
        CheckForFocusChange();
    }

    public void Prev() {
        throw new System.NotImplementedException();
    }

    public void Restart() {
        steps = 0;
        complete = false;
    }

    public void Update(int[] newArray) {
        Restart();
        arrayToSort = (int[])newArray.Clone();
        GenerateActions();
    }
}
