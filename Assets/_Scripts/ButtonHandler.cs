using System;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(EndEffectorManager))]
public class ButtonHandler : MonoBehaviour
{
    public UnityEvent ButtonPressed;
    public UnityEvent ButtonReleased;

    private EndEffectorManager endEffectorManager;
    private bool lastButtonState;
    private bool buttonEventReady;
    private int buttonDebounceThreshold;
    private int buttonDebouceCounter;
    private bool isButtonFlipped;

    private void Awake()
    {
        endEffectorManager = GetComponent<EndEffectorManager>();
    }

    private void Start()
    {
        buttonDebouceCounter = 0;
        lastButtonState = false;
        ButtonPressed ??= new UnityEvent();
        ButtonReleased ??= new UnityEvent();
    }

    private void OnEnable()
    {
        isButtonFlipped = endEffectorManager.GetButtonState();
        endEffectorManager.OnSimulationStep += DoButton;
    }

    private void OnDisable()
    {
        endEffectorManager.OnSimulationStep -= DoButton;
    }

    private void LateUpdate()
    {
        if (buttonEventReady)
        {
            if (lastButtonState) (isButtonFlipped ? ButtonReleased : ButtonPressed).Invoke();
            else (isButtonFlipped ? ButtonPressed : ButtonReleased).Invoke();
            buttonEventReady = false;
        }
    }

    private void DoButton(float[] sensorData)
    {
        float buttonSensor = sensorData[0];
        bool buttonState = buttonSensor > 500;

        if (buttonState != lastButtonState)
        {
            if (buttonDebouceCounter > buttonDebounceThreshold)
            {
                lastButtonState = buttonState;
                buttonEventReady = true;
            }
            else buttonDebouceCounter++;
        }
        else buttonDebouceCounter = 0;
    }
}
