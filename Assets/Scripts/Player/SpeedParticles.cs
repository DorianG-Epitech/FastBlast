using Player;
using UnityEngine;

namespace Fastblast
{
    [RequireComponent(typeof (ParticleSystem))]
    public class SpeedParticles : MonoBehaviour
    {
        public PlayerController PlayerController;
        public int maxEmissionRate = 40;

        private ParticleSystem _particleSystem;

        private void Start()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        private void Update()
        {
            var particleEmission = _particleSystem.emission;
            var newRateOverTime = maxEmissionRate * (PlayerController.GetHorizontalSpeed() / (GameManager.I.Player.moveSpeed * PlayerController.MaxAirSpeedFactor));
            particleEmission.rateOverTime = newRateOverTime;
        }
    }
}