using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Transform targetTr;

    [SerializeField] private Transform pos1;
    [SerializeField] private Transform pos2;
    [SerializeField] private Transform pos3;

    [SerializeField] private GameObject player;

    [SerializeField] private int guardStat;

    [SerializeField] private float angle = 45f; // 부채꼴의 각도
    [SerializeField] private int rayCount = 20; // 레이의 개수
    [SerializeField] private float maxDistance = 10f; // 레이의 최대 거리

    [SerializeField] private LayerMask wallLayer; // 벽 레이어 마스크 추가

    [SerializeField] private GameObject lineRendererPrefab; // LineRenderer 프리팹
    private bool changePos;
    [SerializeField]  private int posNum = 0;
    private List<LineRenderer> lineRenderers = new List<LineRenderer>(); // LineRenderer 목록

    [SerializeField]  private float alarmTimer;
    [SerializeField]  private bool alertMod;
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
        if (GameManager.Instance.alarm && !alertMod && guardStat == 0)
        {
            maxDistance = 12.5f;
            alertMod = true;
            if (Vector3.Distance(transform.position, GameManager.Instance.alarmPosition.position) < 30f)
            {
                alarmTimer = 0;
                alertMod = false;
                guardStat = 2;
            }
        }
        if (alertMod)
        {
            alarmTimer += Time.deltaTime;
        }
        if (alarmTimer > 5)
        {
            alertMod = false;
            alarmTimer = 0;
            maxDistance = 10;
        }
        switch (guardStat)
        {
            case 0: // 일반모드
                _agent.SetDestination(targetTr.position);
                break;
            //case 1: // 경계모드
                //타겟의 포지션이 목적지인코드+ 타이머가 돌아가는코드
                //_agent.SetDestination(targetTr.position);
                //+ 타이머가 일정시간 지나면 타이머를 초기화하고 케이스값을 1로 설정하는 조건문
                
                //break;
            case 2: // 알람을 울린 적에게 가는 상태
                //대충 셋 데스티네이션을 전투모드인 적으로 설정하는 코드
                _agent.SetDestination(GameManager.Instance.alarmPosition.position);
                break;
            case 3: // 전투모드
                _agent.SetDestination(player.transform.position);
                break;
        }

        
        if (guardStat == 2 && !GameManager.Instance.alarm 
            && Vector3.Distance(transform.position, GameManager.Instance.alarmPosition.position) < 5f)
        {
            guardStat = 0;
            maxDistance = 10;
        }
        if (_agent.remainingDistance > maxDistance && guardStat == 3 || GameManager.Instance.isSafe)
        {
            GameManager.Instance.alarm = false;
            guardStat = 0;
            maxDistance = 10;
        }
    }

    void ChangeTarget()
    {
        if (posNum == 0)
        {
            posNum = 1;
            changePos = true;
            targetTr = pos2;
        }
        else if(posNum ==1 && !changePos)
        {
            posNum = 0;
            targetTr = pos1;
        }
        else if(posNum == 1 && changePos)
        {
            posNum = 2;
            targetTr = pos3;
        }
        else if(posNum == 2)
        {
            posNum = 1;
            changePos= false;
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
                        guardStat = 3; // 전투모드로 전환
                        //게임 매니저에서 경계모드 전환용 bool변수 참으로 변경
                        GameManager.Instance.alarm = true;
                        GameManager.Instance.alarmPosition = transform;
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