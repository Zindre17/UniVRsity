using System.Collections.Generic;
using UnityEngine;

public abstract class SortingAlgorithm {

    public int step;
    protected List<GameAction> actions;
    protected List<IState> states;
    protected int[] array;
    protected int size;
    protected string name;
    protected string[] pseudo;
    protected int cursor;
    protected List<int> focusChangePoints;

    public bool Complete { get; private set; }
    protected interface IState {
        string Readable();
    }

    public SortingAlgorithm(int arraySize, int[] _array) {
        size = arraySize;
        array = (int[])_array.Clone();
        step = 0;
        Complete = false;
        actions = new List<GameAction>();
        states = new List<IState>();
        focusChangePoints = new List<int>();
        GenerateActions();
        CheckForFocusChange();
    }

    public virtual string GetStorageName()
    {
        return "";
    }

    protected abstract void CheckForFocusChange(bool reverse = false);
    public virtual bool NeedsExtraSpace()
    {
        return false;
    }

    public bool CorrectAction(GameAction action) {
        if (Complete) return false;
        return actions[step].EqualTo(action);
    }

    public GameAction GetAction() {
        if (Complete) return null;
        return actions[step];
    }

    public GameAction GetAction(int _step) {
        if (_step > actions.Count || _step < 0) return null;
        return actions[_step];
    }

    public string GetName() {
        return name;
    }

    public string[] GetPseudo() {
        return pseudo;
    }

    public string GetState() {
        if (step >= states.Count) return "Complete!";
        return states[step].Readable();
    }

    public virtual void Next() {
        if (Complete) return;
        step++;
        CheckForFocusChange();
        if (step == actions.Count) {
            Complete = true;
            EventManager.AlgorithmCompleted();
        }
    }

    public virtual void Prev() {
        if (step == 0) return;
        if (Complete)
            Complete = false;
        step--;
        CheckForFocusChange(true);
    }

    public void Restart() {
        step = 0;
        Complete = false;
        cursor = 0;
        CheckForFocusChange();
    }

    protected abstract void GenerateActions();

    public virtual void Update(int[] newArray) {
        Restart();
        array = (int[])newArray.Clone();
        actions.Clear();
        states.Clear();
        GenerateActions();
    }
}
