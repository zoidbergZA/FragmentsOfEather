using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class Cloud : MonoBehaviour
{
    private Vector2 myVelocity = Vector2.zero;
	[SerializeField] private GameObject placer;

	void Awake()
	{
		placer.SetActive (false);
	}
	
	void Update ()
    {
	    transform.Translate(myVelocity * Time.deltaTime);
	}

    public void SetVelocity(Vector2 velocity)
    {
        myVelocity = velocity;
    }
}
