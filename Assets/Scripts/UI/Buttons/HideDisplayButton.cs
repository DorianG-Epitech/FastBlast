namespace UI.Buttons
{
    public class HideDisplayButton : DisplayButton
    {
        public override void OnMouseClick()
        {
            display.ResetDisplay();
            display.gameObject.SetActive(false);
        }
    }
}
