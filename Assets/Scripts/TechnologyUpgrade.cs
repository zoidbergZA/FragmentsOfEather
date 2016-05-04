using UnityEngine;
using System.Collections;

public class TechnologyUpgrade : ScriptableObject
{
    public int steps = 5;

    public void DoUpgrade()
    {
        Debug.Log("apply technology upgrade " + name + " at: " + Time.time);
    }
}
