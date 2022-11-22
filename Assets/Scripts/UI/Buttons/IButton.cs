using UnityEngine;

namespace UI.Buttons
{
    public class IButton : MonoBehaviour
    {
        public AudioClip mouseEnterSound;
        public AudioClip mouseExitSound;
        public AudioClip mouseClickSound;

        public virtual void Setup()
        {}

        public virtual void OnMouseEnter()
        {}

        public virtual void OnMouseExit()
        {}

        public virtual void OnMouseClick()
        {}
    }
}
