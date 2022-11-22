using Unity.Mathematics;
using UnityEngine;
using UnityEngine.VFX;

namespace Enemies
{
    public class VFXOrientation : MonoBehaviour
    {
        private GameObject _player;
        void Start()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
        }

        public void PlayVFX()
        {
            GetComponentInChildren<VisualEffect>().Play();
        }

        // Update is called once per frame
        void Update()
        {
            transform.LookAt(_player.transform);
        }
    }
}
