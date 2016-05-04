using UnityEngine;
using System.Collections;

public class Utils
{
    public static float SignedAngle(Vector2 a, Vector2 b)
    {
        float ang = Vector2.Angle(a, b);
        Vector3 cross = Vector3.Cross(a, b);

        if (cross.z > 0)
            ang = 360 - ang;

        if (ang > 180)
            ang -= 360;

        return ang;
    }
}
