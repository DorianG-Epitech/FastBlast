using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public class MultiSpawner : MonoBehaviour
    {
        [SerializeField] GameObject birdPrefab = default;
        [SerializeField] GameObject spiderPrefab = default;
        [SerializeField] GameObject boulderPrefab = default;
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

        

        public void SpawnEnnemy(int birdNumber, int spiderNumber, int boulderNumber, Vector3Int roomSize)
        {
            _childrenMeshRenderers = GetComponentsInChildren<MeshRenderer>();
            StartCoroutine(EnemyDrop(birdPrefab, birdNumber, roomSize));
            StartCoroutine(EnemyDrop(spiderPrefab, spiderNumber, roomSize));
            StartCoroutine(EnemyDrop(boulderPrefab, boulderNumber, roomSize));
        }

        IEnumerator EnemyDrop(GameObject prefab, int number, Vector3Int roomSize)
        {
            for (int i = 0; i < number; i++)
            {
                Color outlineColorRng = Color.HSVToRGB(Random.value, 1, 0.35f);
                Color baseColorRng = Color.HSVToRGB(Random.value, 1, 0.35f);
                _xPos = Random.Range(-(roomSize.x * 12 / 2) + 4, (roomSize.x * 12 / 2) - 4);
                _zPos = Random.Range(-(roomSize.z * 12 / 2) + 4, (roomSize.z * 12 / 2) - 4);
                _enemy = Instantiate(
                    prefab,
                    new Vector3(transform.position.x + _xPos, transform.position.y + 3, transform.position.z + _zPos),
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
