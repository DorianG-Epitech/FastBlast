using UnityEngine;

namespace Items
{
    public class Item : ScriptableObject
    {
        public string itemName;
        public int id;
        public string description;

        public virtual void Interact()
        {}
    }
}
