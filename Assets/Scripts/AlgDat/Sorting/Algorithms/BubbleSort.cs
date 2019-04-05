﻿
public class BubbleSort : SortingAlgorithm {

    private readonly string stateFormat =
        "i = {0} \n" +
        "j = {1} \n" +
        "steps completed: {2} \n";

    public BubbleSort(int arraySize, int[] _array) : base(arraySize, _array) {
        //name
        name = "Bubble sort";
        //pseudo code
        pseudo = new string[1] {
        "for i = 0 to i = A.length - 2  \n" +
        "   for j = A.length - 1 down to i + 1 \n" +
        "       if A[j] < A[j-1]) \n" +
        "           exchange A[j] with A[j-1]"
        };
    }

    internal override void GenerateActions() {
        int[] ar = (int[])array.Clone();
        GameAction a;
        int i = 0;
        int j = size - 1;
        for(i = 0; i < size-1; i++) {
            for(j = size - 1; j > i; j--) {
                states.Add(string.Format(stateFormat, i, j, states.Count));
                a = new CompareAction(j, j-1);
                actions.Add(a);
                if(array[j] < array[j - 1]) {
                    states.Add(string.Format(stateFormat, i, j, states.Count));
                    a = new SwapAction(j, j - 1);
                    actions.Add(a);
                    int tmp = array[j];
                    array[j] = array[j - 1];
                    array[j - 1] = tmp;
                }
            }
        }
        i = size - 2;
        j = size - 1;
        states.Add(string.Format(stateFormat, i, j, states.Count));
        array = ar;
    }

    private void Swap(int i1, int i2) {
        int temp = array[i1];
        array[i1] = array[i2];
        array[i2] = temp;
    }
   
}