using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : GameAction {
    public int source, target;
    public bool store;

    public MoveAction(int _source, int _target) {
        store = false;
        source = _source;
        target = _target;
        type = GameActionType.Move;
    }

    public MoveAction(int _target) {
        store = true;
        target = _target;
        source = -1;
        type = GameActionType.Move;
    }

    public override bool Equals(object obj) {
        if (obj.GetType() == typeof(MoveAction)) {
            MoveAction a = (MoveAction)obj;
            return store == a.store && source == a.source && target == a.target;
        } else return false;
    }
}
