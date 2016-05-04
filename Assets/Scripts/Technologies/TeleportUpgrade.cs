using UnityEngine;
using System.Collections;

public class TeleportUpgrade : Technology
{
    public static event TechHandler TeleportUpgraded;

    [SerializeField] private float mainlandSpeedIncrease = 15f;
    [SerializeField] private ShipProperties newShipProperties;

    public override void DoUpgrade()
    {
        base.DoUpgrade();

        if (TeleportUpgraded != null)
            TeleportUpgraded();

        HandleShipUpgrades(GameManager.Instance.player1.Ship);
        HandleShipUpgrades(GameManager.Instance.player2.Ship);

        GameManager.Instance.Mainland.IncreaseSpeed(mainlandSpeedIncrease);
        GameManager.Instance.AudioManager.playSFX("TeleportObjective");
    }

    private void HandleShipUpgrades(PlayerShip playerShip)
    {
        playerShip.shipProperties = newShipProperties;
        playerShip.Harpoon.ActivateTeleporter();
    }
}
