using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Ship : Vulnerable
{
//    [SerializeField] protected float power = 500f;
//    [SerializeField] protected float maxSpeed = 15f;
//    [SerializeField] protected float turnPower = 2000f;
//    [SerializeField] protected float maxTurnRate = 60f;
//    [SerializeField] protected  float bankPower = 100f;

    public ShipProperties shipProperties;

    [SerializeField] protected ParticleSystem tail;
    [SerializeField] protected AudioClip shootSound;
    [SerializeField] protected Bullet bulletPrefab;
    [SerializeField] protected Transform shoot_ref;
    [SerializeField] protected float bulletRange = 30f;
    [SerializeField] protected float bulletSpeed = 10f;
    [SerializeField] protected float bulletDamage = 10f;
//    [SerializeField] protected GameObject ModelsHolder;

    protected Rigidbody2D rigidbody;

    public float Thrust { get; set; }
    public float Steering { get; set; }
    public bool IsReversing { get; private set; }

    public override void Awake()
    {
        base.Awake();

        rigidbody = GetComponent<Rigidbody2D>();
    }

    public override void Update()
    {
        base.Update();

        if (tail)
        {
            float frac = Hitpoints/MaxHP;
            if (frac < 0.15f) 
                frac = 0f;

            tail.startColor = Color.Lerp(Color.black, Color.white, frac);
            // tail.emissionRate = Mathf.Lerp(50f, 40f, frac);
        }
    }

    public virtual void FixedUpdate()
    {
        float slipAngle = Utils.SignedAngle(transform.up, rigidbody.velocity.normalized);
        float speed = rigidbody.velocity.magnitude;
        float power = shipProperties.Power;

        string state = "";

        // if (Thrust < 0)
        //     power = shipProperties.BrakePower;

        bool blockTorque = false;

        // //check reversing
        // if (Mathf.Abs(slipAngle) > 90 && speed > 1f)
        //     IsReversing = true;
        // else
        //     IsReversing = false;

        if (this is PlayerShip)
        {
            PlayerShip playerShip = this as PlayerShip;

            if (playerShip.IsTwirling)
            {
                blockTorque = true;
                rigidbody.angularVelocity = 0f;
            }
        }

        // if (Mathf.Abs(rigidbody.angularVelocity) < shipProperties.MaxTurnRate)
        {
            float steeringOutput = -Steering;

            if (IsReversing)
                steeringOutput *= -1f;

            if (!blockTorque)
            {
                rigidbody.AddTorque(steeringOutput * shipProperties.TurnPower * Time.deltaTime, ForceMode2D.Force);
            
                Vector2 bankForce = new Vector2(-slipAngle * rigidbody.velocity.sqrMagnitude * 0.4f, rigidbody.velocity.sqrMagnitude) * shipProperties.BankPower * Time.deltaTime;
            	rigidbody.AddRelativeForce(bankForce * Time.deltaTime, ForceMode2D.Force);
            }

            rigidbody.angularVelocity = Mathf.Clamp(rigidbody.angularVelocity, -shipProperties.MaxTurnRate, shipProperties.MaxTurnRate);
        }

        

//        if (gameObject == GameManager.Instance.player1.Ship.gameObject)
//        {
//            float angle = Utils.SignedAngle(transform.up, rigidbody.velocity);
//            float fwdSpeed = rigidbody.velocity.magnitude * Mathf.Cos(Mathf.Deg2Rad * angle);
//            Debug.Log(rigidbody.velocity.magnitude - fwdSpeed);
//        }

        

        rigidbody.AddRelativeForce(Thrust * Vector2.up * power * Time.deltaTime);

//		if (GameManager.Instance.player1.Ship == this) {
//			Debug.Log (Thrust * Vector2.up * power);
//		}

//		if (this is Stuka) {
//			Debug.Log (Thrust * Vector2.up * power);
//		}

//            if (speed < shipProperties.MinSpeed)
//            {
//                state = "min limit = speed: " + speed + ", " + Time.time;
//                rigidbody.AddForce(10f*transform.up*power*Time.deltaTime);
//            }
//            else
//            {
//                if (speed < shipProperties.MaxSpeed)
//                {
//                    state = "normal: " + Time.time;
//                    rigidbody.AddForce(Thrust*transform.up*power*Time.deltaTime);
//                }
//                else
//                {
//                    state = "Max limit: " + Time.time;
//                }
//            }
        
        //        rigidbody.velocity = Vector2.ClampMagnitude(rigidbody.velocity, shipProperties.MaxSpeed);

//        if (gameObject == GameManager.Instance.player1.Ship.gameObject)
//        {
//            Debug.Log(name + " state: " + state);
//        }

            HandleVisualBanking(slipAngle);
    }

    public virtual void Fire(Vector3 direction)
    {
        Bullet b = Instantiate(bulletPrefab, shoot_ref.position, shoot_ref.rotation) as Bullet;
        b.transform.LookAt(shoot_ref.up);
        Rigidbody2D bulletBody = b.GetComponent<Rigidbody2D>();

        direction *= bulletSpeed;

        //ignore bullet collisions with self
        Collider2D[] myColliders = GetComponentsInChildren<Collider2D>();

        foreach (Collider2D col in myColliders)
        {
            Physics2D.IgnoreCollision(b.GetComponent<Collider2D>(), col, true);
        }

        //bulletBody.AddForce(direction);
        bulletBody.AddForce(direction, ForceMode2D.Impulse);
        //set bullet stats
        b.Damage = bulletDamage;

        b.GetComponent<SelfDestruct>().timer = bulletRange / bulletSpeed;
    }

    public void IncreaseMaxSpeed(float amount)
    {
//        shipProperties.maxSpeed += amount;
    }

    public void IncreasePower(float amount)
    {
//        shipProperties.power += amount;
    }

    public virtual void HandleVisualBanking(float slipAngle)
    {
        if (!modelsHolder)
            return;

        if (Mathf.Abs(slipAngle) > 90)
        {
            modelsHolder.transform.localEulerAngles = Vector3.zero;
            return;
        }
        slipAngle *= rigidbody.velocity.sqrMagnitude;
        slipAngle *= 0.004f;

        slipAngle = Mathf.Clamp(slipAngle, -shipProperties.BankAngle, shipProperties.BankAngle);
        modelsHolder.transform.localEulerAngles = new Vector3(0, slipAngle, 0);

//        if (GameManager.Instance.player1.Ship)
//        {
//            if (gameObject == GameManager.Instance.player1.Ship.gameObject)
//                Debug.Log("holder: " + modelsHolder.name + ", angle: " + slipAngle + ", rot: " + modelsHolder.transform.localEulerAngles);
//        }
    }
}
