using UnityEngine;
using System.Collections;

public class ComputerUpgrade : Technology
{
    public static event TechHandler ComputerUpgraded;

    [SerializeField] private float safezoneIncreasePercentage = 25f;
    [SerializeField] private ShipProperties newShipProperties;

    public override void DoUpgrade()
    {
        base.DoUpgrade();

        if (ComputerUpgraded != null)
            ComputerUpgraded();

        HandleShipUpgrades(GameManager.Instance.player1.Ship);
        HandleShipUpgrades(GameManager.Instance.player2.Ship);
        GameManager.Instance.Mainland.Safezone.IncreaseSize(GameManager.Instance.Mainland.Safezone.Range * safezoneIncreasePercentage / 100f, 1 + safezoneIncreasePercentage/100f);
        GameManager.Instance.AudioManager.playSFX("ComputerBeepSound");
    }

    private void HandleShipUpgrades(PlayerShip playerShip)
    {
        playerShip.shipProperties = newShipProperties;
    }
}
