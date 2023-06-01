using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class LoadSceneButton : MonoBehaviour
    {
        public string TargetSceneName = "";

        public void ChangeScene()
        {
            SceneManager.LoadScene(TargetSceneName);
        }
    }
}
