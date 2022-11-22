using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Buttons
{
    public class SceneButton : IButton
    {
        public string sceneName;
        public IDisplay display;

        public override void OnMouseClick()
        {
            base.OnMouseClick();
            if (display != null) {
                display.ResetDisplay();
            }
            SceneManager.LoadScene(sceneName);
        }
    }
}
