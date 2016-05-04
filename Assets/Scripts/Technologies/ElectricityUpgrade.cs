using UnityEngine;
using System.Collections;

public class ElectricityUpgrade : Technology
{
    public static event TechHandler ElectricityUpgraded;

    [SerializeField] private float playerRegenIncrease = 1f;
    [SerializeField] private float mainlandRegenIncrease = 5f;
    [SerializeField] private ShipProperties newShipProperties;

    public override void DoUpgrade()
    {
        base.DoUpgrade();

        HandleShipUpgrades(GameManager.Instance.player1.Ship);
        HandleShipUpgrades(GameManager.Instance.player2.Ship);
        GameManager.Instance.Mainland.IncreaseRegeneration(mainlandRegenIncrease);
        GameManager.Instance.AudioManager.playSFX("ElectricityObjectiveAchieved");

        if (ElectricityUpgraded != null)
            ElectricityUpgraded();
    }

    private void HandleShipUpgrades(PlayerShip playerShip)
    {
        playerShip.shipProperties = newShipProperties;
        playerShip.IncreaseRegeneration(playerRegenIncrease);
    }
}
