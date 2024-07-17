using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController _controller;
    public float speed = 5.0f;
    public float rotationSpeed = 720.0f; // 캐릭터 회전 속도

    public bool isSafe;
    void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // WASD 입력 받기
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // 방향 벡터 생성
        Vector3 move = new Vector3(moveX, 0, moveZ);

        // 이동 벡터의 크기가 0보다 큰지 확인 (즉, 이동 중인지 확인)
        if (move.magnitude > 0)
        {
            // 이동 방향으로 캐릭터를 회전
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // 캐릭터 이동
        _controller.Move(move * speed * Time.deltaTime);
    }
}
