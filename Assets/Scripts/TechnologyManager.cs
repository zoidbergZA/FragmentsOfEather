using UnityEngine;
using System.Collections;

public delegate void NewTechnologyHandler(int newTechLevel);

public class TechnologyManager : MonoBehaviour
{
    public static event NewEraHandler NewTechnologyReached;

    [SerializeField] private Technology[] technologies;

    public int CurrentTechnologyPoints { get; private set; }
    public int NextTechnologyLevel { get; private set; }
    public Technology NexTechnologyUpgrade { get{ return technologies[NextTechnologyLevel]; } }
    public int TechnologyCount { get { return technologies.Length; } }
    public bool IsCompleted { get; private set; }

    public void GainTechnologyPoints(int amount)
    {
        CurrentTechnologyPoints += amount;

        if (CurrentTechnologyPoints >= NexTechnologyUpgrade.Steps)
        {
            HandleUpgrade();
        }
    }

    public void LoseTechnologyPoints(int amount)
    {
        CurrentTechnologyPoints -= amount;

        if (CurrentTechnologyPoints < 0)
            CurrentTechnologyPoints = 0;
    }

    private void HandleUpgrade()
    {
        if (IsCompleted)
            return;

        technologies[NextTechnologyLevel].DoUpgrade();

        if (NewTechnologyReached != null)
            NewTechnologyReached(NextTechnologyLevel);

        if (NextTechnologyLevel >= TechnologyCount - 1)
        {
            IsCompleted = true;
            return;
        }

        NextTechnologyLevel++;
        CurrentTechnologyPoints = 0;
    }
}
