using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotAction : GameAction {

    public int pivotIndex;

    public PivotAction(int _index) {
        type = GameActionType.Pivot;
        pivotIndex = _index;
    }
}
