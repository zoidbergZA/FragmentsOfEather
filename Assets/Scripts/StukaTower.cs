using UnityEngine;
using System.Collections.Generic;

public class StukaTower : Ship
{
    [SerializeField] private float searchRange = 100f;
    [SerializeField] private float shootCooldown = 2f;

    //sound variable here
    public enum EnemyState { SEARCH, ATTACK }
    private EnemyState state;
    private float searchFrequence = 1f;
    private float lastSearchedAt;
    private Vulnerable target;
    private float lastShotAt;

    public override void Awake()
    {
        base.Awake();
    }
    void Start()
    {
        state = EnemyState.SEARCH;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        updateState();
    }

    public override void Die()
    {
        Destroy(gameObject);
    }

    private void updateState()
    {
        switch (state)
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

    private void updateSearchState()
    {
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
            return;
        }
        PlayerShip ps = target.GetComponent<PlayerShip>();
        if (ps && ps.IsHidden)
        {
            state = EnemyState.SEARCH;
            return;
        }
        handleAttack();
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
                if (distance <= searchRange) potentialTargets.Add(player);
            }
        }
        if (potentialTargets.Count > 0)
        {
            target = potentialTargets[Random.Range(0, potentialTargets.Count)];
            state = EnemyState.ATTACK;
            //player detected and got attacked sound here
            //GetComponent<AudioSource>().PlayOneShot({ATTACK_SOUND});
        }
//        else if (potentialTargets.Count <= 0 && mainLand)
//        {
//            target = mainLand;
//            state = EnemyState.ATTACK;
//        }
    }

    private void handleAttack()
    {
		if (!checkLineOfSight (target)) 
		{
			target = null;
			state = EnemyState.SEARCH;
			return;
		}

		if((target.transform.position - this.transform.position).magnitude <= searchRange)
        {
            if (Time.time >= lastShotAt + shootCooldown)
            {
                lastShotAt = Time.time;
                Vector3 shootDirection = (target.transform.position - transform.position).normalized;
                Fire(shootDirection);
				GameManager.Instance.AudioManager.playSFX ("ShootLaser3");
            }
        }
        else
        {
            state = EnemyState.SEARCH;
        }
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
}
