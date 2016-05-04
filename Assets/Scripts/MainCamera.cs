using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour
{
    private Transform mainLand;
//    private Transform p1;
//    private Transform p2;
    private enum CameraState { AtBase, OuttaBase }
    private CameraState state;
    private float initialFOV;
    public float rightBound = 0.9f;
    public float leftBound = 0.1f;
    public float upperBound = 0.9f;
    public float bottomBound = 0.1f;
    public float offsetX;
    public float offsetY;
    public float offsetZ;
    private Vector3 viewPosP1;
    private Vector3 viewPosP2;
    private float zoomStep = 0.15f;
    private float deadZone = 0.1f;
    private bool beyondBounds = false;
    private bool withinBounds = false;

    void Start()
    {
        mainLand = GameManager.Instance.Mainland.transform;
//        p1 = GameManager.Instance.player1.Ship.transform;
//        p2 = GameManager.Instance.player2.Ship.transform;
        state = CameraState.AtBase;
        initialFOV = Camera.main.fieldOfView;
    }

    void Update()
    {
        updateViewPosition();
        checkBeyondBounds();
        checkWithinBounds();
    }

    void LateUpdate ()
    {
        if (!mainLand || !GameManager.Instance.player1.Ship || !GameManager.Instance.player2.Ship) return;
        updateState();
        /*Debug.Log("within: " + withinBounds + " | beyond: " + beyondBounds + " | p1.x: " + viewPosP1.x + " | p1.y:" + viewPosP1.y +
            " | p2.x: " + viewPosP2.x + " | p2.y: " + viewPosP2.y + " | FOV: " + Camera.main.fieldOfView);*/
    }

    private void updateViewPosition()
    {
        if (!mainLand || !GameManager.Instance.player1.Ship || !GameManager.Instance.player2.Ship) return;

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
            transform.position = new Vector3(mainLand.position.x + offsetX, mainLand.position.y - offsetY, mainLand.position.z + offsetZ);
        }
        else actualizeState();
    }

    private void updateOuttaBase()
    {
        if (outtaBase())
        {
            manageZoom();
            transform.position = new Vector3(mainLand.position.x + offsetX, mainLand.position.y - offsetY, mainLand.position.z + offsetZ);
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
        if (viewPosP1.x > rightBound || viewPosP1.x < leftBound || viewPosP1.y > upperBound || viewPosP1.y < bottomBound ||
            viewPosP2.x > rightBound || viewPosP2.x < leftBound || viewPosP2.y > upperBound || viewPosP2.y < bottomBound)
        {
            beyondBounds = true;
            withinBounds = false;
        }
        else beyondBounds = false;
    }

    private void checkWithinBounds()
    {
        if (viewPosP1.x < rightBound - deadZone && viewPosP1.x > leftBound + deadZone && viewPosP1.y < upperBound - deadZone && viewPosP1.y > bottomBound + deadZone &&
            viewPosP2.x < rightBound - deadZone && viewPosP2.x > leftBound + deadZone && viewPosP2.y < upperBound - deadZone && viewPosP2.y > bottomBound + deadZone)
        {
            beyondBounds = false;
            withinBounds = true;
        }
        else withinBounds = false;
    }

    private void manageZoom()
    {
        if (beyondBounds) increaseFOV();
        if (withinBounds && Camera.main.fieldOfView > initialFOV) decreaseFOV();
    }

    private void increaseFOV()
    {
        Camera.main.fieldOfView += zoomStep;
    }

    private void decreaseFOV()
    {
        Camera.main.fieldOfView -= zoomStep;
    }
}