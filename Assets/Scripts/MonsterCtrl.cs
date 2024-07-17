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

    [SerializeField] private float angle = 45f; // 부채꼴의 각도
    [SerializeField] private int rayCount = 20; // 레이의 개수
    [SerializeField] private float maxDistance = 10f; // 레이의 최대 거리

    [SerializeField] private LayerMask wallLayer; // 벽 레이어 마스크 추가

    [SerializeField] private GameObject lineRendererPrefab; // LineRenderer 프리팹
    private bool changePos;
    private List<LineRenderer> lineRenderers = new List<LineRenderer>(); // LineRenderer 목록

    private float alarmTimer;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        targetTr = pos1;

        // LineRenderer 초기화
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
        // 부채꼴 레이캐스트 수행
        ShootFanRays();

        if (_agent.remainingDistance < 2f && guardStat == 0)
        {
            ChangeTarget();
        }

        //게임매니저의 경계모드 전환용 변수가 참이면서 전투모드 상태가 아니면
        //guardStat의 값을 1로 만들고 맥스디스턴스 값을 1.25로 바꾸는 조건문
        if (GameManager.Instance.alarm && guardStat==0)
        {
            maxDistance = 12.5f;
            guardStat = 1;
        }

        switch (guardStat)
        {
            case 0: // 일반모드
                _agent.SetDestination(targetTr.position);
                break;
            case 1: // 경계모드
                //타겟의 포지션이 목적지인코드+ 타이머가 돌아가는코드
                alarmTimer += Time.deltaTime;
                //+ 타이머가 일정시간 지나면 타이머를 초기화하고 케이스값을 1로 설정하는 조건문
                if(alarmTimer > 5)
                {
                    alarmTimer = 0;
                    guardStat = 0;
                }
                break;
            case 2: // 전투모드
                _agent.SetDestination(player.transform.position);
                break;
            case 3: // 전투모드에 들어간 적에게 가는 상태
                //대충 셋 데스티네이션을 전투모드인 적으로 설정하는 코드
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
                    // 벽 레이어 체크
                    Ray wallCheckRay = new Ray(transform.position, (hit.point - transform.position).normalized);
                    if (!Physics.Raycast(wallCheckRay, out RaycastHit wallHit, hit.distance, wallLayer))
                    {
                        maxDistance = 15;
                        guardStat = 2; // 전투모드로 전환
                        //게임 매니저에서 경계모드 전환용 bool변수 참으로 변경
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