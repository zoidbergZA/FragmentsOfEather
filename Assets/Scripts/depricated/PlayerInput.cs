using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour
{
    public enum InputTypes
    {
        KEYBOARD,
        XBOX360
    }

    [SerializeField] private InputTypes inputType;

    public InputTypes InputType { get { return inputType; } }
    public float PowerInput { get; private set; }
    public float TurnInput { get; private set; }

    void Update()
    {
        switch (InputType)
        {
            case InputTypes.KEYBOARD:
                PowerInput = Input.GetAxis("Vertical");
                TurnInput = Input.GetAxis("Horizontal");
                break;
        }
    }
}