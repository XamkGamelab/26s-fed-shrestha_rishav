using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : Component
{
    [SerializeField] protected bool _wantToDestroyOnLoad;

    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<T>();

                if (_instance == null)
                {
                    Debug.LogError(
                        $"{typeof(T).Name} is missing from the scene!"
                    );
                }
            }

            return _instance;
        }
    }

    public virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this as T;

        if (!_wantToDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}