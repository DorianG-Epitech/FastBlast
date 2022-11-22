using UnityEngine;

namespace UI.Buttons
{
    public class QuitButton : IButton
    {
        public IDisplay display;

        public override void OnMouseClick()
        {
            base.OnMouseClick();
            if (display != null) {
                display.ResetDisplay();
            }
            Application.Quit();
        }
    }
}
