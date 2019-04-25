using System.Collections;
using UnityEngine;

public class UseCase : MonoBehaviour
{

    public ImageHandler imageHandler;
    public DataStructure dataStructure;
    public ImageRep imageRep;
    public PatternRep patternRep;
    public GameObject scene;
    public ActionController actionController;
    public GameObject tutorial;
    public AlgoControlManager algoManager;

    [Range(4,16)]
    public int resolution = 8;

    private RegionGrowAlgorithm algorithm;
    private Stage.Data data;
    private bool demo = false;
    private bool started = false;

    public void ChangeDataModel(Stage.Data model) {
        data = model;
        dataStructure.SetMode(data);
        algorithm.ChangeMode(data);
    }

    public void ChangeMode(Stage.Mode mode) {
        if(mode == Stage.Mode.UseCase) {
            scene.SetActive(true);
            Restart();
        } else {
            scene.SetActive(false);
        }
    }

    private void Awake() {
        algorithm = new RegionGrowAlgorithm(resolution, data);
        EventManager.OnSeedChanged += ReSeed;
    }

    private void OnDestroy() {
        EventManager.OnSeedChanged -= ReSeed;
    }

    private void ReSeed(int index) {
        imageRep.Seed(index);
        patternRep.Seed(index);
        algorithm.SetSeedPoint(index);
    }

    public void Started() {
        started = true;
        tutorial.SetActive(false);
    }

    public void Push() {
        if (!started) return;
        Pixel p = imageHandler.GetSelectedPixel();
        if (p == null) {
            ShowHint();
            return;
        }
        if (algorithm.PerformStep(new ImageAction(p.index, ImageAction.ActionType.Push), p)) {
            dataStructure.Push(p);
            imageRep.Added(p.index);
        } else {
            ShowHint();
        }
    }

    public void Pop() {
        if (!started) return;
        if (algorithm.PerformStep(new ImageAction(0, ImageAction.ActionType.Pop))) {
            Pixel p = dataStructure.Pop();
            if (p == null) return;
            imageRep.Next(p.index);
        } else {
            ShowHint();
        }
    }

    public void Check() {
        if (!started) return;
        Pixel p = imageHandler.GetSelectedPixel();
        if (p == null) {
            ShowHint();
            return;
        }
        int index = p.index;
        bool pattern = p.Dark;
        if (algorithm.PerformStep(new ImageAction(index, ImageAction.ActionType.Check), p)) {
            imageRep.Visit(index, pattern);
            if(pattern) patternRep.Pattern(index);
        } else {
            ShowHint();
        }
    }

    public void Prev() {

    }

    public void Next() {
        algoManager.UpdateAlgoButtons(AlgoControlManager.State.Inactive);
        Select(true);
    }

    public void Demo() {
        if (!started) return;
        demo = !demo;
        algoManager.Demo(demo);
        if (demo)
            algoManager.UpdateAlgoButtons(AlgoControlManager.State.Demo);
        else if (algorithm.Complete)
            algoManager.UpdateAlgoButtons(AlgoControlManager.State.Finished);
        else if (algorithm.step == 0)
            algoManager.UpdateAlgoButtons(AlgoControlManager.State.Active);
        else
            algoManager.UpdateAlgoButtons(AlgoControlManager.State.InProgress);
        if (demo)
            actionController.UpdateState(ActionController.State.Empty);
        else
            actionController.UpdateState(ActionController.State.Selected);
        Select();
    }

    private void Select(bool force=false) {
        if ((!demo || algorithm.Complete)&&!force) return;
        ImageAction a = algorithm.GetNext();
        if (a.Type != ImageAction.ActionType.Pop)
            imageHandler.DemoSelectPixel(a.Pixel, function: Press);
        else
            Press();
    }

    private void Press() {
        if (!demo || algorithm.Complete) return;
        ImageAction a = algorithm.GetNext();
        actionController.Press(a.Type, Select);
    }

    public void ShowHint() {
        ImageAction a = algorithm.GetNext();
        if(a.Type != ImageAction.ActionType.Pop)
            imageHandler.Hint(a.Pixel);
        actionController.Hint(a.Type);
    }

    
    public void Restart() {
        started = false;
        demo = false;
        tutorial.SetActive(true);
        algorithm.Restart();
        imageRep.Restart(resolution);
        patternRep.Restart(resolution);
        dataStructure.Restart();
        imageHandler.Restart(resolution);
        actionController.UpdateState(ActionController.State.Empty);
    }
}
