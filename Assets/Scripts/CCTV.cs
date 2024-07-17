using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTV : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private LayerMask wallLayer; // 벽 레이어 마스크 추가
    [SerializeField] private GameObject lineRendererPrefab; // LineRenderer 프리팹

    [SerializeField] private float angle = 45f; // 부채꼴의 각도
    [SerializeField] private int rayCount = 20; // 레이의 개수
    [SerializeField] private float maxDistance = 10f; // 레이의 최대 거리

    private List<LineRenderer> lineRenderers = new List<LineRenderer>(); // LineRenderer 목록
    // Start is called before the first frame update
    void Start()
    {

        // LineRenderer 초기화
        for (int i = 0; i < rayCount; i++)
        {
            GameObject lineObj = Instantiate(lineRendererPrefab);
            lineObj.transform.parent = transform;
            LineRenderer lineRenderer = lineObj.GetComponent<LineRenderer>();
            lineRenderers.Add(lineRenderer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 부채꼴 레이캐스트 수행
        ShootFanRays();
        if(Vector3.Distance(transform.position, player.transform.position) > maxDistance && GameManager.Instance.alarm)
        {
            GameManager.Instance.alarm = false;
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
