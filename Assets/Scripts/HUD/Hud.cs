using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ToastMessage
{
    public string message;
    public float displayTime;
    public Sprite icon;
}

public class Hud : MonoBehaviour
{
    [SerializeField] private Text timelefText;
    [SerializeField] private Text influenceText;
    [SerializeField] private Text nextEraText;
    [SerializeField] private Text technologyText;
    [SerializeField] private GameObject toastPanel;
    [SerializeField] private GameObject GameOverPanel;
	[SerializeField] private Text GameOverText;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject techPanel;
    [SerializeField] private Image boundryWarning;
    [SerializeField] private Text toastText;
    [SerializeField] private float playerPanelGap = 400f;
    [SerializeField] private PlayerInfo player1InfoPanel;
    [SerializeField] private PlayerInfo player2InfoPanel;
    [SerializeField] private InfluenceSlider influenceSlider;

    private Director director;
    private List<ToastMessage> toastMessageList = new List<ToastMessage>(); 
//    private float toastTimer;
    
    private float alarmCooldown = 5f;
    private float lastWarnedAt;

    //prefabs
    [SerializeField] private FloatingText influenceTextPrefab;
    [SerializeField] private FloatingText techTextPrefab;

    public RectTransform CanvasRect { get; private set; }
    public float PlayerPanelGap { get { return playerPanelGap; } }

    void Awake()
    {
        PlayerShip.SafezoneTriggered += PlayerShipOnSafezoneTriggered;
        Island.IslandSpawned += IslandOnIslandSpawned;
        Island.TechnologyScored += OnTechnologyScored;
        Harpoon.IslandHooked += OnIslandHooked;
        Player.InfluenceScored += OnInfluenceScored;
        SteamUpgrade.SteamUpgraded += OnSteamUpgraded;
        ElectricityUpgrade.ElectricityUpgraded += OnElectricityUpgraded;
        ComputerUpgrade.ComputerUpgraded += OnComputerUpgraded;
        TeleportUpgrade.TeleportUpgraded += OnTeleportUpgraded;
        Stuka.StukaSpawned += OnStukaSpawned;
        Stuka.StukaKilled += OnStukaKilled;

        CanvasRect = GetComponent<RectTransform>();
        director = FindObjectOfType<Director>();

        InitializePanels();
    }

    void Start()
    {
        lastWarnedAt = Time.time;
    }

    void Update()
    {
        CheckBoundsWarning();
        HandleToast();

        influenceText.text = "player1: " + GameManager.Instance.player1.Influence + " player2: " + GameManager.Instance.player2.Influence;
        timelefText.text = "time left: " + GameManager.Instance.Mainland.LevelTimeRemained;

        int eraProgress = (int)(100*(1-GameManager.Instance.EraManager.NextEraIn/GameManager.Instance.EraManager.EraInterval));
        nextEraText.text = "era: " + GameManager.Instance.EraManager.CurrentEra + " next in: " + (int)GameManager.Instance.EraManager.NextEraIn + " (" + eraProgress + "%)";

        if (GameManager.Instance.TechnologyManager.IsCompleted)
            technologyText.text = "tech completed!";
        else
        {
            string techOverall = "";
            string techProgress = "";

            techProgress = GameManager.Instance.TechnologyManager.CurrentTechnologyPoints + "/" +
                           GameManager.Instance.TechnologyManager.NexTechnologyUpgrade.Steps;
            techOverall = GameManager.Instance.TechnologyManager.NextTechnologyLevel + "/" +
                          (GameManager.Instance.TechnologyManager.TechnologyCount);

            technologyText.text = "next technology: " + techProgress + " (overall " + techOverall + ")";
        }
    }

    public void ShowInfluenceText(Transform target, string text, Color color)
    {
        FloatingText influence = Instantiate(influenceTextPrefab);
        influence.GetComponent<RectTransform>().SetParent(transform.GetComponent<RectTransform>());
        influence.GetComponent<RectTransform>().localScale = Vector3.one;
        influence.SetText(target, text, color);
    }

    public void ShowTechText(Transform target, string text)
    {
        FloatingText floatingText = Instantiate(techTextPrefab);
        floatingText.GetComponent<RectTransform>().SetParent(transform.GetComponent<RectTransform>());
        floatingText.GetComponent<RectTransform>().localScale = Vector3.one;
        floatingText.SetText(target, text, Color.black);
    }

    public void ShowToast(string message, float time = 4f, Sprite icon = null)
    {
        ToastMessage newToastMessage = new ToastMessage();

        newToastMessage.message = message;
        newToastMessage.displayTime = time;
        newToastMessage.icon = icon;

        toastMessageList.Add(newToastMessage);
    }

    public void ShowPause(bool paused)
    {
        if (paused)
        {
            menuPanel.SetActive(true);
            player1InfoPanel.gameObject.SetActive(false);
            player2InfoPanel.gameObject.SetActive(false);
            techPanel.SetActive(false);
            influenceSlider.gameObject.SetActive(false);
        }

        else
        {
            menuPanel.SetActive(false);
            player1InfoPanel.gameObject.SetActive(true);
            player2InfoPanel.gameObject.SetActive(true);
            techPanel.SetActive(true);
            influenceSlider.gameObject.SetActive(true);
        }
    }

