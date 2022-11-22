using Player;
using Sound;
using UnityEngine;

namespace Items
{
    public class GroundItem : MonoBehaviour
    {
        public EquippableItem itemToAdd;

        public LayerMask groundLayer;
        public float degreesPerSecond = 15.0f;
        public float amplitude = 0.5f;
        public float frequency = 1f;
 
        // Position Storage Variables
        Vector3 posOffset = new Vector3 ();
        Vector3 tempPos = new Vector3 ();
 
        void Start ()
        {
            GetGroundDistance();
            posOffset = transform.position;
            GetComponent<Outline>().enabled = true;
        }
     
        void Update ()
        {
            transform.Rotate(new Vector3(0f, Time.deltaTime * degreesPerSecond, 0f), Space.World);
 
            tempPos = posOffset;
            tempPos.y += Mathf.Sin (Time.fixedTime * Mathf.PI * frequency) * amplitude;
 
            transform.position = tempPos;
        }

        void GetGroundDistance()
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position, Vector3.down, out hit, 20, groundLayer.value)) {
                if (hit.distance > 2) {
                    transform.position = new Vector3(
                        transform.position.x,
                        hit.transform.position.y + 1.5f,
                        transform.position.z
                    );
                }
            }
        }

        public void Interact()
        {
            //Ajouter l'objet à l'inventaire et ajouter les buffs de l'objet
            GameManager.I.Player.AddItem(itemToAdd);
            AudioManager.PlayPickupSound();
            Destroy(gameObject);
        }
    }
}
