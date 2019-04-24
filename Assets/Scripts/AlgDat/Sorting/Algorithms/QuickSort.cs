
using System.Collections.Generic;

public class QuickSort : SortingAlgorithm {

    private class QuickState : IState {
        private readonly string state =
        "p = {0} \n" +
        "r = {1} \n" +
        "i = {2} \n" +
        "j = {3} \n" +
        "x (pivot) = {4} \n"+
        "steps completed: {5}";
        public int p { get; private set; }
        public int r { get; private set; }
        public int i { get; private set; }
        public int j { get; private set; }
        public int x { get; private set; }
        public int steps { get; private set; }

        public QuickState(int _steps, int _p , int _r , int _i = -2, int _j = -2, int _x = -2) {
            steps = _steps;
            p = _p;
            r = _r;
            i = _i;
            j = _j;
            x = _x;
        }

        public string Readable() {
            return string.Format(state, p, r, i == -2? "":i.ToString(), j==-2?"":j.ToString(), x==-2?"":"A[" + x + "]", steps);
        }
    }
    

    private List<int> pivots = new List<int>();

    public QuickSort(int _arrayLength, int[] _array):base(_arrayLength, _array) {
        name = "Quick sort";

        pseudo = new string[] {
            "Qucksort(Array a, int p, int r) \n" +
            "   if p < r \n" +
            "       q = Partition(a, p, r) \n" +
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
        CheckForFocusChange();
    }

    public override void Next() {
        base.Next();
        CheckForFocusChange();
    }

    internal override void GenerateActions() {
        int[] ar = (int[])array.Clone();
        PerformQuickSort(ar, 0, ar.Length - 1);
    }

    private void PerformQuickSort(int[] a, int p, int r) {
        if (p < r) {
            int q = Partition(a, p, r);
            PerformQuickSort(a, p, q - 1);
            PerformQuickSort(a, q + 1, r);
        }
    }

    private int Partition(int[] a, int p, int r) {
        if (states.Count > 0)
            states.Add(new QuickState(states.Count,p, r, _x: pivots[pivots.Count-1]));
        else
            states.Add(new QuickState(states.Count,p, r));
        actions.Add(new PivotAction(r));
        pivots.Add(r);
        int x = a[r];
        int i = p - 1;
        for (int j = p; j < r; j++) {
            states.Add(new QuickState(states.Count,p, r, i, j, r));
            actions.Add(new CompareAction(j, r));
            if(a[j] <= x) {
                i++;
                states.Add(new QuickState(states.Count,p,r,i,j,r));
                actions.Add(new SwapAction(i, j));
                int temp = a[i];
                a[i] = a[j];
                a[j] = temp;
            }
        }
        states.Add(new QuickState(states.Count, p, r, i, -2, r));
        actions.Add(new SwapAction(i + 1, r));
        int tmp = a[i + 1];
        a[i + 1] = a[r];
        a[r] = tmp;
        pivots[pivots.Count - 1] = i + 1;
        return i+1;
    }

    private void CheckForFocusChange() {
        if (complete) return;
        if (GetAction().type == GameAction.GameActionType.Pivot) {
            QuickState s = (QuickState)states[step];
            EventManager.FocusChanged(-1,s.p, s.r);
        }
    }

    public override void Prev() {
        base.Prev();
        if (step < 2 || step > actions.Count -2) return;
        if(GetAction(step+1).type == GameAction.GameActionType.Pivot) {
            QuickState s = (QuickState)states[step-2];
            EventManager.FocusChanged(-1, s.p, s.r);
        }
    }

}
