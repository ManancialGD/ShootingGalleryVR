using UnityEngine;

/// <summary>
/// A generic Singleton pattern implementation for Unity MonoBehaviour classes.
/// This implementation is scene-specific and does not use DontDestroyOnLoad,
/// making it suitable for projects with initialization scenes and scene-based architecture.
/// </summary>
/// <typeparam name="T">The type of the class inheriting from Singleton</typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    /// <summary>
    /// The static instance of the singleton.
    /// This is accessible via the Instance property.
    /// </summary>
    private static T instance;

    /// <summary>
    /// Flag to track if the application is quitting.
    /// Prevents creating new instances during application shutdown.
    /// </summary>
    private static bool isApplicationQuitting = false;

    /// <summary>
    /// Public accessor for the singleton instance.
    /// Implements lazy initialization - creates instance if it doesn't exist.
    /// </summary>
    /// <value>The singleton instance of type T</value>
    public static T Instance
    {
        get
        {
            // Check if application is quitting
            if (isApplicationQuitting)
                return null;


            // Check if instance already exists
            if (instance == null)
            {
                // Try to find existing instance in scene
                instance = FindAnyObjectByType<T>();

                // If no instance found, create new one
                if (instance == null)
                {
                    Debug.LogWarning($"[Singleton] No instance of '{typeof(T)}' found.");
                }
            }
            return instance;
        }
    }

    /// <summary>
    /// Unity's Awake method called when the script instance is being loaded.
    /// Handles singleton initialization and duplicate prevention.
    /// </summary>
    protected virtual void Awake()
    {
        // Check if this is the first instance
        if (instance == null)
        {
            // Set this as the singleton instance
            instance = this as T;
        }
        else
        {
            // If another instance exists and it's not this one, destroy this duplicate
            if (instance != this)
            {
                Debug.LogWarning($"[Singleton] Multiple instances of '{typeof(T)}' found. Destroying duplicate.");
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// Called when the application is quitting.
    /// Sets the quitting flag to prevent creating new instances during shutdown.
    /// </summary>
    protected virtual void OnApplicationQuit()
    {
        // Step Set quitting flag to true
        isApplicationQuitting = true;
    }

    /// <summary>
    /// Called when the singleton instance is destroyed.
    /// Cleans up the static reference to prevent ghost references.
    /// </summary>
    protected virtual void OnDestroy()
    {
        // Check if the destroyed object is the current instance
        if (instance == this)
        {
            // Clear the instance reference
            instance = null;
        }
    }
}
