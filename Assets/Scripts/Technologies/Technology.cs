using UnityEngine;
using System.Collections;

public delegate void TechHandler();

public abstract class Technology : MonoBehaviour
{
    [SerializeField] private int steps = 5;

    public bool Unlocked { get; private set; }
    public int Steps { get { return steps; } }

    public virtual void DoUpgrade()
    {
        if (Unlocked)
            return;

        Unlocked = true;

//        Debug.Log(name + " technology unlocked!");
    }
}
