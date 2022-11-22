using UnityEngine;
using UnityEngine.UI;
using Items;

namespace Enemies
{
    public class EnemyEntity : FightingEntity
    {
        private Canvas _healthCanvas;
        private Slider _healthBar;
        private void Start()
        {
            Init();
            _healthCanvas = GetComponentInChildren<Canvas>(true);
            _healthBar = GetComponentInChildren<Slider>(true);
            foreach (IKFootSolver solver in GetComponentsInChildren<IKFootSolver>())
                solver.speed = moveSpeed;
        }

        void Update()
        {
            HealthBarHandler();
        }

        void HealthBarHandler()
        {
            if (currentHealth < maxHealth)
                _healthCanvas.gameObject.SetActive(true);
            else
                return;
            _healthBar.value = currentHealth / maxHealth;
        }

        public override void Die()
        {
            GameManager.I.OnKill();
            if (Random.Range(0, 100) > 80) {
                Instantiate(ItemsManager.instance.GetRandomItem(Items.EquippableItem.Rarity.COMMON), transform.position, Quaternion.identity);
            }
            base.Die();
        }
    }
}
