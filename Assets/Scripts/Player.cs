using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public delegate void ScoreInfluenceHandler(int amount);

    public static event ScoreInfluenceHandler InfluenceScored;

    public Color color; //temp

    [SerializeField] private Transform spawnRef;
    [SerializeField] private KeySet keySet;

    public PlayerShip Ship { get; private set; }
    public int Influence { get; private set; }
//    public Vector2 SpawnRef { get { return spawnPosition.position; } }

    void Update()
    {
        if (GameManager.Instance.IsPaused)
            return;

        if (Ship)
        {
            if (!Ship.IsDead)
            {
				Ship.Thrust = Input.GetAxis (keySet.thrustAxis); // + Input.GetAxis(keySet.thrustAxisAlt);
				Ship.Steering = Input.GetAxis(keySet.turnAxis); // + Input.GetAxis(keySet.turnAxisAlt);

                if (Input.GetKeyDown(keySet.fireHarpoon) || Input.GetKeyDown(keySet.fireHarpoonAlt))
                {
                    if(Ship.Harpoon.IsReady) GameManager.Instance.AudioManager.playSFX("HarpoonShoot");
                    Ship.Harpoon.Fire();
                }

                if (Input.GetKeyDown(keySet.ability1) || Input.GetKeyDown(keySet.ability1Alt))
                {
                    Ship.Twirl();
                }

                if (Input.GetKey(keySet.fireWeapon1) || Input.GetKey(keySet.fireWeapon1Alt))
                {
                    Ship.WeaponManager.FireWeapon(0);
                }
                if (Input.GetKey(keySet.fireWeapon2) || Input.GetKey(keySet.fireWeapon2Alt))
                {
                    GameManager.Instance.AudioManager.playSFX("ShootLaser2");
                    Ship.WeaponManager.FireWeapon(1);
                }
                    
            }
        }
    }

    void FixedUpdate()
    {
        // if (Ship)
        // {
            
        // }
    }

    public void SpawnShip(PlayerShip playerShipPrefab)
    {
        if (Ship)
            Destroy(Ship);

        Ship = Instantiate(playerShipPrefab, spawnRef.position, spawnRef.rotation) as PlayerShip;
        Ship.SetOwner(this);
    }

    public void KillShip()
    {
        Ship.gameObject.SetActive(false);

        StartCoroutine(RespawnSequence());
    }

    public void AddInfluence(int amount)
    {
        Influence += amount;

        if (InfluenceScored != null)
            InfluenceScored(amount);
    }

    public void RemoveInfluence(int amount)
    {
        Influence -= amount;
    }

    private IEnumerator RespawnSequence()
    {
        yield return new WaitForSeconds(4f); // change time to respawn time

        Ship.transform.position = spawnRef.position;
        Ship.transform.rotation = spawnRef.rotation;
        Ship.gameObject.SetActive(true);
        Ship.Reset();
    }
}
