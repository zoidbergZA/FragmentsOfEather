using UnityEngine;
using System.Collections;

public class TweenTest : MonoBehaviour
{
	void Start ()
    {
        LeanTween.color(gameObject, Color.blue, 0.3f).setLoopPingPong().setDelay(2f);
    }
}
