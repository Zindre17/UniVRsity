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
    public TMPro.TextMeshPro messages;
    private Coroutine message = null;
    private int pixelInFocus;

    [Range(4,16)]
    public int resolution = 8;

    private RegionGrowAlgorithm algorithm;
    private Stage.Data data;
    private bool demo = false;
    private bool started = false;
    private bool performingAction = false;

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
        messages.text = "";
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
        if (demo || performingAction) return;
        imageHandler.SelectPixel(p.index);
        if (!algorithm.Complete)
            actionController.UpdateState(imageHandler.GetSelectedPixel() == null ? ActionController.State.Empty : ActionController.State.Selected);
    }

    private void ReSeed(int index) {
        imageRep.Seed(index);
        patternRep.Seed(index);
        algorithm.SetSeedPoint(index);
        pixelInFocus = index;
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
            if (pattern)
            {
                patternRep.Pattern(index);
                pixelInFocus = index;
            }
            dataStructure.Check();
            StepComplete();
        } else {
            ShowHint();
        }
    }

    public void Prev() {
        performingAction = true;
        UpdateAlgoButtons();
        ImageAction a = algorithm.GetPrev();
        algorithm.UndoStep();
        imageRep.Undo();
        if (a.Type == ImageAction.ActionType.Pop) {
            dataStructure.UnPop();
        } else if (a.Type == ImageAction.ActionType.Push) {
            dataStructure.UnPush();
        } else {
            if ((imageHandler.Get(a.Pixel)).Dark)
                patternRep.Undo();
            dataStructure.UnCheck();
            StepComplete();
        }
    }
    
    private void StepComplete(bool reverse=false) {
        performingAction = false;
        if (algorithm.Complete)
            ShowMessage("Algorithm complete!");
        if(demo && !algorithm.Complete)
        {
            StartCoroutine(DoStepRoutine());
        }
        else if (demo && algorithm.Complete)
        {
            demo = false;
            algoManager.Demo(demo);
        }
        if(!demo)
            UpdateAlgoButtons();
    }

    public void Next() {
        StartCoroutine(DoStepRoutine());
    }

    private void ShowMessage(string _message)
    {
        if (message != null)
            StopCoroutine(message);
        message = StartCoroutine(ShowMessageRoutine(_message));
    }

    private IEnumerator ShowMessageRoutine(string _message)
    {
        messages.text = _message;
        yield return new WaitForSeconds(5f);
        messages.text = "";
    }

    private IEnumerator DoStepRoutine() {
        performingAction = true;
        if (!demo)
            UpdateAlgoButtons();
        ImageAction a = algorithm.GetNext();
        ShowMessage(GetMessage(a));
        if(a.Type != ImageAction.ActionType.Pop) {
            yield return new WaitForSeconds(.2f);
            imageHandler.SelectPixel(a.Pixel);
            yield return new WaitForSeconds(.2f);
        }
        actionController.Press(a.Type);
    }

    private string GetMessage(ImageAction action)
    {
        switch (action.Type) {
            case ImageAction.ActionType.Check:
                return string.Format("Checking pixel at col {0}, row {1}", action.Pixel % resolution, Mathf.FloorToInt(action.Pixel / resolution));
            case ImageAction.ActionType.Pop:
                return string.Format("{0} for next pixel to check", data == Stage.Data.Stack ? "Popping" : "Dequeueing");
            case ImageAction.ActionType.Push:
                string pos;
                if (action.Pixel == pixelInFocus - 1) pos = "to the left";
                else if (action.Pixel == pixelInFocus + 1) pos = "to the right";
                else if (action.Pixel == pixelInFocus + resolution) pos = "above";
                else pos = "below";
                return string.Format("{0} the pixel {1}", data == Stage.Data.Stack ? "Pushing" : "Enqueueing", pos);
        }
        return "";
    }

    private void UpdateAlgoButtons() {
        if (performingAction)
            algoManager.UpdateAlgoButtons(AlgoControlManager.State.Inactive);
        else if (demo)
            algoManager.UpdateAlgoButtons(AlgoControlManager.State.Demo);
        else if (algorithm.Complete)
            algoManager.UpdateAlgoButtons(AlgoControlManager.State.Finished);
        else if (algorithm.step == 0)
            algoManager.UpdateAlgoButtons(AlgoControlManager.State.Active);
        else
            algoManager.UpdateAlgoButtons(AlgoControlManager.State.InProgress);
        if (demo || performingAction || algorithm.Complete)
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
        if (a == null) return;
        if(a.Type != ImageAction.ActionType.Pop)
            imageHandler.Hint(a.Pixel);
        actionController.Hint(a.Type);
    }
    
    public void Restart() {
        started = false;
        demo = false;
        algoManager.Demo(false);
        performingAction = false;
        tutorial.SetActive(true);
        algorithm.Restart();
        imageRep.Restart(resolution);
        patternRep.Restart(resolution);
        dataStructure.Restart();
        imageHandler.Restart(resolution);
        actionController.UpdateState(ActionController.State.Empty);
        algoManager.UpdateAlgoButtons(AlgoControlManager.State.Inactive);
        if (message != null)
            StopCoroutine(message);
        messages.text = "";
    }
}
