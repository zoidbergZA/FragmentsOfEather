using UnityEngine;
using System.Collections.Generic;

public class Stuka : Ship
{
    public delegate void StukaHandler();

    public static event StukaHandler StukaSpawned;
    public static event StukaHandler StukaKilled;

    [SerializeField] private float searchRange = 100f;
    [SerializeField] private float shootCooldown = 2f;
    [SerializeField] private float patrolDistance = 5f;
    [SerializeField] private GameObject particlesPrefab;
	[SerializeField] private LayerMask collisionMask;

    //sound variable here
    public enum EnemyState { SEARCH, ATTACK }
    private EnemyState state;
    private float searchFrequence = 1f;
    private float lastSearchedAt;
    private Vulnerable target;
    private float lastShotAt;
    private float lastTurnedAt;
    private bool flipped = false;
    private int patrolStrategy;


	public override void Awake()
    {
        base.Awake();
    }
	void Start ()
    {
        state = EnemyState.SEARCH;
        patrolStrategy = Random.Range(0, 2);

	    if (StukaSpawned != null)
	        StukaSpawned();
    }
	
	public override void FixedUpdate ()
    {        
        updateState();
		base.FixedUpdate();
	}

    public override void Die()
    {
        if (StukaKilled != null)
            StukaKilled();
		explodeStuka ();
        Destroy(gameObject);
    }

    private void updateState()
    {
		if (checkForwardCollision ()) 
		{
			// if true, turn away to avoid collision
			Thrust = -0.5f;
			if (Steering <= 0f)
				Steering = 1f;
			else
				Steering = -1f;
			return;
		}

        switch(state)
        {
            case EnemyState.SEARCH:
                updateSearchState();
                break;
            case EnemyState.ATTACK:
                updateAttackState();
                break;
            default:
                updateSearchState();
                break;
        }
    }

	private bool checkForwardCollision()
	{
		RaycastHit2D hit = Physics2D.Raycast (transform.position, transform.up, 30f, collisionMask);

		if (!hit)
			return false;
//		Debug.Log(hit.collider.name + " HIT: " + Time.time);

		if (target) 
		{
			if (hit.collider.gameObject == target.gameObject) {
				//			Debug.Log(targ.name + " LOS LOST: " + Time.time);
				return false;
			}
		}
		return true;
	}

    private void updateSearchState()
    {
        patrolArea();
        if (Time.time >= lastSearchedAt + searchFrequence)
        {
            lastSearchedAt = Time.time;
            searchForTarget();
        }
    }

    private void updateAttackState()
    {
        if (!target || target.IsDead)
        {
            state = EnemyState.SEARCH;
            GameManager.Instance.AudioManager.leaveBattle();
            return;
        }
        PlayerShip ps = target.GetComponent<PlayerShip>();
        if (ps && ps.IsHidden)
        {
            state = EnemyState.SEARCH;
            GameManager.Instance.AudioManager.leaveBattle();
            return;
        }
        handleAttack();
    }

	private bool checkLineOfSight(Vulnerable targ)
	{
		RaycastHit2D hit = Physics2D.Raycast (transform.position, targ.transform.position - transform.position);

		if (!hit)
			return false;
		//		Debug.Log("target: " + targ.name + ", " + hit.collider.name + " HIT: " + Time.time);
		if (hit.collider.gameObject != targ.gameObject) 
		{
			//			Debug.Log(targ.name + " LOS LOST: " + Time.time);
			return false;
		}

		return true;
	}

    private void searchForTarget()
    {
        List<PlayerShip> potentialTargets = new List<PlayerShip>();
        Mainland mainLand = null;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, searchRange);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            PlayerShip player = hitColliders[i].transform.GetComponent<PlayerShip>();
            mainLand = hitColliders[i].transform.GetComponent<Mainland>();
            if (player && !player.IsDead)
            {
                float distance = (player.transform.position - transform.position).magnitude;
				if (distance <= searchRange && checkLineOfSight(player)) 
					potentialTargets.Add(player);
            }
        }
        if (potentialTargets.Count > 0)
        {
            target = potentialTargets[Random.Range(0, potentialTargets.Count)];
            state = EnemyState.ATTACK;
            GameManager.Instance.AudioManager.enterBattle();
            //player detected and got attacked sound here
            //GetComponent<AudioSource>().PlayOneShot({ATTACK_SOUND});
        }
        // else if (potentialTargets.Count <= 0 && mainLand)
        // {
        //     target = mainLand;
        //     state = EnemyState.ATTACK;
        //     GameManager.Instance.AudioManager.enterBattle();
        // }
    }

    private void handleAttack()
    {
		float slipAngle = Utils.SignedAngle(transform.up, rigidbody.velocity.normalized);
		float speed = rigidbody.velocity.magnitude;
		float power = shipProperties.Power;

		float steeringInput = -Mathf.Clamp(Utils.SignedAngle(target.transform.position - transform.position, transform.up) / 20f, -1f, 1f);
//		Debug.Log (steeringInput);

		Thrust = 1f;
		Steering = steeringInput;

//		Debug.Log (Thrust);

//        Quaternion rot = Quaternion.LookRotation(transform.position - target.transform.position, Vector3.forward);
//        rot.x = 0.0f;
//        rot.y = 0.0f;
////        shipProperties.power = 1000f;
//        this.transform.rotation = rot;

        if (Time.time >= lastShotAt + shootCooldown)
        {
            lastShotAt = Time.time;

            Vector3 shootDirection = (target.transform.position - transform.position).normalized;

			if (Vector2.Dot (shootDirection, transform.up) > 0.60f) 
			{
				Fire (shootDirection);
				GameManager.Instance.AudioManager.playSFX ("ShootCannon");
			}
        }
        if(target is PlayerShip && (this.transform.position - target.transform.position).magnitude < 10f)
        {
            explodeStuka();
            target.TakeDamage(100, this);
        }
        else if (target is Mainland && (this.transform.position - target.transform.position).magnitude < 30f)
        {
            explodeStuka();
            target.TakeDamage(100, this);
            GameManager.Instance.AudioManager.playSFX("MainlandDamage");
        }
    }

    private void patrolArea()
    {
//        shipProperties.power = 500f;
        this.Thrust = 0.95f;
        patrolInCircle();

        // if (Time.time >= lastTurnedAt + patrolDistance)
        // {
        //     lastTurnedAt = Time.time;
        //     if(patrolStrategy == 0)
        //     {
        //         patrolInSnakeStyle();
        //     }
        //     else
        //     {
        //         patrolInSpiralStyle();
        //     }
        // }
    }

    private void patrolInCircle()
    {
        Steering = 1f;
    }

    private void patrolInSnakeStyle()
    {
        if (this.transform.rotation.eulerAngles.z <= 90 || this.transform.rotation.eulerAngles.z >= 270) this.Steering = -0.6f;
        else if (this.transform.rotation.eulerAngles.z > 90 || this.transform.rotation.eulerAngles.z < 270) this.Steering = 0.6f;
    }

    private void patrolInSpiralStyle()
    {
        if (!flipped)
        {
            Steering = 0.7f;
            flipped = !flipped;
        }
        else
        {
            Steering = -0.1f;
            flipped = !flipped;
        }
    }

    private void explodeStuka()
    {
        Instantiate(particlesPrefab, this.transform.position, Quaternion.identity);
//        ParticleSystem ps = particlesPrefab.GetComponent<ParticleSystem>();
//        if(ps) ps.Play();
        GameManager.Instance.AudioManager.playSFX("EnemyKilledExplosion");
        GameManager.Instance.AudioManager.leaveBattle();
//        Die();
    }
}
