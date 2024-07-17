using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTV : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private LayerMask wallLayer; // �� ���̾� ����ũ �߰�
    [SerializeField] private GameObject lineRendererPrefab; // LineRenderer ������

    [SerializeField] private float angle = 45f; // ��ä���� ����
    [SerializeField] private int rayCount = 20; // ������ ����
    [SerializeField] private float maxDistance = 10f; // ������ �ִ� �Ÿ�

    private List<LineRenderer> lineRenderers = new List<LineRenderer>(); // LineRenderer ���
    // Start is called before the first frame update
    void Start()
    {

        // LineRenderer �ʱ�ȭ
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
        // ��ä�� ����ĳ��Ʈ ����
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
                    // �� ���̾� üũ
                    Ray wallCheckRay = new Ray(transform.position, (hit.point - transform.position).normalized);
                    if (!Physics.Raycast(wallCheckRay, out RaycastHit wallHit, hit.distance, wallLayer))
                    {
                        //���� �Ŵ������� ����� ��ȯ�� bool���� ������ ����
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
