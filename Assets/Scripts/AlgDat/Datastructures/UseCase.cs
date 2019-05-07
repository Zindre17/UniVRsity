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

    private ActionController.State state = ActionController.State.Empty;

    public void ChangeDataModel(Stage.Data model) {
        data = model;
        enabled = true;
    }

    public void ChangeMode(Stage.Mode mode) {
        if(mode == Stage.Mode.UseCase) {
            scene.SetActive(true);
            Restart();
        } else {
            scene.SetActive(false);
        }
    }

    private void Start() {
        enabled = false;
        algorithm = new RegionGrowAlgorithm(resolution, data);
        EventManager.OnSeedChanged += ReSeed;
        EventManager.OnActionCompleted += StepComplete;
        EventManager.OnPixelSelected += PixelSelected;
    }
    private void Update()
    {
        dataStructure.SetMode(data);
        algorithm.ChangeMode(data);
        actionController.UpdateLabels(data == Stage.Data.Queue);
        enabled = false;
    }

    private void OnDestroy() {
        EventManager.OnSeedChanged -= ReSeed;
        EventManager.OnActionCompleted -= StepComplete;
        EventManager.OnPixelSelected -= PixelSelected;
    }

    private void PixelSelected(Pixel p)
    {
        if (demo) return;
        imageHandler.SelectPixel(p.index);
        state = imageHandler.GetSelectedPixel() == null ? ActionController.State.Empty : ActionController.State.Selected;
        actionController.UpdateState(state);
    }

    private void ReSeed(int index) {
        imageRep.Seed(index);
        patternRep.Seed(index);
        algorithm.SetSeedPoint(index);
    }

    public void Started() {
        started = true;
        tutorial.SetActive(false);
        UpdateAlgoButtons();
        actionController.UpdateState(ActionController.State.Empty);
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
            StepComplete();
        } else {
            ShowHint();
        }
    }

    public void Prev() {
        algoManager.UpdateAlgoButtons(AlgoControlManager.State.Inactive);
        ImageAction a = algorithm.GetPrev();
        imageRep.Undo();
        if (a.Type == ImageAction.ActionType.Pop) {
            dataStructure.UnPop();
        } else if (a.Type == ImageAction.ActionType.Push) {
            dataStructure.UnPush();
        } else {
            if ((imageHandler.Get(a.Pixel)).Dark)
                patternRep.Undo();
            StepComplete();
        }
        algorithm.UndoStep();
    }
    
    private void StepComplete(bool reverse=false) {
        if(demo && !algorithm.Complete)
        {
            StartCoroutine(DoStepRoutine());
        }
        else if (demo && algorithm.Complete)
        {
            demo = false;
            algoManager.Demo(demo);
        }
        UpdateAlgoButtons();
    }

    public void Next() {
        algoManager.UpdateAlgoButtons(AlgoControlManager.State.Inactive);
        StartCoroutine(DoStepRoutine());
    }

    private IEnumerator DoStepRoutine() {
        actionController.UpdateState(ActionController.State.Empty);
        ImageAction a = algorithm.GetNext();
        if(a.Type != ImageAction.ActionType.Pop) {
            yield return new WaitForSeconds(.2f);
            imageHandler.SelectPixel(a.Pixel);
            yield return new WaitForSeconds(.2f);
        }
        actionController.Press(a.Type);
    }

    private void UpdateAlgoButtons() {
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
    }

    public void Demo() {
        if (!started) return;
        demo = !demo;
        algoManager.Demo(demo);
        UpdateAlgoButtons();
        if(demo && !algorithm.Complete)
            StartCoroutine(DoStepRoutine());
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
        algoManager.UpdateAlgoButtons(AlgoControlManager.State.Inactive);
    }
}
