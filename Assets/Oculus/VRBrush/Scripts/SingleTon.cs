using UnityEngine;

// Singleton: Inherit and use this class when used as only one object in the scene
public abstract class SingleTon<T> : MonoBehaviour where T : MonoBehaviour
{
    private static GameObject obj;
    private static T _instance;
    public static T instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    if (obj == null) 
                        obj = new GameObject("SingleTon");
                    _instance = obj.AddComponent<T>();

                    DontDestroyOnLoad(obj);
                }
            }
            return _instance;
        }
    }
}
