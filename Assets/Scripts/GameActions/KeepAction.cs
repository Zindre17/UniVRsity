using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepAction : GameAction {

    public int index1, index2;

    public KeepAction(int _index1, int _index2) {
        type = GameActionType.Keep;
        index1 = _index1;
        index2 = _index2;
    }
}
