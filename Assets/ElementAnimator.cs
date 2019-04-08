using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementAnimator : MonoBehaviour
{
    public GameObject backWall;
    private Coroutine routine;
    private readonly float movementMagnitude = 0.4f;

    private SortingElement compared1, compared2;
    private SortingElement prevPivot;

    public void Stop() {
        if(routine != null) {
            StopCoroutine(routine);
            routine = null;
        }
        compared1 = compared2 = prevPivot = null;
        backWall.transform.localPosition = new Vector3(0,5,10);
    }

    public void Split(ArrayManager array, Split split, List<Split> splits) {
        routine = StartCoroutine(SplitAnimation(array, split, splits));
    }

    private IEnumerator SplitAnimation(ArrayManager array, Split split ,List<Split> splits) {
        float prevTime = Time.time;
        float duration = .6f;
        float elapsed = 0f;
        float percent;
        Vector3 aStart = array.transform.position;
        Vector3 wStart = backWall.transform.position;
        Vector3 path = Vector3.forward * 1.5f;
        List<Vector3> starts = null;
        if(splits != null) {
            if (splits.Count != 0) {
                starts = new List<Vector3>(splits.Count * 2 - 1);
                foreach (Split s in splits) {
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
                int i = 0;
                foreach(Split s in splits) {
                    s.Left.transform.position = starts[i * 2] + path * percent;
                    s.Right.transform.position = starts[i * 2 + 1] + path * percent;
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
            int i = 0;
            foreach (Split s in splits) {
                s.Left.transform.position = starts[i * 2] + path;
                s.Right.transform.position = starts[i * 2 + 1] + path;
            }
        }
        splits.Add(split);
        split.Show();
        routine = null;
        EventManager.ActionCompleted();
    }

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

    #region Compare
    public void Compare(SortingElement s1, SortingElement s2) {
        
        routine = StartCoroutine(CompareAnimation(s1, s2));

        
    }

    private IEnumerator DecompareAnimation(SortingElement s1, SortingElement s2) {
        float prevTime = Time.time;
        float duration = .3f;
        float elapsed = 0f;
        Vector3 path = Vector3.forward * movementMagnitude;
        Vector3 s1Start, s2Start;
        if (s1 != null)
            s1Start = s1.transform.position;
        else
            s1Start = Vector3.zero;
        if (s2 != null)
            s2Start = s2.transform.position;
        else
            s2Start = Vector3.zero;
        float percent;
        while(elapsed < duration) {
            percent = elapsed / duration;
            if (s1 != null)
                s1.transform.position = s1Start + path * percent;
            if (s2 != null)
                s2.transform.position = s2Start + path * percent;

            float time = Time.time;
            elapsed += time - prevTime;
            prevTime = time;
            yield return null;
        }
        if (s1 != null)
            s1.transform.position = s1Start + path;
        if (s2 != null)
            s2.transform.position = s2Start + path;
        routine = null;
        EventManager.ActionCompleted();
    }

    private IEnumerator CompareAnimation(SortingElement s1, SortingElement s2) {
        bool decompare1, decompare2, compare1, compare2;
        compare1 = compare2 = true;
        decompare1 = decompare2 = false;
        if (compared1 != null) {
            decompare1 = decompare2 = true;
            if (s1.Index == compared1.Index) {
                decompare1 = false;
                compare1 = false;
            }
            if (s1.Index == compared2.Index) {
                decompare2 = false;
                compare1 = false;
            }
            if (s2.Index == compared1.Index) {
                decompare1 = false;
                compare2 = false;
            }
            if (s2.Index == compared2.Index) {
                decompare2 = false;
                compare2 = false;
            }
        }
        float prevTime = Time.time;
        float duration = .3f;
        float elapsed = 0f;

        Vector3 path = -Vector3.forward * movementMagnitude;
        Vector3 s1Start, s2Start, d1Start, d2Start;
        s1Start = s1.transform.position;
        s2Start = s2.transform.position;
        d1Start = compared1 == null ? Vector3.zero : compared1.transform.position;
        d2Start = compared2 == null ? Vector3.zero : compared2.transform.position;

        while (elapsed < duration) {
            float percent = elapsed / duration;
            if (compare1) {
                s1.transform.position = s1Start + path * percent;
            }
            if (compare2) {
                s2.transform.position = s2Start + path * percent;
            }
            if (decompare1) {
                compared1.transform.position = d1Start - path * percent;
            }
            if (decompare2) {
                compared2.transform.position = d2Start - path * percent;
            }
            float time = Time.time;
            elapsed += time - prevTime;
            prevTime = time;
            yield return null;
        }
        if (compare1) {
            s1.transform.position = s1Start + path;
        }
        if (compare2) {
            s2.transform.position = s2Start + path;
        }
        if (decompare1) {
            compared1.transform.position = d1Start - path;
        }
        if (decompare2) {
            compared2.transform.position = d2Start - path;
        }
        compared1 = s1;
        compared2 = s2;
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
