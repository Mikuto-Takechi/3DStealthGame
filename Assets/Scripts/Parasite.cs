using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Parasite : MonoBehaviour
{
    [SerializeField] bool _stopFlag = false;
    [SerializeField] float _wanderMoveRange = 3f;
    [SerializeField] Transform[] _patrolAnchor;
    [SerializeField] Animator _animator;
    int _patrolIndex = 0;
    NavMeshAgent _agent;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        if (!_agent.hasPath && !_stopFlag)
        {
            Patrol();
        }
    }
    void LateUpdate()
    {
        if(_animator && _agent && _agent.velocity.magnitude > 0)
        {
            _animator.SetFloat("Speed", _agent.speed);
        }
        else
        {
            _animator.SetFloat("Speed", 0);
        }
    }
    void Patrol()
    {
        if(_patrolIndex >= _patrolAnchor.Length)
        {
            _patrolIndex = 0;
        }
        _agent.SetDestination(_patrolAnchor[_patrolIndex].position);
        _patrolIndex++;
    }
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (_agent != null)
        {
            Gizmos.color = Color.red;
            Vector3 prefPos = transform.position;
            foreach (var p in _agent.path.corners)
            {
                Gizmos.DrawLine(prefPos, p);
                prefPos = p;
            }
        }
    }
#endif
}
