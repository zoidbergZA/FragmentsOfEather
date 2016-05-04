/*using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour
{
    private Transform mainLand;
    private enum CameraState { AtBase, OuttaBase }
    private CameraState state;
    private float initialFOV;
    public float rightOuterBound = 0.9f;
    public float leftOuterBound = 0.1f;
    public float upperOuterBound = 0.9f;
    public float bottomOuterBound = 0.1f;
    public float rightInnerBound = 0.6f;
    public float leftInnerBound = 0.3f;
    public float upperInnerBound = 0.6f;
    public float bottomInnerBound = 0.3f;
    public float offsetX;
    public float offsetY;
    public float offsetZ;
    private Vector3 viewPosP1;
    private Vector3 viewPosP2;
    private float zoomStep = 0.1f;
    private bool P1beyondBounds = false;
    private bool P2beyondBounds = false;
    private bool P1withinBounds = false;
    private bool P2withinBounds = false;
    private bool P1inDeadZone = false;
    private bool P2inDeadZone = false;

    void Start()
    {
        mainLand = GameManager.Instance.Mainland.transform;
        state = CameraState.AtBase;
        initialFOV = Camera.main.fieldOfView;
    }

    void Update()
    {
        updateViewPosition();
        checkBeyondBounds();
        checkWithinBounds();
        Debug.Log("within1: " + P1withinBounds + " | beyound1: " + P1beyondBounds + " | within2: " + P2withinBounds + " | beyound2: " + P2beyondBounds +
            " | inDead1: " + P1inDeadZone + " | inDead2: " + P2inDeadZone + " | p1.x: " + viewPosP1.x + " | p1.y:" + viewPosP1.y +
            " | p2.x:" + viewPosP2.x + " | p2.y:" + viewPosP2.y);
    }
	
	void LateUpdate ()
    {
        if (!mainLand || !GameManager.Instance.player1.Ship || !GameManager.Instance.player2.Ship) return;
        updateState();
	}

    private void updateViewPosition()
    {
        viewPosP1 = Camera.main.WorldToViewportPoint(GameManager.Instance.player1.Ship.transform.position);
        viewPosP2 = Camera.main.WorldToViewportPoint(GameManager.Instance.player2.Ship.transform.position);
    }

    private bool atBase()
    {
        if (GameManager.Instance.player1.Ship.IsInSafezone && GameManager.Instance.player2.Ship.IsInSafezone) return true;
        else return false;
    }

    private bool outtaBase()
    {
        if (!GameManager.Instance.player1.Ship.IsInSafezone || !GameManager.Instance.player2.Ship.IsInSafezone) return true;
        else return false;
    }

    private void updateState()
    {
        switch (state)
        {
            case CameraState.AtBase:
                updateBothAtBase();
                break;
            case CameraState.OuttaBase:
                updateOuttaBase();
                break;
            default:
                updateBothAtBase();
                break;
        }
    }

    private void updateBothAtBase()
    {
        if (atBase())
        {
            if (Camera.main.fieldOfView > initialFOV) decreaseFOV();
            transform.position = new Vector3(mainLand.position.x + offsetX, mainLand.position.y + offsetY, mainLand.position.z + offsetZ);
        }
        else actualizeState();
    }

    private void updateOuttaBase()
    {
        if (outtaBase())
        {
            manageZoom();
            transform.position = new Vector3(mainLand.position.x + offsetX, mainLand.position.y + offsetY, mainLand.position.z + offsetZ);
        }
        else actualizeState();
    }

    private void actualizeState()
    {
        if (atBase()) state = CameraState.AtBase;
        else if (outtaBase()) state = CameraState.OuttaBase;
    }

    private void checkBeyondBounds()
    {
        if (viewPosP1.x > rightOuterBound || viewPosP1.x < leftOuterBound || viewPosP1.y > upperOuterBound || viewPosP1.y < bottomOuterBound)
        {
            P1beyondBounds = true;
            P1inDeadZone = false;
            P1withinBounds = false;
        }
        if (viewPosP2.x > rightOuterBound || viewPosP2.x < leftOuterBound || viewPosP2.y > upperOuterBound || viewPosP2.y < bottomOuterBound)
        {
            P2beyondBounds = true;
            P2inDeadZone = false;
            P2withinBounds = false;
        }
    }

    private void checkWithinBounds()
    {
        if (viewPosP1.x < rightInnerBound && viewPosP1.x > leftInnerBound && viewPosP1.y < upperInnerBound && viewPosP1.y > bottomInnerBound)
        {
            P1withinBounds = true;
            P1inDeadZone = false;
            P1beyondBounds = false;
        }
        if (viewPosP2.x < rightInnerBound && viewPosP2.x > leftInnerBound && viewPosP2.y < upperInnerBound && viewPosP2.y > bottomInnerBound)
        {
            P2withinBounds = true;
            P2inDeadZone = false;
            P2beyondBounds = false;
        }
    }

    private void checkDeadZone()
    {
        if (viewPosP1.x < rightOuterBound && viewPosP1.x > leftOuterBound && viewPosP1.y < upperOuterBound && viewPosP1.y > bottomOuterBound &&
            viewPosP1.x > rightInnerBound && viewPosP1.x < leftInnerBound && viewPosP1.y > upperInnerBound && viewPosP1.y < bottomInnerBound)
        {
            P1inDeadZone = true;
            P1beyondBounds = false;
            P1withinBounds = false;
        }
        if (viewPosP2.x < rightOuterBound && viewPosP2.x > leftOuterBound && viewPosP2.y < upperOuterBound && viewPosP2.y > bottomOuterBound &&
            viewPosP2.x > rightInnerBound && viewPosP2.x < leftInnerBound && viewPosP2.y > upperInnerBound && viewPosP2.y < bottomInnerBound)
        {
            P2inDeadZone = true;
            P2beyondBounds = false;
            P2withinBounds = false;
        }
    }

    private void manageZoom()
    {
        if ((P1beyondBounds && P2inDeadZone) || (P1beyondBounds && P2withinBounds) || (P1beyondBounds && P2beyondBounds) ||
            (P2beyondBounds && P1inDeadZone) || (P2beyondBounds && P1withinBounds)) increaseFOV();
        if ((P1withinBounds && P2withinBounds) && Camera.main.fieldOfView > initialFOV) decreaseFOV();
    }

    private void increaseFOV()
    {
        Camera.main.fieldOfView += zoomStep;
    }

    private void decreaseFOV()
    {
        Camera.main.fieldOfView -= zoomStep;
    }
}*/
