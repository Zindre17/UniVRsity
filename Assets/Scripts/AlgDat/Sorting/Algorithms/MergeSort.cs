using System.Collections.Generic;
using UnityEngine;

public class MergeSort : SortingAlgorithm {

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
        //actions.Add(new SplitAction(-1));
        //actions.Add(new SplitAction(0));
        _MergeSort(array, 0, size, currentArray, false);
    }

    private int currentArray = -1;
    private List<int> splits = new List<int>();

    private void _MergeSort(int[] a, int p, int r, int current, bool left) {
        if (p < r) {
            int q = Mathf.FloorToInt((p + r) / 2);
            actions.Add(new SplitAction(current));
            int nextLeft = current + (left ? 2 : 1);
            int nextRight = current + (left ? 3 : 2);
            _MergeSort(a, p, q, nextLeft, true);
            _MergeSort(a, q+1, r, nextRight, false);
            actions.Add(new MergeAction(nextLeft, nextRight));
            Merge(a, p, q, r, nextLeft, nextRight);
        }
    }

    private void Merge(int[] a, int p, int q, int r, int leftArray, int rightArray) {
        int n1 = q - p + 1;
        int n2 = r - q;
        int[] L = new int[n1 + 1];
        int[] R = new int[n2 + 1];
        int i, j;
        for (i = 0; i < n1; i++)
            L[i] = a[p + i];
        for (j = 0; j < n2; j++)
            R[j] = a[q + j];
        L[n1] = int.MaxValue;
        R[n2] = int.MaxValue;
        i = 0;
        j = 0;
        for (int k = p; k < r; k++) {
            actions.Add(new CompareAction(leftArray, i, rightArray, j));
            if (L[i] <= R[j]) {
                actions.Add(new MoveAction(leftArray, i, -2));
                a[k] = L[i];
                i++;
            } else {
                actions.Add(new MoveAction(rightArray, j, -2));
                a[k] = R[j];
                j++;
            }
        }
    }
}
