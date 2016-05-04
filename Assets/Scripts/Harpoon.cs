using UnityEngine;
using System.Collections;
//using UnityEditor;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Harpoon : MonoBehaviour
{
    public delegate void HookHandler();

    public static event HookHandler IslandHooked;

//    public Transform visualHelper;

    [SerializeField] private Transform chainRoot;
    [SerializeField] private Transform cable_ref;
    [SerializeField] private float retractRate = 1f;
    [SerializeField] private float shootForce = 500f;
    [SerializeField] public GameObject model;
    [SerializeField] private DistanceJoint2D hookedJoint;
    [SerializeField] private DistanceJoint2D baseJoint;
    
    private LineRenderer lineRenderer;
    private float maxDistance;
    private bool isRetracting;
    private PlayerShip myShip;
    private Collider2D myCollider;
    private Rigidbody2D myRigidbody;

    public bool IsHooked
    {
        get
        {
            if (!hookedJoint.connectedBody)
                return false;
            return true;
        }
    }
    public bool IsReady { get; private set; }
    public bool IsTeleporter { get; private set; }

    void Awake()
    {
        myShip = transform.root.GetComponent<PlayerShip>();
        myCollider = GetComponent<Collider2D>();
        myRigidbody = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        maxDistance = baseJoint.distance;
        hookedJoint.enabled = false;

        //ignore own ship collider
        Physics2D.IgnoreCollision(myCollider, transform.root.GetComponent<Collider2D>());        
    }

    void Start()
    {
//        ActivateTeleporter();
        Reset();
    }

    void Update()
    {
        if (!IsHooked && !IsReady)
            Retract();

        if (isRetracting)
            HandleRetract();

        if (lineRenderer.enabled)
        {
            lineRenderer.SetPosition(0, chainRoot.position);
            lineRenderer.SetPosition(1, cable_ref.position);
        }
    }

    public void Fire()
    {
        if (IsHooked)
        {
            Release();
            return;
        }

        if (isRetracting || !IsReady || myShip.IsTwirling)
            return;

        transform.position = myShip.transform.position;
        transform.rotation = myShip.transform.rotation;

        model.SetActive(true);
        myCollider.enabled = true;
        lineRenderer.enabled = true;
        myRigidbody.WakeUp();
        myRigidbody.velocity = transform.root.GetComponent<Rigidbody2D>().velocity;
        myRigidbody.AddForce(transform.root.up * shootForce, ForceMode2D.Impulse);
        IsReady = false;

        Retract();
    }

    public void Release()
    {
        if (!IsHooked)
            return;

        isRetracting = true;
        hookedJoint.connectedBody = null;
        hookedJoint.enabled = false;
        myRigidbody.freezeRotation = false;
    }

    public void ActivateTeleporter()
    {
        if (IsTeleporter)
            return;

        IsTeleporter = true;
    }

    public void Reset()
    {
        myRigidbody.freezeRotation = false;
        hookedJoint.enabled = false;
        myCollider.enabled = false;
        lineRenderer.enabled = false;
        model.SetActive(false);
        baseJoint.distance = maxDistance;
        myRigidbody.Sleep();
        isRetracting = false;
        IsReady = true;
    }

    private void Retract()
    {
        if (isRetracting)
            return;

        isRetracting = true;
    }

    private void HandleRetract()
    {
        baseJoint.distance -= retractRate*Time.deltaTime;
        
        if (baseJoint.distance < 0.2f)
        {
            baseJoint.distance = 0;
            isRetracting = false;
            Reset();
        } 
    }

    private void Hook(Rigidbody2D other, Vector2 contact)
    {
//        visualHelper.transform.position = contact;
        myCollider.enabled = false;
        // baseJoint.connectedAnchor = transform.InverseTransformPoint(contact);
        // hookedJoint.connectedAnchor = other.transform.InverseTransformPoint(contact);
//        visualHelper.transform.position = contact;

        isRetracting = false;
        hookedJoint.enabled = true;
        hookedJoint.connectedBody = other;
        myRigidbody.freezeRotation = true;

        other.angularVelocity *= 0.5f;
        other.angularDrag = 0.15f;

//        //position the harpoon
//        transform.position = contact;
//        hookedJoint.distance = (other.transform.position - transform.position).magnitude;

        //set cable length to the distance to hooked object (note: commented coz may be better for slingshots)
        baseJoint.distance = (other.transform.position - myShip.transform.position).magnitude;
//        baseJoint.distance = maxDistance;

        
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.relativeVelocity.magnitude < 100f) //min speed for penetration
            return;

        if (coll.gameObject.layer != LayerMask.NameToLayer("Hookable"))
        {
            GameManager.Instance.AudioManager.playSFX("HarpoonHitMetal");
            return;
        }

        Island island = coll.transform.GetComponent<Island>();
        if (island)
        {
            island.SetOwner(myShip.Owner);
            GameManager.Instance.AudioManager.playSFX("HarpoonHitRock");

            if (IslandHooked != null)
                IslandHooked();

            if (IsTeleporter)
            {
                island.transform.position = GameManager.Instance.Mainland.transform.position;
                island.ConnectToMainland();
                myRigidbody.velocity = Vector2.zero;
                //return before hook call
                return;
            }
        }

        Hook(coll.rigidbody, coll.contacts[0].point);
    }
}
