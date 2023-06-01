using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Scenes.InGame.Player
{
    public class StaminaLogic : MonoBehaviour
    {
        public float StaminaConsumptionRate  = 0.20f;

        public float StaminaRecoveryRate  = 0.12f;

        public float CurrentStamina { get; private set; }

        private InGameInputManager inputManager;
        
        private void Awake()
        {
            inputManager = GameObject.FindObjectOfType<InGameInputManager>();

            CurrentStamina = 1f;
        }

        private async void Start()
        {
            await StaminaTransition(this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow();
        }

        private async UniTask StaminaTransition(CancellationToken token)
        {
            while (true)
            {
                // スプリントをしていない間は、スタミナが全回復するまでスタミナを回復する
                while (!inputManager.GetSprintInput() && CurrentStamina < 1f)
                {
                    CurrentStamina = Mathf.Clamp01(CurrentStamina + StaminaRecoveryRate * Time.deltaTime);
                    await UniTask.Yield(token);
                }

                // スプリントをするまで待機
                await UniTask.WaitUntil(() => inputManager.GetSprintInput(), cancellationToken: token);

                // スプリントをしている間は、スタミナが無くなるまでスタミナを減らす
                while (inputManager.GetSprintInput() && CurrentStamina > 0f)
                {
                    CurrentStamina = Mathf.Clamp01(CurrentStamina - StaminaConsumptionRate * Time.deltaTime);
                    await UniTask.Yield(token);
                }

                // スプリントをやめるまで待機
                await UniTask.WaitUntil(() => !inputManager.GetSprintInput(), cancellationToken: token);
            }
        }
    }
}
