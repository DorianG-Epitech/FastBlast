using Fastblast;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class KillCount : MonoBehaviour
    {
        private Text text;

        private void Start()
        {
            text = GetComponent<Text>();
        }

        private void Update()
        {
            text.text = "Kills until next difficulty : " + GameManager.I.killCount + "/" + GameManager.I.killCap ;
        }
    }
}
