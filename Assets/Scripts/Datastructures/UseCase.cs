using UnityEngine;

public class UseCase : MonoBehaviour
{

    public ImageHandler imageHandler;
    public DataStructure dataStructure;
    public ImageRep imageRep;

    private Stage.Data data;
    public Stage.Data Data {
        get { return data; }
        set {
            data = value;
            dataStructure.SetMode(data);
        }
    }

    public void Push() {
        Pixel p = imageHandler.GetSelectedPixel();
        if (p == null) return;
        dataStructure.Push(p);
    }

    public void Pop() {
        Pixel p = dataStructure.Pop();
        if (p == null) return;
    }

    private void Start() {
        imageRep.Resolution = imageHandler.resolution;
    }
}
