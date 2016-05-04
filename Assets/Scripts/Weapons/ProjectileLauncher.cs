using UnityEngine;
using System.Collections;

public class ProjectileLauncher : Weapon
{
    [SerializeField] private GameObject projectilePrefab;

    public override bool Fire()
    {
        if (base.Fire())
        {
            LaunchProjectile();
            GameManager.Instance.AudioManager.playSFX("BomDropExplosion");
            return true;
        }
        return false;
    }

    private void LaunchProjectile()
    {
        Instantiate(projectilePrefab, shootRef.position, shootRef.rotation);
    }
}
