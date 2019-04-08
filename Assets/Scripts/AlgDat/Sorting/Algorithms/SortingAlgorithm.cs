using System.Collections.Generic;

public abstract class SortingAlgorithm {

    internal int step;
    internal bool complete;
    internal List<GameAction> actions;
    internal List<string> states;
    internal int[] array;
    internal int size;
    internal string name;
    internal string[] pseudo;

    public SortingAlgorithm(int arraySize, int[] _array) {
        size = arraySize;
        array = (int[])_array.Clone();
        step = 0;
        complete = false;
        actions = new List<GameAction>();
        states = new List<string>();
        GenerateActions();
    }

    public bool CorrectAction(GameAction action) {
        if (complete) return false;
        return actions[step].EqualTo(action);
    }

    public GameAction GetAction() {
        if (complete) return null;
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
        return states[step];
    }

    public virtual void Next() {
        if (complete) return;
        step++;
        if (step == actions.Count) {
            complete = true;
            EventManager.AlgorithmCompleted();
        }
    }

    public void Prev() {
        if (complete)
            complete = false;
        if(step!= 0)
            step--;
    }

    public void Restart() {
        step = 0;
        complete = false;
    }

    internal abstract void GenerateActions();

    public virtual void Update(int[] newArray) {
        Restart();
        array = (int[])newArray.Clone();
        actions.Clear();
        states.Clear();
        GenerateActions();
    }
}
