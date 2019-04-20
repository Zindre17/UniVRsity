using System.Collections.Generic;
using UnityEngine;

public class MergeSort : SortingAlgorithm {

    private class MergeState : IState {

        private readonly string state =
            "p = {0}\n" +
            "r = {1}\n" +
            "q = {2}\n" +
            "i = {3}\n" +
            "j = {4}\n" +
            "completed steps: {5}";

        public int p { get; private set; }
        public int r { get; private set; }
        public int q { get; private set; }
        public int i { get; private set; }
        public int j { get; private set; }
        public int steps { get; private set; }
        public int array { get; private set; }

        public MergeState(int _array, int _steps, int _p, int _r, int _q = -1, int _i = -1, int _j = -1) {
            array = _array;
            steps = _steps;
            p = _p;
            r = _r;
            q = _q;
            i = _i;
            j = _j; 
        }

        public string Readable() {
            return string.Format(state,
                p, r,
                q == -1 ? "" : q.ToString(),
                i == -1 ? "" : i.ToString(),
                j == -1 ? "" : j.ToString(),
                steps);
        }
    }

    public MergeSort(int _arrayLength, int[] _array) : base(_arrayLength, _array) {
        name = "Merge sort";

        pseudo = new string[] {
            "Mergesort(Array a, int p, int r)\n" +
            "   if p < r\n" +
            "       //find middle of array\n"+
            "       q = floor((p+r)/2)\n" +
            "       //perform mergesort on the each half of array\n"+
            "       Mergesort(a,p,q)\n" +
            "       Mergesort(a,q+1,r)\n" +
            "       Merge(A,p,q,r)",

            "Merge(Array a, int p, int q, int r)\n" +
            "n1 = q - p\n" +
            "n2 = r - q\n" +
            "let L and R be new arrays with lengths n1+1, and n2+1\n" +
            "L[0:n1-1] = a[p:q]\n"+
            "R[0:n2-1] = a[q:r]\n"+
            "L[n1] = infinity\n" +
            "R[n2] = infinity\n" +
            "i = j = 1\n" +
            "for k = p to r\n" +
            "   if L[i] <= R[j]\n" +
            "       a[k] = L[i]\n" +
            "       i = i + 1\n" +
            "   else\n" +
            "       a[k] = R[j]\n" +
            "       j = j + 1"
        };
    }

    internal override void GenerateActions() {
        focusChangePoints = new List<int>();
        _MergeSort(array, 0, size-1, currentArray, false);
    }

    private int cursor = 0;
    private List<int> focusChangePoints;

    private void CheckForFocusChange() {
        if (cursor >= focusChangePoints.Count) return;
        if(step == focusChangePoints[cursor]) {
            MergeState s = (MergeState)states[step];
            EventManager.FocusChanged(s.array, s.i, s.i);
            EventManager.FocusChanged(s.array + 1, s.j, s.j);
            cursor++;
        }
    }

    public override void Next() {
        base.Next();
        CheckForFocusChange();
    }

    private int currentArray = -1;

    private void _MergeSort(int[] a, int p, int r, int current, bool left) {
        if (p < r) {
            int q = Mathf.FloorToInt((p + r) / 2);
            states.Add(new MergeState(current, states.Count, p, r,_q:q));
            actions.Add(new SplitAction(current));
            int nextLeft = current + (left ? 2 : 1);
            int nextRight = current + (left ? 3 : 2);
            _MergeSort(a, p, q, nextLeft, true);
            _MergeSort(a, q+1, r, nextRight, false);
            states.Add(new MergeState(current,states.Count, p, r, _q:q));
            actions.Add(new MergeAction(nextLeft, nextRight));
            Merge(a, p, q, r, nextLeft, nextRight);
        }
    }

    private void Merge(int[] a, int p, int q, int r, int leftArray, int rightArray) {
        int n1 = q - p + 1;
        int n2 = r - q;
        int[] L = new int[n1 + 1];
        int[] R = new int[n2 + 1];
        int i;
        int j;
        for (i = 0; i < n1; i++) {
            L[i] = a[p + i];
        }
        for (j = 0; j < n2; j++) {
            R[j] = a[q +1+ j];
        }
        L[n1] = int.MaxValue;
        R[n2] = int.MaxValue;
        i = 0;
        j = 0;
        for (int k = p; k <= r; k++) {
            focusChangePoints.Add(states.Count);
            states.Add(new MergeState(leftArray, states.Count, p, r, _q: q, _i: i, _j: j));
            actions.Add(new CompareAction(leftArray, i, rightArray, j));
            if (L[i] <= R[j]) {
                states.Add(new MergeState(leftArray, states.Count, p, r, _q: q, _i: i, _j: j));
                actions.Add(new MoveAction(leftArray, i, -2));
                a[k] = L[i];
                i++;
            } else {
                states.Add(new MergeState(leftArray, states.Count, p, r, _q: q, _i: i, _j: j));
                actions.Add(new MoveAction(rightArray, j, -2));
                a[k] = R[j];
                j++;
            }
        }
    }
}
