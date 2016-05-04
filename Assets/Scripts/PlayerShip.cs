using UnityEngine;
using System.Collections;
//using UnityEditor;

[RequireComponent(typeof(TechnologyProgressor))]
public class PlayerShip : Ship
{
    public delegate void SafezoneHandler(bool enter);

    public static event SafezoneHandler SafezoneTriggered;

    [SerializeField] private float twirlDuration = 2f;
    [SerializeField] private float twirlCooldown = 6f;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject deathParticlesPrefab;
    [SerializeField] private GameObject damageParticlesPrefab;
//    [SerializeField] private GameObject model;

    private Collider2D myCollider;
    //private Rigidbody2D myRigidbody; //already serialized in parent class Ship
    private Harpoon harpoon;
    private int cloudCount;
    private TechnologyProgressor techProgressor;
    private float nextTwirlAt;
       
    public Player Owner { get; private set; }
    public WeaponManager WeaponManager { get; private set; }
    public Harpoon Harpoon { get { return harpoon; } }
    public bool IsInSafezone { get; private set; }
    public bool IsTwirling { get; private set; }

    public bool IsTwirlReady
    {
        get
        {
            if (!IsTwirling && Time.time >= nextTwirlAt)
                return true;
            else
                return false;
        }
    }

    public bool IsHidden
    {
        get { if (cloudCount > 0) return true;
            return false;
        }
    }

    public override void Awake()
    {
        base.Awake();

        techProgressor = GetComponent<TechnologyProgressor>();
        WeaponManager = GetComponentInChildren<WeaponManager>();
        myCollider = GetComponent<Collider2D>();
        //myRigidbody = GetComponent<Rigidbody2D>(); //already serialized in parent class Ship
        harpoon = GetComponentInChildren<Harpoon>();
        animator.enabled = false;
    }

    public override void Update()
    {
        base.Update();

//        if (this == GameManager.Instance.player1.Ship)
//            Debug.Log(IsReversing + ", " + Time.time);

        CheckOutOfBounds();

        if (IsInSafezone)
        {
            Heal(8f * Time.deltaTime);
        }
        //else
        {
            if (Mathf.Abs(Thrust) > 0.1f)
            {
				//if (tail.isPaused)
				tail.enableEmission = true;
                TakeDamage(shipProperties.BurnRate * Time.deltaTime, null);
            }
            else
            {
				//if (!tail.isPaused)
				tail.enableEmission = false; 
            }
        }
    }

    public void UpdateProperties()
    {
        rigidbody.mass = shipProperties.Mass;
        rigidbody.drag = shipProperties.LinearDrag;
        rigidbody.angularDrag = shipProperties.AngularDrag;
    }

    public void SetOwner(Player owner)
    {
        Owner = owner;
        normalColor = owner.color;

        //temp color
        Renderer[] renderers = modelsHolder.GetComponentsInChildren<Renderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = owner.color;
        }

		Harpoon.model.GetComponent<Renderer> ().material.color = owner.color;

        //        modelsHolder.GetComponentInChildren<Renderer>().material.color = Owner.color;
    }

    public void Reset()
    {
        IsDead = false;
        Hitpoints = MaxHP;
        techProgressor.SetModel(GameManager.Instance.TechnologyManager.NextTechnologyLevel - 1);
        UpdateProperties();
    }
    public void Twirl()
    {
        if (IsTwirlReady)
            StartCoroutine(StartTwirl());
    }

    public override void HandleVisualBanking(float slipAngle)
    {
        if (!IsTwirling)
        {
            base.HandleVisualBanking(slipAngle); 
            return;
        }
    }

    public override void Die()
    {
        Instantiate(deathParticlesPrefab, this.transform.position, Quaternion.identity);
        ParticleSystem ps = deathParticlesPrefab.GetComponent<ParticleSystem>();
        if (ps) ps.Play();
        Hitpoints = 0f;
        IsDead = true;
        Harpoon.Release();
        Harpoon.Reset();
        GameManager.Instance.AudioManager.playSFX("PlayerkilledExplosion");
        if (Owner)
            Owner.KillShip();
    }

    private void CheckOutOfBounds()
    {
        Vector2 myPos = Camera.main.WorldToViewportPoint(transform.position);
        myPos -= new Vector2(0.5f, 0.5f);
    
        if (myPos.magnitude >= 0.497f)
        {
            // Die();
            TakeDamage(6f * Time.deltaTime, null);
        }
    }

    private IEnumerator StartTwirl()
    {
        IsTwirling = true;
        animator.enabled = true;
        nextTwirlAt = Time.time + twirlCooldown;
        myCollider.isTrigger = true;
        modelsHolder.transform.localEulerAngles = new Vector3(0, 180, 0);
        animator.speed = 1f/twirlDuration;
        animator.Play("Twirl");
//        EditorApplication.isPaused = true;
        yield return new WaitForSeconds(twirlDuration);

        animator.enabled = false;
        modelsHolder.transform.localEulerAngles = Vector3.zero;
        myCollider.isTrigger = false;
        IsTwirling = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Safezone safezone = other.GetComponent<Safezone>();

        if (safezone)
        {
            IsInSafezone = true;

            if (SafezoneTriggered != null)
                SafezoneTriggered(true);
        }

        Cloud cloud = other.GetComponent<Cloud>();
        if (cloud)
        {
            GameManager.Instance.AudioManager.playSFX("Clouds");
            cloudCount++;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Safezone safezone = other.GetComponent<Safezone>();

        if (safezone)
        {
            IsInSafezone = false;

            if (SafezoneTriggered != null)
                SafezoneTriggered(false);
        }

        Cloud cloud = other.GetComponent<Cloud>();
        if (cloud)
        {
            cloudCount--;
        }
    }
}
