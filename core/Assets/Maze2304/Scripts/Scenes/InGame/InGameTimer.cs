using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Scenes.InGame
{
    public class InGameTimer : MonoBehaviour
    {
        public float CurrentInGameTime { get; private set; }

        public bool IsRunning { get; private set; }

        private void Update()
        {
            if (IsRunning)
            {
                CurrentInGameTime += Time.deltaTime;
            }
        }

        public void ResetTimer()
        {
            IsRunning = false;
            CurrentInGameTime = 0f;
        }

        public void StartTimer()
        {
            IsRunning = true;
        }

        public void StopTimer()
        {
            IsRunning = false;
        }
    }
}
