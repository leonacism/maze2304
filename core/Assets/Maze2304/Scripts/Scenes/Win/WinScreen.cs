using Core;
using UnityEngine;
using TMPro;

namespace Scenes.Win
{
    public class WinScreen : MonoBehaviour
    {
        public TextMeshProUGUI WinTimeText;

        void Start()
        {
            var totalSeconds = (int)DataManager.Instance.InGameWinTime;

            int hours = totalSeconds / 3600;
            int minutes = (totalSeconds % 3600) / 60;
            int seconds = totalSeconds % 60;

            WinTimeText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
        }
    }
}
