using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Mainland : Vulnerable
{
    public Transform spawnRef1;
    public Transform spawnRef2;
    public Vector2[] path;

    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private GameObject deathParticlesPrefab;

    private Vector2 nextPoint;
    private int counter;
    private float totalDistance;
    private float mainlandSpeed;
    private float startedAt;
    private GameObject ghost;

    public Safezone Safezone { get; private set; }

    public override void Awake()
    {
        base.Awake();

        Safezone = GetComponentInChildren<Safezone>();
    }

    public float TotalInfluence
    {
        get { return GameManager.Instance.player1.Influence + GameManager.Instance.player2.Influence; }
    }

    public float LevelTimeRemained
    {
        get { return GameManager.Instance.level.levelTime - Time.time + GameManager.Instance.GameStartedAt; } //todo: change back from level.leveltime to levelSettings.LevelTime
    }

    private bool reachedPoint
    {
        get { return (nextPoint - (Vector2)transform.position).magnitude <= 0.5f; }
    }

    void OnEnable()
    {
        TechnologyManager.NewTechnologyReached += OnNewTechnologyReached;
    }

    void OnDisable()
    {
        TechnologyManager.NewTechnologyReached -= OnNewTechnologyReached;
    }

    public override void Start()
    {
        base.Start();

		Invulnerable = true;
        setUpPath();
    }

    public override void Update()
    {
        if (GameManager.Instance.GameState != GameManager.GameStates.RUNNING)
            return;

        base.Update();
        
        if(path != null) followPath();
    }

    public override void Die()
    {
        GameManager.Instance.EndRound(false);
        Instantiate(deathParticlesPrefab, this.transform.position, Quaternion.identity);
        ParticleSystem ps = deathParticlesPrefab.GetComponent<ParticleSystem>();
        if (ps) ps.Play();
    }

    public void IncreaseSpeed(float amount)
    {
        mainlandSpeed += amount;
    }

    public void SetColor(Color newColor)
    {
        GetComponentInChildren<Renderer>().material.color = newColor;
    }

    private void setUpPath()
    {
        if (path != null && path.Length > 0)
        {
            counter = 0;
            totalDistance = getTotalDistance();
            mainlandSpeed = getSpeed();
            nextPoint = path[counter];

            //place ghost at finish
            ghost = (GameObject)Instantiate(ghostPrefab, path[path.Length - 1], Quaternion.identity);
        }
    }

    private float getSpeed()
    {
        if (GameManager.Instance.level.levelTime != 0)
            return totalDistance/GameManager.Instance.level.levelTime;
        else
        {
            return totalDistance;
        }
    }

    private float getTotalDistance()
    {
        float result = ((Vector3)path[0] - transform.position).magnitude;
        for (int i = 1; i < path.Length; i++)
        {
            result += (path[i] - path[i - 1]).magnitude;
        }
        return result;
    }

    private void followPath()
    {
        if (reachedPoint)
        {
            counter++;
            if (counter < path.Length)
            {
                nextPoint = path[counter];
            }
            else
            {
                path = null;
                GameManager.Instance.EndRound(true);
                return;
            }
        }
        if (counter < path.Length)
            transform.position = Vector2.MoveTowards(transform.position, (Vector3)nextPoint, mainlandSpeed * Time.deltaTime);
    }

    private void OnNewTechnologyReached(int techLevel)
    {
        if (GameManager.Instance.Leader)
        {
            SetColor(GameManager.Instance.Leader.color);
        }
    }
}

