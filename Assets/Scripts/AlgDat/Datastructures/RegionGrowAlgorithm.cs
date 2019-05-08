using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RegionGrowAlgorithm {

    public RegionGrowAlgorithm(int _resolution, Stage.Data datamode) {
        resolution = _resolution;
        data = datamode;
        actions = new List<ImageAction>();
        datastructure = new List<int>();
        image = new bool[resolution * resolution];
    }

    private int resolution;
    private Stage.Data data;
    private List<int> datastructure;
    private List<ImageAction> actions;
    internal int step = 0;
    private bool[] image;

    public bool Complete { get; private set; }

    private void ExploreNeighbourPixels(int index, bool b) {
        if (b) {
            TryPush(index + resolution);
            if(!AtRightEdge(index)) TryPush(index + 1);
            TryPush(index - resolution);
            if(!AtLeftEdge(index)) TryPush(index - 1);
        } 
        PopAndCheckPopped();    
    }

    private bool AtRightEdge(int index) {
        return index % resolution == resolution - 1;
    }

    private bool AtLeftEdge(int index) {
        return index % resolution == 0;
    }

    private void PopAndCheckPopped() {
        if (datastructure.Count == 0) return;
        actions.Add(new ImageAction(0, ImageAction.ActionType.Pop));
        int index;
        if (data == Stage.Data.Stack)
            index = datastructure[datastructure.Count - 1];
        else
            index = datastructure[0];
        datastructure.Remove(index);
        actions.Add(new ImageAction(index, ImageAction.ActionType.Check));
    }

    private void TryPush(int index) {
        if (index >= resolution * resolution || index < 0) return;
        if (image[index]) return;
        actions.Add(new ImageAction(index, ImageAction.ActionType.Push));
        datastructure.Add(index);
        image[index] = true;
    }

    public void ChangeMode(Stage.Data mode) {
        if (mode == Stage.Data.LinkedList) return;

        data = mode;
        Restart();
    }

    public void Restart() {
        step = 0;
        Complete = false;
        datastructure.Clear();
        actions.Clear();
        image = new bool[resolution * resolution];
    }

    public void SetSeedPoint(int index) {
        if (datastructure.Count != 0) return;
        image[index] = true;
        ExploreNeighbourPixels(index, true);
    }

    public bool CorrectStep(ImageAction action) {
        if (step >= actions.Count) return false;
        return actions[step].Equals(action);
    }

    public bool PerformStep(ImageAction action) {
        if (action.Type != ImageAction.ActionType.Pop) return false;
        if (CorrectStep(action)){
            step++;
            Complete = step == actions.Count;
            return true;
        }
        return false;
    }

    public bool PerformStep(ImageAction action, Pixel pixel) {
        if (CorrectStep(action)) {
            if (action.Type == ImageAction.ActionType.Check) { 
                if (pixel.index == action.Pixel)
                    ExploreNeighbourPixels(action.Pixel, pixel.Dark);
                else return false;
            }
            step++;
            Complete = step == actions.Count;
            return true;
        }
        return false;
    }

    public void UndoStep() {
        if (Complete)
            Complete = false;
        step--;
    }

    public ImageAction GetNext() {
        if (Complete) return null;
        return actions[step];
    }
    public ImageAction GetPrev() {
        if (step == 0) return null;
        return actions[step - 1];
    }
}
