
public class InsertionSort :ISortingAlgorithm {

    private readonly string pseudo =
        "for j = 1 to A.length -1 \n" +
        "   key = A[j] \n" +
        "   i = j - 1 \n" +
        "   while i > -1 and A[i] > key \n" +
        "       A[i+1] = A[i] \n" +
        "       i = i - 1 \n" +
        "   A[i+1] = key";

    private readonly string state =
        "j = {0} \n" +
        "i = {1} \n" +
        "key = {2} \n" +
        "completed steps: {3}";

    private int i, j, key;

    private int steps;

    private int arrayLength;
    private int[] arrayToSort;

    private GameAction nextAction;

    public InsertionSort(int _arrayLength, int[] array) {
        arrayLength = _arrayLength;
        arrayToSort = (int[])array.Clone();
        j = 1;
        i = 0;
        key = -1;
        steps = 0;
        nextAction = new StoreAction(j);
    }

    public event Complete OnComplete;

    public bool CorrectAction(GameAction action) {
        if (action.type == nextAction.type) {
            if (action.type == GameAction.GameActionType.Store) {
                StoreAction a = (StoreAction)action;
                StoreAction na = (StoreAction)nextAction;
                return a.index == na.index;
            } else if (action.type == GameAction.GameActionType.Compare) {
                CompareAction a = (CompareAction)action;
                CompareAction na = (CompareAction)nextAction;
                return a.index1 == na.index1 && a.store == na.store && a.index2 == na.index2;
            } else if (action.type == GameAction.GameActionType.Move) {
                MoveAction a = (MoveAction)action;
                MoveAction na = (MoveAction)nextAction;
                return a.source == na.source && a.target == na.target && a.store == na.store;
            }      
        } return false;
    }

    public string GetPseudo() {
        return pseudo;
    }

    public string GetState() {
        return key == -1?string.Format(state, i, j, "empty"): string.Format(state, i, j, key);
    }

    public void Next() {
        steps++;
        if (nextAction.type == GameAction.GameActionType.Store) {
            key = arrayToSort[j];
            i = j - 1;
            nextAction = new CompareAction(i);
        } else if (nextAction.type == GameAction.GameActionType.Compare) {
            if(arrayToSort[i] > key) {
                nextAction = new MoveAction(i + 1, i);
            } else {
                nextAction = new MoveAction(i + 1);
            }
        } else if(nextAction.type == GameAction.GameActionType.Move){
            MoveAction a = (MoveAction)nextAction;
            if (a.store) {
                arrayToSort[i + 1] = key;
                j++;
                nextAction = new StoreAction(j);
            } else {
                arrayToSort[i + 1] = arrayToSort[i];
                i--;
                if (i == -1)
                    nextAction = new MoveAction(i + 1);
                else
                    nextAction = new CompareAction(i);
            }
            //if (i > -1) {
            //    if(arrayToSort[i] > key) {             
            //        arrayToSort[i + 1] = arrayToSort[i];
            //        i--;
            //        nextAction = i == -1 ? (GameAction) new MoveAction(i + 1): new CompareAction(i + 1);
            //    } else {
            //        arrayToSort[i + 1] = key;
            //        j++;
            //        nextAction = new StoreAction(j);
            //    }
            //} else {
            //    arrayToSort[i + 1] = key;
            //    nextAction = GameAction.GameActionType.Move;
            //}
        } 
    }

    
    
    public void Prev() {
        throw new System.NotImplementedException();
    }

}
