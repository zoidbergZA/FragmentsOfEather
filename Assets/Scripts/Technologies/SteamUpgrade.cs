using UnityEngine;
using System.Collections;

public class SteamUpgrade : Technology
{
    public static event TechHandler SteamUpgraded;

    [SerializeField] private int weaponUnlock;
    [SerializeField] private ShipProperties newShipProperties;
//    [SerializeField] private float maxSpeedIncrease = 10f;
//    [SerializeField] private float powerIncrease = 1000f;
    [SerializeField] private float mainlandSpeedIncrease = 20f;

    public override void DoUpgrade()
    {
        base.DoUpgrade();

        if (SteamUpgraded != null)
            SteamUpgraded();

        HandleShipUpgrades(GameManager.Instance.player1.Ship);
        HandleShipUpgrades(GameManager.Instance.player2.Ship);
        GameManager.Instance.Mainland.IncreaseSpeed(mainlandSpeedIncrease);
        GameManager.Instance.AudioManager.playSFX("Steamsound2");
    }

    private void HandleShipUpgrades(PlayerShip playerShip)
    {
        playerShip.WeaponManager.ActivateWeapon(weaponUnlock);
        playerShip.shipProperties = newShipProperties;
        playerShip.UpdateProperties();
    }
}
