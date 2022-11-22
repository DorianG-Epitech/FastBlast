using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    public class EnemyMovement : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _navMeshAgent;
        private Transform _targetPosition;

        private void Start()
        {
            _targetPosition = GameObject.FindWithTag("Player").transform;
        }

        void Update()
        {
            _navMeshAgent.speed = GetComponent<EnemyEntity>().moveSpeed;
            if (_targetPosition != null)
                _navMeshAgent.SetDestination(_targetPosition.position);
            else
                _targetPosition = GameObject.FindWithTag("Player").transform;
        }
    }
}
