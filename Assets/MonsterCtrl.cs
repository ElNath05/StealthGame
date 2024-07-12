using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Transform targetTr;

    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        
    }

    // Update is called once per frame
    void Update()
    {
        _agent.SetDestination(targetTr.position);

        //if(_agent.remainingDistance < 10f)
        //{
        //    _agent.isStopped = true;
        //}
        //else
        //{
        //    _agent.isStopped = false;
        //}
    }
}
