using Common.Data.Scenes;
using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scenes.InGame
{
    public class InGameManager : MonoBehaviour
    {
        public float DelayBeforeEndScene = 2f;

        public CanvasGroup EndGameFadeCanvasGroup;

        public bool GameIsEnding { get; private set; }

        private InGameTimer timer;

        private void Awake()
        {
            timer = GetComponent<InGameTimer>();

            EventManager.AddListener<GameOver>(OnGameOver);
        }

        // Start is called before the first frame update
        private void Start()
        {
            timer.StartTimer();
        }

        private async void OnGameOver(GameOver evt)
        {
            if (!GameIsEnding)
            {
                GameIsEnding = true;

                SceneKind nextScene = evt switch
                {
                    GameOver(true) => SceneKind.WinScene,
                    GameOver(false) => SceneKind.LoseScene,
                };

                switch(nextScene)
                {
                    case SceneKind.WinScene:
                        timer.StopTimer();
                        DataManager.Instance.InGameWinTime = timer.CurrentInGameTime;
                        Debug.Log($"Win!");
                        break;
                    case SceneKind.LoseScene:
                        timer.ResetTimer();
                        Debug.Log("Lose...");
                        break;
                }

                await FadeAndGoToNextScene(nextScene);
            
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                GameIsEnding = false;
            }
        }

        private async UniTask FadeAndGoToNextScene(SceneKind sceneToLoad)
        {
            EndGameFadeCanvasGroup.gameObject.SetActive(true);

            var fadeDuration = DelayBeforeEndScene;
            var elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                var ratio = Mathf.Clamp01(elapsedTime / fadeDuration);

                EndGameFadeCanvasGroup.alpha = ratio;

                await UniTask.Yield();
                
                elapsedTime += Time.deltaTime;
            }

            SceneManager.LoadScene(sceneToLoad.ToString());
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener<GameOver>(OnGameOver);
        }
    }
}
