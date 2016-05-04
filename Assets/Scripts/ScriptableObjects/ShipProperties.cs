using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class ShipProperties : ScriptableObject
{
    [SerializeField] private float power = 500f;
	[SerializeField] private float burnRate = 1.3f;
    [SerializeField] private float brakePower = 500f;
    [SerializeField] private float maxSpeed = 15f;
    [SerializeField] private float minSpeed = 15f;
    [SerializeField] private float turnPower = 2000f;
    [SerializeField] private float maxTurnRate = 60f;
    [SerializeField] private float bankPower = 100f;
    [SerializeField] private float mass = 10f;
    [SerializeField] private float linearDrag = 0.3f;
    [SerializeField] private float angularDrag = 0.25f;
    [SerializeField] private float bankAngle = 20f;

    public float Power { get { return power; } }
	public float BurnRate { get { return burnRate; } }
    public float BrakePower { get { return brakePower; } }
    public float MaxSpeed { get { return maxSpeed; } }
    public float MinSpeed { get { return minSpeed; } }
    public float TurnPower { get { return turnPower; } }
    public float MaxTurnRate { get { return maxTurnRate; } }
    public float BankPower { get { return bankPower; } }
    public float Mass { get { return mass; } }
    public float LinearDrag { get { return linearDrag; } }
    public float AngularDrag { get { return angularDrag; } }
    public float BankAngle { get { return bankAngle; } }
}
