using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandOnInput : MonoBehaviour
{
    public InputActionProperty triggerValue;
    public InputActionProperty gripValue;

    public Animator handAnimator;

    private void Update()
    {
        if (triggerValue.action.TryReadValue(out float trigger))
        {
            handAnimator.SetFloat("Trigger", trigger);
        }

        if (gripValue.action.TryReadValue(out float grip))
        {
            handAnimator.SetFloat("Grip", grip);
        }
    }
}
