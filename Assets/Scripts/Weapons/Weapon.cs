using UnityEngine;
using System.Collections;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected Transform shootRef;
    [SerializeField] private float cooldown = 0.3f;
    [SerializeField] private PlayerShip playerShip;

    private float nextFire;

    public bool IsActivated { get; private set; }
    public bool IsReady
    {
        get
        {
            if (Time.time <= nextFire || !IsActivated || playerShip.IsTwirling)
                return false;
            return true;
        }
    }

    public void Awake()
    {
        playerShip = transform.root.GetComponent<PlayerShip>();
    }

    public void Activate()
    {
        if (IsActivated)
            return;

        IsActivated = true;
    }

    public virtual bool Fire()
    {
        if (!IsReady)
            return false;

        nextFire = Time.time + cooldown;

        return true;
    }
}
