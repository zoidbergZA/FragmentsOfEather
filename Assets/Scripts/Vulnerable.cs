using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Vulnerable : MonoBehaviour 
{
    [SerializeField] private float maxHitpoints = 100f;
    [SerializeField] private float regenerationRate = 0f;
    [SerializeField] protected GameObject modelsHolder;
    [SerializeField] private float flashDuration = 0.25f;
	[SerializeField] private string damageSound = "";

    private Renderer[] renderers;
    public Color normalColor;
//    private readonly float maxFlashDelay = 3f;
//    private readonly float flashTime = 0.25f;
//    private LTDescr flashTween;
//    private bool isFlashing;

    public bool Invulnerable { get; set; }
    public bool IsDead { get; protected set; }
    public float Hitpoints { get; protected set; }
    public float MaxHP { get { return maxHitpoints; } }

    public virtual void Awake()
    {
        Hitpoints = MaxHP;
        renderers = modelsHolder.GetComponentsInChildren<Renderer>();
        normalColor = renderers[0].material.color;
    }

    public virtual void Start()
    {
        if (GameManager.Instance.ShowHealthbars)
        {
            Healthbar healthbar = Instantiate(GameManager.Instance.HealthbarPrefab);
            healthbar.SetTarget(this);
        }
        
//        PrepareNextFlash();
    }

    public virtual void Update()
    {
        HandleRegeneration();
 
        // //damage color fade
        // for (int i = 0; i < renderers.Length; i++)
        // {
        //     renderers[i].material.color = Color.Lerp(Color.white, normalColor, Hitpoints/maxHitpoints);
        // }
    }

    public abstract void Die();

    public virtual void TakeDamage(float amount, Vulnerable attacker)
    {
        if (IsDead || amount <= 0)
            return;

        if (Invulnerable)
            return;

        Hitpoints -= amount;

		if (amount > 2f && damageSound != "") 
		{
			GameManager.Instance.AudioManager.playSFX (damageSound);
		}

//        if (!isFlashing)
//            PrepareNextFlash();

        if (Hitpoints <= 0 && !IsDead)
        {
            Hitpoints = 0;
            IsDead = true;

            Die();
        }
    }

    public void Heal(float amount)
    {
        if (Hitpoints >= maxHitpoints || IsDead)
            return;

        Hitpoints += amount;

        if (Hitpoints > maxHitpoints)
            Hitpoints = MaxHP;
    }

    public void IncreaseRegeneration(float amount)
    {
        regenerationRate += amount;
    }

//    private void PrepareNextFlash()
//    {
//        if (Hitpoints >= MaxHP - 1f)
//        {
//            isFlashing = false;
//            return;
//        }
//
//        isFlashing = true;
////        Debug.Log("tweener for: " + name + ", at: " + Time.time);
//
////        if (modelsHolder)
////        {
//        float delayTimer = (Hitpoints/MaxHP*maxFlashDelay) + flashDuration; 
//        LeanTween.color(modelsHolder.gameObject, Color.white, flashDuration).setLoopPingPong(1).setDelay(delayTimer).setOnComplete(PrepareNextFlash);
////        }
//    }

    private void HandleRegeneration()
    {
        if (IsDead)
            return;

        Hitpoints += regenerationRate * Time.deltaTime;

        if (Hitpoints > maxHitpoints)
            Hitpoints = maxHitpoints;
    }

    public virtual void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.layer == LayerMask.NameToLayer("Harpoons"))
            return;

        float damage = coll.relativeVelocity.magnitude * 0.35f;
        // Debug.Log(damage);
        if (damage < 1f)
            return;

        TakeDamage(damage, null);
    }
}
