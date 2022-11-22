using UnityEngine;

namespace Enemies
{
    public class FacingPlayer : MonoBehaviour
    {
        private Transform _player;

        private void Start()
        {
            _player = GameObject.FindWithTag("Player").transform;
        }

        void Update()
        {
            transform.LookAt(_player);
        }
    }
}
