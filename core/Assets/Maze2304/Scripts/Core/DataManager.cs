using UnityEngine;

namespace Core
{
    public class DataManager : MonoBehaviour
    {
        static public DataManager Instance { get; private set; }

        public float InGameWinTime { get; set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
