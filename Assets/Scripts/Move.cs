public class Move {

    private Element[] selections;

    private int selectionCount;
    private int maxSelections = 3;


    public Move(Element _selection1, Element _selection2, Element _selection3) {
        selections = new Element[maxSelections];
        selections[0] = _selection1;
        selections[1] = _selection2;
        selections[2] = _selection3;
        selectionCount = 3;
    }

    public Move(Element _selection1, Element _selection2) {
        selections = new Element[maxSelections];
        selections[0] = _selection1;
        selections[1] = _selection2;
        selections[2] = null;
        selectionCount = 2;
    }

    public Move(Element _selection1) {
        selections = new Element[maxSelections];
        selections[0] = _selection1;
        selections[1] = selections[2] = null;
        selectionCount = 1;
    }

    public Move() {
        selections = new Element[maxSelections];
        selections[0] = selections[1] = selections[2] = null;
        selectionCount = 0;
    }

    public void AddSelection(Element selection) {
        if (selectionCount == maxSelections)
            return;
        int index = selectionCount;
        selections[index] = selection;
        selectionCount++;
    }

    public void RemoveSelection(Element selection) {
        if (selectionCount == 0)
            return;
        for( int i = 0; i< selectionCount; i++) {
            if( selections[i].Equals(selection)) {
                selections[i] = null;
                selectionCount--;
                return;
            }
        }
    }

    public Element GetFirstSelection() {
        return selections[0];
    }

    public Element GetSecondSelection() {
        return selections[1];
    }

    public Element GetThirdSelection() {
        return selections[2];
    }

    public int GetSelectionCount() {
        return selectionCount;
    }

}