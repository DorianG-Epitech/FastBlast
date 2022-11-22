namespace UI.Displays
{
    public class SettingsMenu : IDisplay
    {
        public override void SetDisplay()
        {
            base.SetDisplay();
            gameObject.SetActive(true);
        }

        public override void ResetDisplay()
        {
            base.ResetDisplay();
            gameObject.SetActive(false);
        }
    }
}
