using Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    private static Compass compass;
    private static Transform sTransform;

    private void Awake()
    {
        compass = this;
        sTransform = transform;
    }

    public static void ChangeDirection(Direction dir)
    {
        Vector3 wantedRot = Vector3.forward;

        switch (dir)
        {
            case Direction.up:
                wantedRot *= 0;
                break;
            case Direction.right:
                wantedRot *= 90;
                break;
            case Direction.down:
                wantedRot *= 180;
                break;
            case Direction.left:
                wantedRot *= 270;
                break;
            default:
                break;
        }

        sTransform.LerpRotation(wantedRot, 2, compass);
    }
}
