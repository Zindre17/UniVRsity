
public class InsertionSort :SortingAlgorithm {

    private readonly string state =
        "i = {0} \n" +
        "j = {1} \n" +
        "key = {2} \n" +
        "completed steps: {3}";

    public InsertionSort(int _arrayLength, int[] _array):base(_arrayLength, _array) {
        name = "Insertion sort";

        pseudo = new string[] {
            "for j = 1 to A.length -1 \n" +
            "   key = A[j] \n" +
            "   i = j - 1 \n" +
            "   while i > -1 and A[i] > key \n" +
            "       A[i+1] = A[i] \n" +
            "       i = i - 1 \n" +
            "   A[i+1] = key"
        };
    }

    internal override void GenerateActions() {
        int i = -1;
        int key = -1;
        int j;
        for (j = 1; j < size; j++) {
            states.Add(string.Format(state, i, j, key, states.Count));
            key = array[j];
            actions.Add(new StoreAction(j));
            i = j - 1;
            while(i > -1 ) {
                states.Add(string.Format(state, i, j, key, states.Count));
                actions.Add(new CompareAction(-1, i));
                if (array[i] <= key)
                    break;
                states.Add(string.Format(state, i, j, key, states.Count));
                actions.Add(new MoveAction(i, i + 1));
                array[i + 1] = array[i];
                i--;
            }
            states.Add(string.Format(state, i, j, key, states.Count));
            actions.Add(new MoveAction(-1, i + 1));
            array[i + 1] = key;
        }
        states.Add(string.Format(state, i, j, key, states.Count));
    }
}
