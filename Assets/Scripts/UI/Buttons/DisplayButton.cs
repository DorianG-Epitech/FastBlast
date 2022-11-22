using UI.Displays;

namespace UI.Buttons
{
    public class DisplayButton : IButton
    {
        public IDisplay display;

        public override void OnMouseClick()
        {
            base.OnMouseClick();
            display.SetDisplay();
            display.gameObject.SetActive(true);
        }
    }
}
