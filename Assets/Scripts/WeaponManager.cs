using UnityEngine;
using System.Collections;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private Weapon[] weapons;
    public Weapon[] Weapons { get { return weapons; } }

    void Start()
    {
        ActivateAll();
    }

    public void ActivateWeapon(int weapon)
    {
        weapons[weapon].Activate();
    }
    
    public void FireWeapon(int weapon)
    {
        weapons[weapon].Fire();
    }

    private void ActivateAll()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].Activate();
        }
    }
}
