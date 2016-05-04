using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Bomb : MonoBehaviour
{
    [SerializeField] private float attractionForce = 200f;
    [SerializeField] private float damage = 250f;
//    [SerializeField] private float damageRange = 20f;

    private GameObject target;
    private Rigidbody2D myRigidbody2D;

    void Awake()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (target)
        {
            Vector2 dir = (target.transform.position - transform.position).normalized;
            myRigidbody2D.AddForce(dir * attractionForce * Time.deltaTime, ForceMode2D.Impulse);
        }
    }

    public void SetTarget(GameObject target)
    {
        if (this.target != null)
            return;
        
        this.target = target;
    }

    private void Explode()
    {
        if(target != null)
        {
            Vulnerable vulnerable = target.transform.root.GetComponent<Vulnerable>();
            if (vulnerable)
            {
                vulnerable.TakeDamage(damage, null);
            }
        }
        GameManager.Instance.AudioManager.playSFX("BombExplosion");
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (target)
        {
            if (coll.gameObject != target.gameObject)
                return;
        }
        Explode();
    }
}
