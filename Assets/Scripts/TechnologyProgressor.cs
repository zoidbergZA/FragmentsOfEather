using UnityEngine;
using System.Collections;

public class TechnologyProgressor : MonoBehaviour
{
    [SerializeField] private GameObject[] techModels;

    void Start()
    {
        SetModel(GameManager.Instance.TechnologyManager.NextTechnologyLevel - 1);
    }

    void OnEnable()
    {
        TechnologyManager.NewTechnologyReached += OnNewTechnologyReached;
    }

    void OnDisable()
    {
        TechnologyManager.NewTechnologyReached -= OnNewTechnologyReached;
    }

    public void SetModel(int techLevel)
    {
        for (int i = 0; i < techModels.Length; i++)
        {
            if (i == techLevel + 1)
                techModels[i].SetActive(true);
            else
                techModels[i].SetActive(false);
        }
    }

    private void OnNewTechnologyReached(int techLevel)
    {
//        Debug.Log(name + " upgraded to tech level " + techLevel + " at: " + Time.time);
        SetModel(techLevel);

		PlayerShip playerShip = GetComponent<PlayerShip> ();

		if (playerShip)
		{
			playerShip.UpdateProperties ();
		}
    }
}
