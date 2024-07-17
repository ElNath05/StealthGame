using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
public class DoorOpen : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private int wallType;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            switch(wallType)
            {
                case 0:
                    if (GameManager.Instance.hasBkey)
                    {
                        animator.SetBool("Open", true);
                    }
                    break;
                case 1:
                    if (GameManager.Instance.hasGkey)
                    {
                        animator.SetBool("Open", true);
                    }
                    break; 
                case 2:
                    if (GameManager.Instance.hasRkey)
                    {
                        animator.SetBool("Open", true);
                    }
                    break;
            }            
        }
    }
}
