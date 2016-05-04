using UnityEngine;
using System.Collections;

public class PlayerGun : Weapon
{
    [SerializeField] private float range = 30f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float thickness = 0.01f;
    [SerializeField] private GameObject damageParticlesPrefab;

    public override bool Fire()
    {
        if (base.Fire())
        {
//            Debug.Log("fire " + name + " at: " + Time.time);
            FireRay();
            if (GameManager.Instance.EraManager.CurrentEra == 0) GameManager.Instance.AudioManager.playSFX("ShootLaser1");
            else if (GameManager.Instance.EraManager.CurrentEra == 1) GameManager.Instance.AudioManager.playSFX("ShootLaser2");
            else if (GameManager.Instance.EraManager.CurrentEra == 2) GameManager.Instance.AudioManager.playSFX("ShootLaser3");
            return true;
        }
        return false;
    }

    private void FireRay()
    {
        RaycastHit2D hit;
        Vector2 endPoint = shootRef.position + shootRef.up * range;

        hit = Physics2D.CircleCast(shootRef.position, thickness, shootRef.up, range);

        if (hit)
        {
//            Debug.Log("bullet hit " + hit.transform.name + " at: " + Time.time);
            endPoint = hit.point;

            Vulnerable vulnerable = hit.transform.root.GetComponent<Vulnerable>();

            if (vulnerable)
            {
                vulnerable.TakeDamage(damage, null);
                if (damageParticlesPrefab)
                {
                    Instantiate(damageParticlesPrefab, vulnerable.transform.position, Quaternion.identity);
                    ParticleSystem ps = damageParticlesPrefab.GetComponent<ParticleSystem>();
                    if (ps) ps.Play();
                }
            }
        }

        GameManager.Instance.TracerHelper.ShowTracer(shootRef.position, endPoint);
    }
}
