using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;

namespace UI
{
    public class TutorialManager : MonoBehaviour
    {
        enum Step
        {
            Moving,
            Sprinting,
            Jumping,
            Crouching,
            WallRunning,
            Shooting,
            BulletTiming,
            End,
        }
        private TextMeshProUGUI _tutorialText;
        public GameObject player;
        private PlayerInputs _inputs;
        private Step _currentStep = Step.Moving;
        
        void Start()
        {
            _inputs = player.GetComponent<PlayerInputs>();
            _tutorialText = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void MovingStep()
        {
            _tutorialText.text = "Welcome to the tutorial !\nUse <color=#dbdb3d>W A S D</color> to move around";
            if (_inputs.move.x != 0 || _inputs.move.y != 0)
                _currentStep = Step.Sprinting;
        }
        
        private void SprintingStep()
        {
            _tutorialText.text = "Good job !\nNow hold <color=#dbdb3d>SHIFT</color> while moving to Sprint!";
            if (_inputs.sprint)
                _currentStep = Step.Jumping;
        }
        
        private void JumpingStep()
        {
            _tutorialText.text = "Nice !\nYou can also Jump with <color=#dbdb3d>SPACEBAR</color>";
            if (_inputs.jump)
                _currentStep = Step.Crouching;
        }
        
        private void CrouchingStep()
        {
            _tutorialText.text = "Hold <color=#dbdb3d>CTRL</color>\nto Crouch";
            if (_inputs.crouch)
                _currentStep = Step.WallRunning;
        }
        
        private void WallRunStep()
        {
            _tutorialText.text = "Now MOVE, SPRINT and JUMP along a Wall\nto do a <color=#dbdb3d>WALLRUN</color>";
            if (_inputs.gameObject.transform.GetComponent<WallRun>().isWallRunning)
                _currentStep = Step.Shooting;
        }

        private void ShootingStep()
        {
            _tutorialText.text = "Press <color=#dbdb3d>LMB</color> to shoot !";
            if (Input.GetMouseButtonDown(0))
                _currentStep = Step.BulletTiming;
        }
        
        private void BulletTimeStep()
        {
            _tutorialText.text = "Hold <color=#dbdb3d>RMB</color> to slow time !";
            if (Input.GetMouseButtonDown(1))
                _currentStep = Step.End;
        }
        
        private void EndStep()
        {
            _tutorialText.text = "TUTORIAL FINISHED\n" +
                                 "Press <color=#dbdb3d>ESCAPE</color> and press <color=#dbdb3d>MENU</color>\n" +
                                 "to go back to title screen";
        }

        void Update()
        {
            if (_currentStep == Step.Moving)
                MovingStep();
            else if (_currentStep == Step.Sprinting)
                SprintingStep();
            else if (_currentStep == Step.Jumping)
                JumpingStep();
            else if (_currentStep == Step.WallRunning)
                WallRunStep();
            else if (_currentStep == Step.Crouching)
                CrouchingStep();
            else if (_currentStep == Step.Shooting)
                ShootingStep();
            else if (_currentStep == Step.BulletTiming)
                BulletTimeStep();
            else if (_currentStep == Step.End)
                EndStep();
        }
    }
}
