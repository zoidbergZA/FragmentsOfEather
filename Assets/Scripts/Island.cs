using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpringJoint2D))]
public class Island : MonoBehaviour
{
    public delegate void IslandHandler();
    public delegate void TechHandler(int amount);

    public static event IslandHandler IslandSpawned;
    public static event TechHandler TechnologyScored;

    [SerializeField] private int influenceValue = 1;
    [SerializeField] private int technologyValue = 1;

    private SpringJoint2D spring;

    public Player Owner { get; set; } 
    public int InfluenceValue {get { return influenceValue; } }
    public int TechnologyValue { get { return technologyValue; } }
    public bool IsConnected { get; private set; }

    public void Awake()
    {
        spring = GetComponent<SpringJoint2D>();
        spring.enabled = false;

        if (IslandSpawned != null)
            IslandSpawned();
    }

    void OnEnable()
    {
        TechnologyManager.NewTechnologyReached += OnNewTechnologyReached;
		EraManager.NewEra += OnEraReached;
    }

    void OnDisable()
    {
        TechnologyManager.NewTechnologyReached -= OnNewTechnologyReached;
		EraManager.NewEra -= OnEraReached;
	}

//    public override void Update()
//    {
//        base.Update();
//
//        TakeDamage(2f*Time.deltaTime, null);
//    }

    public void SetOwner(Player newOwner)
    {
        if (IsConnected)
            return;

        Owner = newOwner;

		ChangeColor ();
    }

    public void ConnectToMainland()
    {
        if (IsConnected)
            return;

        IsConnected = true;

        spring.connectedBody = GameManager.Instance.Mainland.GetComponent<Rigidbody2D>();
        spring.distance = GameManager.Instance.Mainland.Safezone.Range - 12f;
        spring.enabled = true;

        if (Owner)
        {
            GameManager.Instance.ScoreInfluence(Owner, InfluenceValue);
            GameManager.Instance.Hud.ShowInfluenceText(Owner.Ship.transform, "+" + InfluenceValue, Owner.color);
        }

        GameManager.Instance.TechnologyManager.GainTechnologyPoints(TechnologyValue);
        GameManager.Instance.Hud.ShowTechText(GameManager.Instance.Mainland.transform, "+" + TechnologyValue);

        if (TechnologyScored != null)
            TechnologyScored(TechnologyValue);
    }
    
    private void Revert()
    {
        if (Owner)
            GameManager.Instance.LoseInfluence(Owner, InfluenceValue);
        GameManager.Instance.TechnologyManager.LoseTechnologyPoints(TechnologyValue);
    }

	private void ChangeColor()
	{
		if (!Owner)
			return;

		Renderer[] rends = GetComponentsInChildren<Renderer> ();
		for (int i = 0; i < rends.Length; i++)
		{
			rends[i].material.color = Owner.color;
		}
		//        GetComponentInChildren<Renderer>().material.color = Owner.color;
	}

	private void OnEraReached(int era)
	{
		ChangeColor ();
	}

    private void OnNewTechnologyReached(int newEra)
    {
        if (IsConnected)
            Destroy(gameObject);
    }
}
