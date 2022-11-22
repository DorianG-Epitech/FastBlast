using System.Collections;
using UnityEngine;

namespace Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] GameObject enemyPrefab = default;
        [SerializeField] int nbToSpawn = 1;
        [SerializeField] float timeInSecondsDelayBetweenSpawns;
        private GameObject _enemy = default;
        private Material _enemyMaterial = default;
        private float _xPos = default;
        private float _zPos = default;
        private int _enemyCount = default;
        private MeshRenderer[] _childrenMeshRenderers;
        private static readonly int OutlineColor = Shader.PropertyToID("Color_0e7423a3292b4cd4a9ccda3a11925654");
        private static readonly int BaseColor = Shader.PropertyToID("Color_1dcdbeec17ea46a1b1f02ecb2a7a6a0c");

        // Start is called before the first frame update
        void Start()
        {
            _childrenMeshRenderers = GetComponentsInChildren<MeshRenderer>();
            StartCoroutine(EnemyDrop());
        }

        IEnumerator EnemyDrop()
        {
            while (true)
            {
                Color outlineColorRng = Color.HSVToRGB(Random.value, 1, 0.35f); 
                Color baseColorRng = Color.HSVToRGB(Random.value, 1, 0.35f);
                _xPos = Random.Range(-10, 10);
                _zPos = Random.Range(-10, 10);
                _enemy = Instantiate(
                    enemyPrefab, 
                    new Vector3(transform.position.x + _xPos, 5, transform.position.z + _zPos), 
                    Quaternion.identity
                );
                _enemy.GetComponent<EnemyEntity>().maxHealth *= GameManager.I.difficultyScale;
                _enemy.GetComponent<EnemyEntity>().moveSpeed *= GameManager.I.difficultyScale;
                _enemy.GetComponent<EnemyEntity>().weaponConfiguration.AttackDamage *= GameManager.I.difficultyScale;
                _enemy.GetComponent<EnemyEntity>().weaponConfiguration.AttackRange *= GameManager.I.difficultyScale;
                _enemy.GetComponent<EnemyEntity>().weaponConfiguration.AttackRate /= GameManager.I.difficultyScale;
                _enemy.GetComponent<EnemyEntity>().weaponConfiguration.LifeSteal *= GameManager.I.difficultyScale;
                _childrenMeshRenderers = _enemy.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer rend in _childrenMeshRenderers)
                {
                    rend.material.SetColor(OutlineColor, outlineColorRng);
                    rend.material.SetColor(BaseColor, baseColorRng);
                }
                yield return new WaitForSeconds(timeInSecondsDelayBetweenSpawns * GameManager.I.difficultyScale);
            }
        }
    }
}
