using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSort :ISortingAlgorithm {

    private int i = 0;
    public int I { get { return i; } }

    private int j = 0;
    public int J { get { return j; } }

    private int neededSelections = 2;
    private int arrayLength;

    private bool complete = false;

    private Stack<int> prevIs;
    private Stack<int> prevJs;

    public BubbleSort(int _arrayLength) {
        arrayLength = _arrayLength;

        prevIs = new Stack<int>();
        prevJs = new Stack<int>();
        
        i = 0;
        j = arrayLength - 1;
        
    }

    public void Next() {
        if (!complete) {
            prevIs.Push(i);
            prevJs.Push(i);
            if(j > i + 1) {
                j--;
            } else {
                if(i < arrayLength-1) {
                    i++;
                    j = arrayLength-1;
                }
            }
        }
    }

    public void Prev() {
        if(prevIs.Count > 0) {
            i = prevIs.Pop();
            j = prevJs.Pop();
        }
    }

    public bool CorrectMove(Move move) {
        if(move.GetSelectionCount() < 2) 
            return false;
        int m1 = move.GetFirstSelection().Index;
        int m2 = move.GetSecondSelection().Index;
        return (m1 == j && m2 == j - 1) || (m1 == j - 1 && m2 == j);
    }

    public int RequiredSelections() {
        return neededSelections;
    }

    public string GetPseudo() {
        return "for i = 0 to A.length -2 \n   for j = A.length downto i+1 \n       if A[j] < A[j-1] \n           exchange A[j] with A[j-1]";
    }

    private readonly string[] pseudo = {
        "for i = 0 to A.length -2",
        "   for j = A.length-1 downto i+1",
        "       if A[j] < A[j-1]",
        "           exchange A[j] with A[j-1]"
    };

}
