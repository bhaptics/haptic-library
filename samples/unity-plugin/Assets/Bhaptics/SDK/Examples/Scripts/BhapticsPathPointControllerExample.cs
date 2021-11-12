using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Bhaptics.Tact;
using Bhaptics.Tact.Unity;


public class BhapticsPathPointControllerExample : MonoBehaviour
{
    public HapticClipPositionType clipPositionType;
    public int motorIntensity = 100;




    private List<PathPoint> pathPointList;
    private BoxCollider targetCollider;
    private string key = System.Guid.NewGuid().ToString();
    private int duration = 100;
    private bool isClickedArea;







    void Awake()
    {
        targetCollider = GetComponentInChildren<BoxCollider>();

        pathPointList = new List<PathPoint>();
        pathPointList.Add(new PathPoint(0f, 0f, motorIntensity));
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isClickedArea = GetPathPointOffset() != null;
        }

        if (!isClickedArea)
        {
            return;
        }

        if (!Input.GetMouseButton(0))
        {
            isClickedArea = false;
            return;
        }

        var haptic = BhapticsManager.GetHaptic();

        if (haptic == null)
        {
            return;
        }

        var currentPathPointOffset = GetPathPointOffset();

        if (currentPathPointOffset != null)
        {
            pathPointList[0].X = currentPathPointOffset.Value.x;
            pathPointList[0].Y = currentPathPointOffset.Value.y;

            haptic.Submit(key, BhapticsUtils.ToPositionType(clipPositionType), pathPointList, duration);
        }
    }







    private Vector2? GetPathPointOffset()
    {
        var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitInfo;

        if (Physics.Raycast(mouseRay, out hitInfo))
        {
            var hitCollider = hitInfo.collider.GetComponent<BoxCollider>();
            if (hitCollider == null)
            {
                return null;
            }

            if (!targetCollider.Equals(hitCollider))
            {
                return null;
            }

            return ConvertPositionToPathOffset(hitInfo.point, hitCollider);
        }

        return null;
    }

    private Vector2 ConvertPositionToPathOffset(Vector3 currentPos, BoxCollider currentHitCollider)
    {
        Vector2 widthArea = new Vector2(
            currentHitCollider.transform.position.x - currentHitCollider.size.x * 0.5f
            , currentHitCollider.transform.position.x + currentHitCollider.size.y * 0.5f);

        Vector2 heightArea = new Vector2(
            currentHitCollider.transform.position.y + currentHitCollider.size.x * 0.5f
            , currentHitCollider.transform.position.y - currentHitCollider.size.y * 0.5f);

        return new Vector2(
            (currentPos.x - widthArea.x) / (widthArea.y - widthArea.x)
            , (currentPos.y - heightArea.x) / (heightArea.y - heightArea.x));
    }
}