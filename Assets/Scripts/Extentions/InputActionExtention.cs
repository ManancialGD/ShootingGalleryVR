using UnityEngine.InputSystem;

public static class InputActionExtension
{
    public static bool TryReadValue<T>(this InputAction action, out T value) where T : struct
    {
        value = default;

        if (action == null || !action.enabled)
            return false;

        try
        {
            value = action.ReadValue<T>();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
