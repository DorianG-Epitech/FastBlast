using Fastblast;
using UnityEngine;
using UnityEngine.UI;
namespace Player
{
    public class DifficultyCount : MonoBehaviour
    {
        private Text text;

        private void Start()
        {
            text = GetComponent<Text>();
        }

        private void Update()
        {
            text.text = "Difficulty scale : " + GameManager.I.difficultyScale;
        }
    }
}