    public void ShowGameOver()
    {
		Color winColor = GameManager.Instance.player1.color;
		string winText = "PLAYER 1 WINS!";

		if (GameManager.Instance.Leader == GameManager.Instance.player2) 
		{
			winColor = GameManager.Instance.player2.color;
			winText = "PLAYER 2 WINS!";
		}

		GameOverText.color = winColor;
		GameOverText.text = winText;
        GameOverPanel.SetActive(true);

		player1InfoPanel.gameObject.SetActive (false);
		player2InfoPanel.gameObject.SetActive (false);
	}

    public void OnRestartClick()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    private void CheckBoundsWarning()
    {
        if (director.IsMaxHeight)
        {
            boundryWarning.enabled = true;
            if (Time.time >= lastWarnedAt + alarmCooldown)
            {
                lastWarnedAt = Time.time;
                GameManager.Instance.AudioManager.playSFX("AlarmDamage");
            }
        }
        else
            boundryWarning.enabled = false;
    }

    private void HandleToast()
    {
        if (toastMessageList.Count == 0)
        {
            return;
        }

        else
        {
            if (toastMessageList[0].displayTime > 0)
            {
                toastMessageList[0].displayTime -= Time.deltaTime;
                    toastText.text = toastMessageList[0].message;
//                    //todo: set icon sprite
                    toastPanel.SetActive(true);

            }
            else
            {
                toastMessageList.RemoveAt(0);

                if (toastMessageList.Count == 0)
                {
                    toastPanel.SetActive(false);
                    return;
                }

                toastText.text = toastMessageList[0].message;
                toastPanel.SetActive(true);
                //todo: set icon sprite
            }
        }
    }

    private void InitializePanels()
    {
        menuPanel.SetActive(false);
        toastPanel.SetActive(false);
        GameOverPanel.SetActive(false);

        player1InfoPanel.TargetPlayer = GameManager.Instance.player1;
        player2InfoPanel.TargetPlayer = GameManager.Instance.player2;

//        //clone player1 panek for player 2
//        player2InfoPanel = Instantiate(player1InfoPanel, Vector3.zero, Quaternion.identity) as PlayerInfo;
//        player2InfoPanel.TargetPlayer = GameManager.Instance.player2;
//        player2InfoPanel.RectTransform.SetParent(transform.GetComponent<RectTransform>());
//        player2InfoPanel.RectTransform.localScale = Vector3.one;
//        player2InfoPanel.RectTransform.anchoredPosition = player1InfoPanel.RectTransform.anchoredPosition + Vector2.down*playerPanelGap;

        //set colors
        player1InfoPanel.SetColors(GameManager.Instance.player1.color);
        player2InfoPanel.SetColors(GameManager.Instance.player2.color);
    }

    private void PlayerShipOnSafezoneTriggered(bool enter)
    {
        if (!enter)
        {
            ShowToast("Pilot, be careful! Outside the protection zone you lose energy, return to our land to recharge.");
            PlayerShip.SafezoneTriggered -= PlayerShipOnSafezoneTriggered;
        }
    }

    private void IslandOnIslandSpawned()
    {
        ShowToast("We found usable aether! Press 'A' to drag it to us using your harpoon.");
        Island.IslandSpawned -= IslandOnIslandSpawned;
    }

    private void OnIslandHooked()
    {
        ShowToast("Well done, now bring it back to the mainland!");
        Harpoon.IslandHooked -= OnIslandHooked;
    }

    private void OnInfluenceScored(int amount)
    {
        //todo: add icon
        ShowToast("That's " + amount + " Influence Points! Gather more Influence points than your partner to become king of the new Aethereal World.");
        Player.InfluenceScored -= OnInfluenceScored;
    }

    private void OnTechnologyScored(int amount)
    {
        ShowToast("Aetherial points will help us to upgrade our fleet. Now, keep gathering aether to help us research new technologies");
        Island.TechnologyScored -= OnTechnologyScored;
    }

    private void OnSteamUpgraded()
    {
        ShowToast("We've gathered enough resources for an upgrade! you have cannons, press 'RB' to fire. Engines upgraded to steam, you can pull heavier objects.");
        SteamUpgrade.SteamUpgraded -= OnSteamUpgraded;
    }

    private void OnStukaSpawned()
    {
        ShowToast("Enemy ships detected!! You have to keep them away from our land!");
        Stuka.StukaSpawned -= OnStukaSpawned;
    }

    private void OnElectricityUpgraded()
    {
        ShowToast("Eureka! We invented Electricity! Ramjets should make you faster, and micro-dynamos mean that you won't have to come back to recharge so often.");
        ElectricityUpgrade.ElectricityUpgraded -= OnElectricityUpgraded;
    }

    private void OnComputerUpgraded()
    {
        ShowToast("Computational Cloud! We have equiped the fleet with Smart Bombs! Press the 'X' button to place them and fry those enemies!");
        ComputerUpgrade.ComputerUpgraded -= OnComputerUpgraded;
    }

    private void OnStukaKilled()
    {
        ShowToast("You killed an enemy, you have been rewarded with Influence!");
        Stuka.StukaKilled -= OnStukaKilled;
    }

    private void OnTeleportUpgraded()
    {
        ShowToast("Transporters online! Harpoon upgraded, now you can instantly teleport resources home.");
        TeleportUpgrade.TeleportUpgraded -= OnTeleportUpgraded;
    }
}
