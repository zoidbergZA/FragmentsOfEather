using System;
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
    public enum GameStates
    {
        PREGAME,
        RUNNING,
        POSTGAME
    }

    private static GameManager _instance;

    static public GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(GameManager)) as GameManager;

                if (_instance == null)
                {
                    throw new Exception("no GameManager!");
                }
            }
            return _instance;
        }
    }

    public Level level;
    public Player player1;
    public Player player2;
    public PlayerShip startShipPrefab;
    
    //global prefabs
    public Healthbar HealthbarPrefab;

    [SerializeField] private float introTime = 20f;
    [SerializeField] private bool showHealthbars;

    private ConfigLoader configLoader;
    private float introTimer;

    public bool IsPaused { get; private set; }
    public bool ShowHealthbars { get { return showHealthbars; } }
    public GameStates GameState { get; private set; }
    public EraManager EraManager { get; private set; }
    public TechnologyManager TechnologyManager { get; private set; }
    public TracerHelper TracerHelper { get; private set; }
    public Hud Hud { get; private set; }
    public LevelSettings LevelSettings { get; private set; }
    public Mainland Mainland { get; private set; }
    public Player Leader { get; private set; }
    public AudioManager AudioManager { get; private set; }
    public float IntroProgress { get { return 1f - introTimer / introTime;} }
    public float GameStartedAt { get; private set; }
    public float LevelProgress
    {
        get
        {
            if (!LevelSettings.IsLoaded)
                return 0;
            return 1f - ((level.levelTime - Time.time + GameStartedAt)/level.levelTime);
        }
    }

    void Awake()
    {
        configLoader = GetComponent<ConfigLoader>();
        EraManager = GetComponent<EraManager>();
        TracerHelper = GetComponent<TracerHelper>();
        TechnologyManager = GetComponent<TechnologyManager>();
        Hud = FindObjectOfType<Hud>();
        LevelSettings = GetComponent<LevelSettings>();
        Mainland = FindObjectOfType<Mainland>();
        AudioManager = GetComponent<AudioManager>();

        introTimer = introTime;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!IsPaused)
                Pause();
            else
                Unpause();
        }

        if (GameState == GameStates.PREGAME)
        {
            if (Input.anyKeyDown)
            {
                StartRound();
                return;
            }

            if (introTimer > 0)
            {
                introTimer -= Time.deltaTime;
            }
            else
            {
                StartRound();
            }
        }

        if (Mainland.LevelTimeRemained <= 0)
        {
            EndRound(true);
        }
    }

    public void StartRound()
    {
        if (GameState != GameStates.PREGAME) // or || !LevelSettings.IsLoaded
            return;

//        Debug.Log("start round " + Time.time);
        GameStartedAt = Time.time;
        GameState = GameStates.RUNNING;

        player1.SpawnShip(startShipPrefab);
        player2.SpawnShip(startShipPrefab);

//        Hud.ShowToast("The game has started!");
    }

    public void Pause()
    {
        if (IsPaused)
            return;

        IsPaused = true;
        Time.timeScale = 0;
        Hud.ShowPause(true);
    }

    public void Unpause()
    {
        if (!IsPaused)
            return;

        IsPaused = false;
        Time.timeScale = 1;
        Hud.ShowPause(false);
    }

    public void EndRound(bool success)
    {
        Hud.ShowGameOver();
        Time.timeScale = 0.5f;
        if(success)
        {
            GameManager.Instance.AudioManager.playSFX("PlatherialWinningSound");
            GameManager.Instance.AudioManager.stopBGMusic();
        }
        else
        {
            GameManager.Instance.AudioManager.playSFX("GameOverSound");
            GameManager.Instance.AudioManager.stopBGMusic();
        }
        
    }

	public void Quit()
	{
		Application.Quit ();
	}

    public void ScoreInfluence(Player player, int amount)
    {
        player.AddInfluence(amount);
        
        if (!Leader)
            HandleNewLeader(player);
        else 
            CheckLeader(player);
    }

    public void LoseInfluence(Player player, int amount)
    {
        player.RemoveInfluence(amount);
        CheckLeader(player);
    }

    private void CheckLeader(Player updatedPlayer)
    {
        if(!Leader)
        {
            HandleNewLeader(updatedPlayer);
            return;
        }
        if (Leader != updatedPlayer)
        {
            if (updatedPlayer.Influence >= Leader.Influence)
                HandleNewLeader(updatedPlayer);
        }
    }

    private void HandleNewLeader(Player newLeader)
    {
        Leader = newLeader;
        Mainland.SetColor(newLeader.color);
    }
}
