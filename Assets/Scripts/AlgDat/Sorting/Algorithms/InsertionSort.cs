
public class InsertionSort :SortingAlgorithm {

    private class InsertionState : IState {
        private readonly string state =
        "i = {0} \n" +
        "j = {1} \n" +
        "key = {2} \n" +
        "completed steps: {3}";

        public int i { get; private set; }
        public int j { get; private set; }
        public int key { get; private set; }
        public int steps { get; private set; }

        public InsertionState(int _steps,int _i, int _j, int _key) {
            i = _i;
            j = _j;
            key = _key;
            steps = _steps;
        }

        public string Readable() {
            return string.Format(state, i==-2?"":i.ToString(), j, key==-1?"":key.ToString(), steps);
        }
    }

    public override string GetStorageName()
    {
        return "key";
    }

    public InsertionSort(int _arrayLength, int[] _array):base(_arrayLength, _array) {
        name = "Insertion sort";

        pseudo = new string[] {
            "InsertionSort(Array A)\n" +
            "   for j = 1 to A.length -1 \n" +
            "       //store A[j]\n" +
            "       key = A[j] \n" +
            "       i = j - 1 \n" +
            "       //compare A[i] and storage(key)\n" +
            "       while i > -1 and A[i] > key \n" +
            "           //copy A[i] to A[i+1]\n" +
            "           A[i+1] = A[i] \n" +
            "           i = i - 1 \n" +
            "       //copy storage to A[i+1]\n" +
            "       A[i+1] = key"
        };
    }

    protected override void GenerateActions() {
        int i = -2;
        int key = -1;
        int j;
        for (j = 1; j < size; j++) {
            focusChangePoints.Add(states.Count);
            states.Add(new InsertionState(states.Count, i, j, key));
            key = array[j];
            actions.Add(new StoreAction(j));
            i = j - 1;
            while(i > -1 ) {
                states.Add(new InsertionState(states.Count, i, j, key));
                actions.Add(new CompareAction(-1, i));
                if (array[i] <= key)
                    break;
                states.Add(new InsertionState(states.Count, i, j, key));
                actions.Add(new MoveAction(-1,i, i + 1));
                array[i + 1] = array[i];
                i--;
            }
            states.Add(new InsertionState(states.Count, i, j, key));
            actions.Add(new MoveAction(-1,-1, i + 1));
            array[i + 1] = key;
        }
        //states.Add(string.Format(state, i, j, key, states.Count));
    }

    protected override void CheckForFocusChange(bool reverse = false)
    {
        if (reverse)
        {
            if (cursor == 0) return;
            if (focusChangePoints[cursor - 1] == step + 1)
            {
                cursor--;
                InsertionState s = (InsertionState)states[step];
                EventManager.FocusChanged(-1, 0, s.j);
                EventManager.FocusChanged(-1, 0, s.j);
            }
        }
        else
        {
            if (cursor >= focusChangePoints.Count) return;
            if (step == focusChangePoints[cursor])
            {
                InsertionState s = (InsertionState)states[step];
                EventManager.FocusChanged(-1, 0, s.j);
                EventManager.FocusChanged(-1, 0, s.j);
                cursor++;
            }
        }
    }
}
