using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class MonsterCtrl : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Transform targetTr;

    [SerializeField] private Transform pos1;
    [SerializeField] private Transform pos2;


    private bool changePos;
    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        targetTr = pos1;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        if (_agent.remainingDistance < 2f)
        {
            ChangeTarget();
        }

        _agent.SetDestination(targetTr.position);

        Debug.DrawRay(transform.position, transform.forward * 10, Color.red);
        if (Physics.Raycast(transform.position, transform.forward * 10, out hit))
        {
            if (hit.transform.CompareTag("Player"))
            {
                Debug.Log("Player Hit");
            }
        }
    }

    void ChangeTarget()
    {
        if(changePos)
        {
            changePos = false;
            targetTr = pos1;
        }
        else
        {
            changePos=true;
            targetTr = pos2;
        }
    }
}
