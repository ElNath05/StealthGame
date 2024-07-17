using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Transform targetTr;

    [SerializeField] private Transform pos1;
    [SerializeField] private Transform pos2;

    [SerializeField] private GameObject player;

    [SerializeField] private int guardStat;

    [SerializeField] private float angle = 45f; // ��ä���� ����
    [SerializeField] private int rayCount = 20; // ������ ����
    [SerializeField] private float maxDistance = 10f; // ������ �ִ� �Ÿ�

    [SerializeField] private LayerMask wallLayer; // �� ���̾� ����ũ �߰�

    [SerializeField] private GameObject lineRendererPrefab; // LineRenderer ������
    private bool changePos;
    private List<LineRenderer> lineRenderers = new List<LineRenderer>(); // LineRenderer ���

    private float alarmTimer;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        targetTr = pos1;

        // LineRenderer �ʱ�ȭ
        for (int i = 0; i < rayCount; i++)
        {
            GameObject lineObj = Instantiate(lineRendererPrefab);
            lineObj.transform.parent = transform;
            LineRenderer lineRenderer = lineObj.GetComponent<LineRenderer>();
            lineRenderers.Add(lineRenderer);
        }
    }

    void Update()
    {
        // ��ä�� ����ĳ��Ʈ ����
        ShootFanRays();

        if (_agent.remainingDistance < 2f && guardStat == 0)
        {
            ChangeTarget();
        }

        //���ӸŴ����� ����� ��ȯ�� ������ ���̸鼭 ������� ���°� �ƴϸ�
        //guardStat�� ���� 1�� ����� �ƽ����Ͻ� ���� 1.25�� �ٲٴ� ���ǹ�
        if (GameManager.Instance.alarm && guardStat==0)
        {
            maxDistance = 12.5f;
            guardStat = 1;
        }

        switch (guardStat)
        {
            case 0: // �Ϲݸ��
                _agent.SetDestination(targetTr.position);
                break;
            case 1: // �����
                //Ÿ���� �������� ���������ڵ�+ Ÿ�̸Ӱ� ���ư����ڵ�
                alarmTimer += Time.deltaTime;
                //+ Ÿ�̸Ӱ� �����ð� ������ Ÿ�̸Ӹ� �ʱ�ȭ�ϰ� ���̽����� 1�� �����ϴ� ���ǹ�
                if(alarmTimer > 5)
                {
                    alarmTimer = 0;
                    guardStat = 0;
                }
                break;
            case 2: // �������
                _agent.SetDestination(player.transform.position);
                break;
            case 3: // ������忡 �� ������ ���� ����
                //���� �� ����Ƽ���̼��� ��������� ������ �����ϴ� �ڵ�
                break;
        }

        if (_agent.remainingDistance > maxDistance && guardStat == 2 || GameManager.Instance.isSafe)
        {
            guardStat = 0;
            maxDistance = 10;
        }
    }

    void ChangeTarget()
    {
        if (changePos)
        {
            changePos = false;
            targetTr = pos1;
        }
        else
        {
            changePos = true;
            targetTr = pos2;
        }
    }

    void ShootFanRays()
    {
        float startAngle = -angle / 2;
        float stepAngle = angle / (rayCount - 1);

        for (int i = 0; i < rayCount; i++)
        {
            float currentAngle = startAngle + (stepAngle * i);
            Vector3 direction = Quaternion.Euler(0, currentAngle, 0) * transform.forward;

            Ray ray = new Ray(transform.position, direction);
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
            {
                //Debug.DrawLine(ray.origin, hit.point, Color.red);
                lineRenderers[i].SetPosition(0, ray.origin);
                lineRenderers[i].SetPosition(1, hit.point);
                if (hit.transform.CompareTag("Player") && !GameManager.Instance.isSafe)
                {
                    // �� ���̾� üũ
                    Ray wallCheckRay = new Ray(transform.position, (hit.point - transform.position).normalized);
                    if (!Physics.Raycast(wallCheckRay, out RaycastHit wallHit, hit.distance, wallLayer))
                    {
                        maxDistance = 15;
                        guardStat = 2; // �������� ��ȯ
                        //���� �Ŵ������� ����� ��ȯ�� bool���� ������ ����
                    }
                }
            }
            else
            {
                //Debug.DrawLine(ray.origin, ray.origin + direction * maxDistance, Color.green);
                lineRenderers[i].SetPosition(0, ray.origin);
                lineRenderers[i].SetPosition(1, ray.origin + direction * maxDistance);
            }
        }
    }
}