using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBubbleSort : ISortingAlgorithm {

    private bool complete = false;
    private int i, j;
    private int arrayLength;
    private int[] arrayToSort;
    private int steps = 0;

    private readonly string pseudo =
        "for i = 0 to i = A.length - 2  \n" +
        "   for j = A.length - 1 down to i + 1 \n" +
        "       if A[j] < A[j-1]) \n" +
        "           exchange A[j] with A[j-1]"
        ;

    private string state =
        "i = {0} \n " +
        "j = {1} \n " +
        "steps completed: {2} \n";

    private GameAction.GameActionType expectedActionType;

    public NewBubbleSort(int _arrayLength, int[] array) {
        arrayLength = _arrayLength;
        i = 0;
        j = arrayLength - 1;
        expectedActionType = GameAction.GameActionType.Compare;
        arrayToSort = (int[])array.Clone();
    }

    public event Complete OnComplete;

    public bool Complete() {
        return complete;
    }

    public bool CorrectAction(GameAction action) {
        if (action.type == expectedActionType) {
            if (action.type == GameAction.GameActionType.Compare) {
                CompareAction a = (CompareAction)action;
                return (a.index1 == j && a.index2 == j - 1) || (a.index1 == j - 1 && a.index2 == j);
            } else if (action.type == GameAction.GameActionType.Swap) {
                SwapAction a = (SwapAction)action;
                return (a.index1 == j && a.index2 == j - 1) || (a.index1 == j - 1 && a.index2 == j);
            } else if (action.type == GameAction.GameActionType.Keep) {
                KeepAction a = (KeepAction)action;
                return (a.index1 == j && a.index2 == j - 1) || (a.index1 == j - 1 && a.index2 == j);
            } else return false;
        } else return false;
    }

    public string GetPseudo() {
        return pseudo;
    }

    public string GetState() {
        return string.Format(state, i, j, steps);
    }

    public void Next() {
        if (expectedActionType == GameAction.GameActionType.Swap)
            Swap(j, j - 1);
        if (!complete) {
            if(expectedActionType == GameAction.GameActionType.Compare) {
                if(arrayToSort[j-1] > arrayToSort[j]) {
                    expectedActionType = GameAction.GameActionType.Swap;
                } else {
                    expectedActionType = GameAction.GameActionType.Keep;
                }
            } else {
                expectedActionType = GameAction.GameActionType.Compare;
                if (j > i + 1) {
                    j--;
                } else {
                    if( i < arrayLength - 1) {
                        i++;
                        j = arrayLength - 1;
                    }
                    if(i == arrayLength - 1) {
                        complete = true;
                        if (OnComplete != null) OnComplete();
                    }
                }
            }
            steps++;
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
}
