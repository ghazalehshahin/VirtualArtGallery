using UnityEngine;
using UnityEngine.Events;

public class ButtonHandler : MonoBehaviour
{
    public UnityEvent ButtonPressed;
    public UnityEvent ButtonReleased;
    
    private bool lastButtonState;
    private bool buttonEventReady;
    private int buttonDebounceThreshold;
    private int buttonDebouceCounter;
    private bool isButtonFlipped;

    private void Start()
    {
        buttonDebouceCounter = 0;
        lastButtonState = false;
        ButtonPressed ??= new UnityEvent();
        ButtonReleased ??= new UnityEvent();
    }

    public void SetButtonState(bool buttonState)
    {
        isButtonFlipped = buttonState;
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

    public void DoButton(float buttonSensor)
    {
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
