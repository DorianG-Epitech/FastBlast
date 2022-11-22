using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputButton : MonoBehaviour
{
    public void OnInputPressed(Button button, string action)
    {
        button.GetComponentInChildren<TextMeshPro>().text = "Press any key...";
    }
}
