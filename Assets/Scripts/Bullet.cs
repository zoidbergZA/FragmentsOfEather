using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour 
{
    public enum BulletType
    {
        PlayerBullet,
        EnemyBullet
    }

    [SerializeField] private BulletType bulletType;
    [SerializeField] private GameObject damageParticlesPrefab;
	
	public float Damage { get; set; }
    
    void Update()
    {
//        transform.Translate(Direction * Speed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Vulnerable vulnerable = collision.transform.root.GetComponent<Vulnerable>();

        if (vulnerable)
        {
            if (bulletType == BulletType.EnemyBullet)
            {
                if (vulnerable is PlayerShip || vulnerable is Mainland)
                {
                    vulnerable.TakeDamage(Damage, null);
                    Instantiate(damageParticlesPrefab, this.transform.position, Quaternion.identity);
                    ParticleSystem ps = damageParticlesPrefab.GetComponent<ParticleSystem>();
                    if (ps) ps.Play();
                }
            }

            if (bulletType == BulletType.PlayerBullet)
            {
                if (vulnerable is Stuka) vulnerable.TakeDamage(Damage, null);
            }
        }

//        Instantiate(impactPrefab, transform.position, transform.rotation);
        Destroy(gameObject);
    }

	
	
	
	
	
	
	
}
