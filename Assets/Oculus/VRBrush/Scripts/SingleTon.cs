using UnityEngine;

// 싱글톤 : 씬에서 하나의 객체로만 사용될경우 이클래쓰를 상속받아서 사용
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
