using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeSort : ISortingAlgorithm {

    private readonly string[] pseudo = new string[] {
        "Mergesort(Array a, int p, int r)\n" +
        "   if p < r\n" +
        "       q = floor((p+r)/2)\n" +
        "       Mergesort(a,p,q)\n" +
        "       Mergesort(a,q+1,r)\n" +
        "       Merge(A,p,q,r)",
        "Merge(Array a, int p, int q, int r)\n" +
        "n1 = q - p + 1\n" +
        "n2 = r - q\n" +
        "let L and R be new arrays with lengths n1+1, and n2+1\n" +
        "for i = 1 to n1\n" +
        "   L[i] = A[p + i - 1]\n" +
        "for j = 1 to n2\n" +
        "   R[j] = A[q + j]\n" +
        "L[n1 + 1] = infinity\n" +
        "R[n2 + 1] = infinity\n" +
        "i = 1\n" +
        "j = 1\n" +
        "for k = p to r\n" +
        "   if L[i] <= R[j]\n" +
        "       A[k] = L[i]\n" +
        "       i = i + 1\n" +
        "   else\n" +
        "       A[k] = R[j]\n" +
        "       j = j + 1"
    };

    private int steps = 0;
    private int arrayLength;
    private int[] arrayToSort;
    private bool complete = false;

    private List<GameAction> actions;
    private List<State> states;

    private struct State {

    }

    public MergeSort(int _arrayLength, int[] array) {
        arrayLength = _arrayLength;
        arrayToSort = (int[])array.Clone();
        actions = new List<GameAction>();
        states = new List<State>();
        GenerateActions();
    }

    private void GenerateActions() {

    }

    private void _MergeSort(int[] a, int p, int r) {
        if (p < r) {
            int q = Mathf.FloorToInt((p + r) / 2);
            _MergeSort(a, p, q);
            _MergeSort(a, q + 1, r);
            Merge(a, p, q, r);
        }
    }

    private void Merge(int[] a, int p, int q, int r) {
        int n1 = q - p + 1;
        int n2 = r - q;
        int[] L = new int[n1 + 1];
        int[] R = new int[n2 + 1];
        int i, j;
        for(i = 0; i < n1; i++) 
            L[i] = a[p + i - 1];
        for (j = 0; j < n2; j++)
            R[j] = a[q + j];
        L[n1] = int.MaxValue;
        R[n2] = int.MaxValue;
        i = j = 0;
        for(int k = p; k <= r; k++) {
            if(L[i] <= R[j]) {
                a[k] = L[i];
                i++;
            } else {
                a[k] = R[j];
                j++;
            }
        }
    }

    public bool CorrectAction(GameAction action) {
        return action.EqualTo(GetAction());
    }

    public GameAction GetAction() {
        return actions[steps];
    }

    public GameAction GetAction(int step) {
        if (step < actions.Count)
            return actions[step];
        else return null;
    }

    public string GetName() {
        return "Mergesort";
    }

    public string[] GetPseudo() {
        return pseudo;
    }

    public string GetState() {
        throw new System.NotImplementedException();
    }

    public void Next() {
        throw new System.NotImplementedException();
    }

    public void Prev() {
        throw new System.NotImplementedException();
    }

    public void Restart() {
        throw new System.NotImplementedException();
    }

    public void Update(int[] newArray) {
        Restart();
        arrayToSort = (int[])newArray.Clone();
        GenerateActions();
    }
}
