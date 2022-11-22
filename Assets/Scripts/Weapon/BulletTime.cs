using Sound;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ressources.Weapon.Scripts
{
    [RequireComponent(typeof(Animator))]
    public class BulletTime : MonoBehaviour
    {
        public RectTransform BulletTimeBar;
        public float IncreaseSpeed = 0.1f;
        public float DecreaseSpeed = 0.1f;
        public float _remainingTime = 1f;
        public PauseManager pauseManager;
        private Animator _animator;
        private bool _isBulletTime;


        public void BulletTimeInput(InputAction.CallbackContext context)
        {
            bool value = context.ReadValueAsButton();

            if (pauseManager && pauseManager.pauseDisplay.gameObject.activeInHierarchy)
            {
                if (!value)
                {
                    pauseManager.SetBTRelease();
                }
                return;
            }

            if (value && _remainingTime > 0.5f)
                ActivateBulletTime();
            else
                StopBulletTime();
        }

        private void ActivateBulletTime()
        {
            Time.timeScale = 0.3f;
            AudioManager.PlayBulletTimeStartSound();
            _isBulletTime = true;
            _animator.SetBool("BulletTime", true);
        }

        private void StopBulletTime()
        {
            Time.timeScale = 1f;
            AudioManager.PlayBulletTimeEndSound();
            _isBulletTime = false;
            _animator.SetBool("BulletTime", false);
        }

        public void ReleaseBulletTime()
        {
            Time.timeScale = 1f;
            _isBulletTime = false;
            _animator.SetBool("BulletTime", false);
        }

        private void Start()
        {
            pauseManager = GameObject.Find("PauseManager").GetComponent<PauseManager>();
            BulletTimeBar = GameObject.Find("BulletTimeValue").transform as RectTransform;
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            _remainingTime = Mathf.Clamp01(_remainingTime + Time.unscaledDeltaTime * (_isBulletTime ? -DecreaseSpeed : IncreaseSpeed));
            if (_remainingTime == 0) {
                StopBulletTime();
                _remainingTime = 0f;
            }
            BulletTimeBar.sizeDelta = new Vector2(_remainingTime * 500, 0);
        }
    }
}