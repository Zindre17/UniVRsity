
using System.Collections.Generic;

public class QuickSort : SortingAlgorithm {

    private readonly string state =
        "p = {0} \n" +
        "r = {1} \n" +
        "i = {2} \n" +
        "j = {3} \n" +
        "x (pivot) = {4}";

    private int focusPos = -1;
    private List<int> focusPoints;
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
        focusPoints = new List<int>();
        //CheckForFocusChange();
    }

    internal override void GenerateActions() {
        int[] ar = (int[])array.Clone();
        PerformQuickSort(ar, 0, ar.Length - 1);
    }

    private void PerformQuickSort(int[] a, int p, int r) {
        if (p < r) {
            //focusPoints.Add(p);
            //focusPoints.Add(r);
            int q = Partition(a, p, r);
            PerformQuickSort(a, p, q - 1);
            PerformQuickSort(a, q + 1, r);
        }
    }

    private string GetStateString(int p, int r, int i, int j, int x) {
        return string.Format(state, p, r, i == -2 ? "" : i.ToString(), j == -2 ? "" : j.ToString(), x == -2 ? "" : "A[" + x + "]");
    }

    private int Partition(int[] a, int p, int r) {
        if (states.Count > 0)
            states.Add(GetStateString(p, r, -2, -2, pivots[pivots.Count-1]));
        else
            states.Add(GetStateString(p, r, -2, -2, -2));
        actions.Add(new PivotAction(r));
        pivots.Add(r);
        int x = a[r];
        int i = p - 1;
        for (int j = p; j < r; j++) {
            states.Add(GetStateString(p, r, i, j, r));
            actions.Add(new CompareAction(j, r));
            if(a[j] <= x) {
                i++;
                states.Add(GetStateString(p,r,i,j,r));
                actions.Add(new SwapAction(i, j));
                int temp = a[i];
                a[i] = a[j];
                a[j] = temp;
            }
        }
        states.Add(GetStateString(p, r, i, -2, r));
        actions.Add(new SwapAction(i + 1, r));
        int tmp = a[i + 1];
        a[i + 1] = a[r];
        a[r] = tmp;
        pivots[pivots.Count - 1] = i + 1;
        return i+1;
    }

    private void CheckForFocusChange() {
        if (GetAction().type == GameAction.GameActionType.Pivot) {
            focusPos++;
            EventManager.FocusChanged(focusPoints[focusPos * 2], focusPoints[focusPos * 2 + 1]);
        }
    }

}
