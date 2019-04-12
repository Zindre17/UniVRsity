﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementAnimator : MonoBehaviour
{
    public GameObject backWall;
    private Coroutine routine;
    private readonly float movementMagnitude = 0.4f;

    private SortingElement prevPivot;

    public void Stop() {
        if(routine != null) {
            StopCoroutine(routine);
            routine = null;
        }
        backWall.transform.localPosition = new Vector3(0,5,10);
        if (prevPivot != null)
            prevPivot.Pivot = false;
    }

    #region Merge
    public void MergeComplete(ArrayManager array, List<PartialArray> splits, Split split, CombinedArray combined) {
        routine = StartCoroutine(MergeCompleteAnimation(array, splits, split, combined));
    }

    private IEnumerator MergeCompleteAnimation(ArrayManager array, List<PartialArray> splits, Split split, CombinedArray combined) {
        float prevTime = Time.time;
        float duration = .6f;
        float elapsed = 0f;
        int i;
        Vector3 leftSize = split.Left.transform.localScale;
        Vector3 rightSize = split.Right.transform.localScale;
        List<Vector3> starts = new List<Vector3>(splits.Count);
        for(i = 0; i< splits.Count; i++) {
            starts.Add(splits[i].transform.position);
        }
        Vector3 arrayStart = array.transform.position;
        Vector3 wallStart = backWall.transform.position;
        Vector3 path = Vector3.forward * 1.5f;
        float percent;
        while (elapsed < duration) {
            percent = elapsed / duration;
            for(i = 0; i <splits.Count; i++) {
                splits[i].transform.position = starts[i] - path * percent;
            }
            array.transform.position = arrayStart - path * percent;
            backWall.transform.position = wallStart - path * percent;
            split.Left.transform.localScale = leftSize * (1 - percent);
            split.Right.transform.localScale = rightSize * (1 - percent);
            float time = Time.time;
            elapsed += time - prevTime;
            prevTime = time;
            yield return null;
        }
        for(i = 0; i < splits.Count; i++) {
            splits[i].transform.position = starts[i] - path;
        }
        array.transform.position = arrayStart - path;
        backWall.transform.position = wallStart - path;
        for (i = 0; i < combined.Size; i++) {
            SortingElement correct = combined.Get(i);
            for(int j = 0; j < splits.Count; j++) {
                if (splits[j].End >= correct.Index && splits[j].Start <= correct.Index) {
                    splits[j].Get(correct.Index - splits[j].Start).Size = correct.Size;
                }
            }
            array.Get(correct.Index).Size = correct.Size;
        }
        Destroy(split.Left.gameObject);
        Destroy(split.Right.gameObject);
        Destroy(combined.gameObject);
        if (splits.Count > 1) {
            splits[splits.Count - 1].Active = true;
            splits[splits.Count - 1].InFocus = true;
            splits[splits.Count - 2].Active = true;
            splits[splits.Count - 2].InFocus = true;
        }
        routine = null;
    }

    public void Merge(Split split, CombinedArray combined) {
        routine = StartCoroutine(MergeAnimation(split, combined));
    }

    private IEnumerator MergeAnimation(Split split, CombinedArray array) {
        float prevTime = Time.time;
        float duration = .6f;
        float elapsed = 0f;
        float percent;
        Vector3 path = new Vector3((split.Left.Size + split.Right.Size+1) / 4f , 0, 0);
        Vector3 leftStart = split.Left.transform.position;
        Vector3 rightStart = split.Right.transform.position;
        Vector3 size = array.transform.localScale;
        array.transform.localScale = Vector3.zero;
        array.gameObject.SetActive(true);
        split.Left.Expand();
        split.Right.Expand();

        while (elapsed < duration) {
            percent = elapsed / duration;
            array.transform.localScale = size * percent;
            split.Left.transform.position = leftStart - path * percent;
            split.Right.transform.position = rightStart + path * percent;
            float time = Time.time;
            elapsed += time - prevTime;
            prevTime = time;
            yield return null;
        }
        array.transform.localScale = size;
        split.Left.transform.position = leftStart - path;
        split.Right.transform.position = rightStart + path;

        routine = null;
        EventManager.ActionCompleted();
    }
    #endregion

    #region Split
    public void Split(ArrayManager array, List<Split> splits) {
        routine = StartCoroutine(SplitAnimation(array, splits));
    }

    private IEnumerator SplitAnimation(ArrayManager array, List<Split> splits) {
        float prevTime = Time.time;
        float duration = .6f;
        float elapsed = 0f;
        float percent;
        Vector3 aStart = array.transform.position;
        Vector3 wStart = backWall.transform.position;
        Vector3 path = Vector3.forward * 1.5f;
        List<Vector3> starts = null;
        Split s;
        int i;
        if(splits != null) {
            if (splits.Count != 0) {
                starts = new List<Vector3>(splits.Count * 2 - 1);
                for (i = 0; i < splits.Count - 1; i++) {
                    s = splits[i];
                    starts.Add(s.Left.transform.position);
                    starts.Add(s.Right.transform.position);
                }
            }
        }

        while (elapsed < duration) {
            percent = elapsed / duration;
            array.transform.position = aStart + path * percent;
            backWall.transform.position = wStart + path * percent;
            if (splits != null) {
                if (splits.Count != 0) {
                    for (i = 0; i < splits.Count -1; i++) {
                        s = splits[i];
                        s.Left.transform.position = starts[i * 2] + path * percent;
                        s.Right.transform.position = starts[i * 2 + 1] + path * percent;
                    }
                }
            }
            float time = Time.time;
            elapsed += time - prevTime;
            prevTime = time;
            yield return null;
        }
        array.transform.position = aStart + path;
        backWall.transform.position = wStart + path;
        if (splits != null) {
            if (splits.Count != 0) {
                for (i = 0; i < splits.Count - 1; i++) {
                    s = splits[i];
                    s.Left.transform.position = starts[i * 2] + path;
                    s.Right.transform.position = starts[i * 2 + 1] + path;
                }
            }
        }
        splits[splits.Count - 1].Show();
        routine = null;
        EventManager.ActionCompleted();
    }

    #endregion

    #region Swap

    public void Swap(SortingElement s1, SortingElement s2) {
        if (s1.Index == s2.Index) {
            routine = StartCoroutine(SwapAnimation(s1));
        } else {
            routine = StartCoroutine(SwapAnimation(s1, s2));
        }
    }

    private IEnumerator SwapAnimation(SortingElement s) {
        float prevTime = Time.time;
        float elapsed = 0;
        float duration = .6f;
        float part = duration / 2f;
        Vector3 start = s.transform.position;
        Vector3 path = -Vector3.forward * movementMagnitude;

        while (elapsed < duration) {
            float percent;
            if (elapsed < part) {
                percent = elapsed / part;
                s.transform.position = start + path * percent;
            } else {
                percent = (elapsed - part) / part;
                s.transform.position = start + path - path * percent;
            }
            float time = Time.time;
            elapsed += time - prevTime;
            prevTime = time;
            yield return null;
        }
        s.transform.position = start;
        routine = null;
        EventManager.ActionCompleted();
    }

    private IEnumerator SwapAnimation(SortingElement s1, SortingElement s2) {
        float prevTime = Time.time;
        float elapsed = 0;
        float duration = .6f;
        float part = duration / 3f;

        Vector3 s1Point1, s1Point2, s1Point3, s1Point4;
        Vector3 s2Point1, s2Point2, s2Point3, s2Point4;
        Vector3 s1Path1, s1Path2, s1Path3;
        Vector3 s2Path1, s2Path2, s2Path3;

        s1Point1 = s1.transform.position;
        s2Point1 = s2.transform.position;

        s1Point2 = new Vector3(s1Point1.x, s1Point1.y, s1Point1.z - 1 * movementMagnitude);
        s2Point2 = new Vector3(s2Point1.x, s2Point1.y, s2Point1.z - 2 * movementMagnitude);

        s1Point3 = new Vector3(s2Point1.x, s2Point1.y, s1Point1.z - 1 * movementMagnitude);
        s2Point3 = new Vector3(s1Point1.x, s1Point1.y, s2Point1.z - 2 * movementMagnitude);

        s1Point4 = s2Point1;
        s2Point4 = s1Point1;

        s1Path1 = s1Point2 - s1Point1;
        s1Path2 = s1Point3 - s1Point2;
        s1Path3 = s1Point4 - s1Point3;

        s2Path1 = s2Point2 - s2Point1;
        s2Path2 = s2Point3 - s2Point2;
        s2Path3 = s2Point4 - s2Point3;

        while (elapsed < duration) {
            float percent;
            if (elapsed < part) {
                percent = elapsed / part;
                s1.transform.position = s1Point1 + s1Path1 * percent;
                s2.transform.position = s2Point1 + s2Path1 * percent;
            } else if (elapsed < part * 2) {
                percent = (elapsed - part) / part;
                s1.transform.position = s1Point2 + s1Path2 * percent;
                s2.transform.position = s2Point2 + s2Path2 * percent;
            } else {
                percent = (elapsed - part * 2) / part;
                s1.transform.position = s1Point3 + s1Path3 * percent;
                s2.transform.position = s2Point3 + s2Path3 * percent;
            }
            float time = Time.time;
            elapsed += time - prevTime;
            prevTime = time;
            yield return null;
        }

        s1.transform.position = s1Point1;
        s2.transform.position = s2Point1;

        int temp = s1.Size;
        s1.Size = s2.Size;
        s2.Size = temp;

        if (s1.Pivot) {
            prevPivot.Pivot = false;
            s2.Pivot = true;
            prevPivot = s2;
        } else if (s2.Pivot) {
            prevPivot.Pivot = false;
            s1.Pivot = true;
            prevPivot = s1;
        }

        routine = null;
        EventManager.ActionCompleted();
    }

    #endregion

    #region Store
    public void Store(Vector3 center, SortingElement stored, SortingElement target) {
        routine = StartCoroutine(StoreAnimation(center, stored, target));
    }

    private IEnumerator StoreAnimation(Vector3 center, SortingElement stored, SortingElement target) {
        stored.gameObject.SetActive(true);
        stored.Size = target.Size;
        float duration = .7f;
        float elapsed = 0;
        float part1 = .35f;
        float prevTime = Time.time;
        Vector3 mid = new Vector3(target.transform.position.x, target.transform.position.y, center.z);
        Vector3 path1 = mid - target.transform.position;
        Vector3 path2 = center - mid;
        while (duration > elapsed) {
            float percent;
            if (elapsed < part1) {
                percent = elapsed / part1;
                stored.transform.position = target.transform.position + percent * path1;
            } else {
                percent = (elapsed - part1) / (duration - part1);
                stored.transform.position = mid + percent * path2;
            }
            float time = Time.time;
            elapsed += (time - prevTime);
            prevTime = time;
            yield return null;
        }
        stored.transform.position = center;
        routine = null;
        EventManager.ActionCompleted();
    }

    #endregion

    #region CopyTo
    public void CopyTo(SortingElement s1, SortingElement s2) {
        routine = StartCoroutine(CopyToAnimation(s1, s2));
    }

    private IEnumerator CopyToAnimation(SortingElement source, SortingElement target) {
        target.Size = source.Size;
        float prevTime = Time.time;
        float duration = .6f;
        float elapsed = 0f;
        bool fromStore = source.Index == -1;
        float part;
        Vector3 start = source.transform.position;
        Vector3 t = target.transform.position;
        if (fromStore) {
            part = duration / 2f;
            Vector3 mid = new Vector3(t.x, t.y, start.z);
            Vector3 path1 = mid - start;
            Vector3 path2 = t - mid;
            while (elapsed < duration) {
                float percent;
                if (elapsed < part) {
                    percent = elapsed / part;
                    target.transform.position = start + path1 * percent;
                } else {
                    percent = (elapsed - part) / part;
                    target.transform.position = mid + path2 * percent;
                }
                float time = Time.time;
                elapsed += time - prevTime;
                prevTime = time;
                yield return null;
            }
        } else {
            part = duration / 3f;
            Vector3 path1 = -Vector3.forward * movementMagnitude;
            Vector3 p2 = start + path1;
            Vector3 path2 = new Vector3(t.x - start.x, t.y - start.y, 0);
            Vector3 p3 = p2 + path2;
            Vector3 path3 = t - p3;
            while (elapsed < duration) {
                float percent;
                if (elapsed < part) {
                    percent = elapsed / part;
                    target.transform.position = start + path1 * percent;
                } else if (elapsed < part * 2) {
                    percent = (elapsed - part) / part;
                    target.transform.position = p2 + path2 * percent;
                } else {
                    percent = (elapsed - part * 2) / part;
                    target.transform.position = p3 + path3 * percent;
                }
                float time = Time.time;
                elapsed += time - prevTime;
                prevTime = time;
                yield return null;
            }
        }
        target.transform.position = t;
        routine = null;
        EventManager.ActionCompleted();
    }
    #endregion

    #region Pivot
    public void Pivot(SortingElement s) {
        if (prevPivot != null)
            prevPivot.Pivot = false;
        prevPivot = s;
        prevPivot.Pivot = true;
        EventManager.ActionCompleted();
    }
    #endregion
}
