using UnityEngine;

public class SwapAction : GameAction
{
    public int index1, index2;

    public SwapAction(int _index1, int _index2)
    {
        type = GameActionType.Swap;
        index1 = _index1;
        index2 = _index2;
    }

    //public override void DoAction()
    //{
    //    int temp = s1.Index;
    //    s1.Index = s2.Index;
    //    s2.Index = temp;

    //    Vector3 s1Pos = new Vector3(s1.transform.position.x, s1.transform.position.y, s1.transform.position.z);
    //    Vector3 s2Pos = new Vector3(s2.transform.position.x, s2.transform.position.y, s2.transform.position.z);

    //    Vector3[] s1Path = new Vector3[] { s1Pos - Vector3.forward * scale, s2Pos - Vector3.forward * scale, s2Pos };
    //    Vector3[] s2Path = new Vector3[] { s2Pos - Vector3.forward * scale * 2, s1Pos - Vector3.forward * scale * 2, s1Pos };

    //    s1.Move(s1Path);
    //    s2.Move(s2Path);
    //}
}