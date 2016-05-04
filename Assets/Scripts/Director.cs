using UnityEngine;
using System.Collections;

public class Director : MonoBehaviour
{
    public Transform visualHelper;

    [SerializeField] private float height = 80f;
    [SerializeField] private float back = 20f;
    [SerializeField] private float maxLookOffset = 15f;
    [SerializeField] private float maxHeight = 200f;
    [SerializeField] private float minHeight = 90f;
    [SerializeField] private float zoomRate = 15f;
    [SerializeField] private float farEdge = 0.33f;
    [SerializeField] private float nearEdge = 0.28f;

    //intro members
    [SerializeField] private float introStartHeight = 300;

    private Transform targeTransform;

    public bool IsMaxHeight
    {
        get
        {
            if (height/maxHeight > 0.91f)
                return true;
            return false;
        }
    }
    public PlayerShip furthestShip
    {
        get
        {
            float p1 = (GameManager.Instance.Mainland.transform.position - GameManager.Instance.player1.Ship.transform.position).sqrMagnitude;
            float p2 = (GameManager.Instance.Mainland.transform.position - GameManager.Instance.player2.Ship.transform.position).sqrMagnitude;

            if (p1 > p2)
                return GameManager.Instance.player1.Ship;
            else
                return GameManager.Instance.player2.Ship;
        }
    }

    void Start()
    {
        targeTransform = GameManager.Instance.Mainland.transform;
    }

	void Update ()
	{
	    if (GameManager.Instance.GameState == GameManager.GameStates.PREGAME)
	    {
	        HandleIntro();
	    }

        if (!GameManager.Instance.Mainland || !GameManager.Instance.player1.Ship || !GameManager.Instance.player2.Ship)
            return;

        Vector2 centroid = GameManager.Instance.Mainland.transform.position +
	                       GameManager.Instance.player1.Ship.transform.position +
	                       GameManager.Instance.player2.Ship.transform.position;

	    centroid /= 3f;

	    Vector3 offset =(Vector3) centroid - GameManager.Instance.Mainland.transform.position;
        Vector3 focusPoint = GameManager.Instance.Mainland.transform.position + Vector3.ClampMagnitude(offset, maxLookOffset);

//	    visualHelper.position = focusPoint;

        Vector2 furthest = Camera.main.WorldToViewportPoint(furthestShip.transform.position);
        furthest -= new Vector2(0.5f, 0.5f);

	    float closestToEdge = furthest.magnitude;

	    if (closestToEdge >= 0.35f && height < maxHeight)
	    {
	        height += zoomRate * Time.deltaTime;
	    }
        else if (closestToEdge <= 0.3f && height > minHeight)
        {
            height -= zoomRate * Time.deltaTime;
        }
        
        PositionRig(targeTransform.position + new Vector3(0, -back, -height), focusPoint);
	}

    private void HandleIntro()
    {
        Vector3 offset = (Vector3.back * introStartHeight * (1f - GameManager.Instance.IntroProgress));
//        offset += Vector3.back*minHeight;
//        Debug.Log(offset);
        PositionRig(targeTransform.position + new Vector3(0, -back, -height) + offset, targeTransform.position);
    }

    private void PositionRig(Vector3 pos, Vector3 lookAt)
    {
        transform.position = Vector3.Lerp(transform.position, pos, 0.6f);
        transform.LookAt(lookAt);
    }
}
