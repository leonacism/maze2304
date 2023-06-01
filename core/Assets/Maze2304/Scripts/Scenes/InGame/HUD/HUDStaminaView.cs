using Cysharp.Threading.Tasks;
using Scenes.InGame.Player;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.InGame.HUD
{
    public class HUDStaminaView : MonoBehaviour
    {
        public string TargetName;

        public float StaminaGreenThreshold = 0.65f;
        public Color StaminaGreenColor = Color.green;

        public float StaminaYellowThreshold = 0.15f;
        public Color StaminaYellowColor = Color.yellow;
        
        public float TransitionRange = 0.05f;
        public Color StaminaRedColor = Color.red;

        public Image staminaGauge;

        private StaminaLogic stamina;

        private float initialWidth;

        private void Awake()
        {
            this.stamina = GameObject.Find(TargetName)?.GetComponent<StaminaLogic>();            
            initialWidth = staminaGauge.rectTransform.rect.width;
        }

        private void Start()
        {
            var token = this.GetCancellationTokenOnDestroy();

            MakeTransparent(token).SuppressCancellationThrow();

            ChangeColor(StaminaYellowColor, StaminaGreenColor, StaminaGreenThreshold, token).SuppressCancellationThrow();

            ChangeColor(StaminaRedColor, StaminaYellowColor, StaminaYellowThreshold, token).SuppressCancellationThrow();
        }

        private void Update()
        {
            staminaGauge.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, stamina.CurrentStamina * initialWidth);
        }

        private async UniTask MakeTransparent(CancellationToken token)
        {
            var transparentColor = new Color(StaminaGreenColor.r, StaminaGreenColor.g, StaminaGreenColor.b, 0f);
            while (true)
            {
                // スタミナが満タンになったら、ゲージを透明に近づける
                while (stamina.CurrentStamina == 1f && staminaGauge.color.a != 0f)
                {
                    staminaGauge.color = Color.Lerp(staminaGauge.color, transparentColor, 0.05f);
                    await UniTask.Yield(token);
                }

                // スタミナが満タンでなくなるまで待機
                await UniTask.WaitUntil(() => stamina.CurrentStamina < 1f, cancellationToken: token);

                // スタミナが満タンでなくなったら、ゲージを不透明に近づける
                while (stamina.CurrentStamina < 1f && staminaGauge.color.a != 1f)
                {
                    staminaGauge.color = Color.Lerp(staminaGauge.color, StaminaGreenColor, 0.05f);
                    await UniTask.Yield(token);
                }

                // スタミナが満タンになるまで待機
                await UniTask.WaitUntil(() => stamina.CurrentStamina == 1f, cancellationToken: token);
            }
        }

        private async UniTask ChangeColor(Color left, Color right, float threshold, CancellationToken token)
        {
            var delta = 0.5f * TransitionRange;
            while (true)
            {
                // 閾値に差し掛かったら、片方の色に近づける
                while (Mathf.Abs(stamina.CurrentStamina - threshold) < delta)
                {
                    staminaGauge.color = Color.Lerp(left, right, normalize(stamina.CurrentStamina - threshold, -delta, delta));
                    await UniTask.Yield(token);
                }

                // スタミナが閾値の一定範囲内に近づくまで待機
                await UniTask.WaitUntil(() => Mathf.Abs(stamina.CurrentStamina - threshold) < delta, cancellationToken: token);
            }
        }

        static private float normalize(float x, float min, float max) => Mathf.Clamp01((x - min) / (max - min));
    }
}
