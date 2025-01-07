using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private float speed = 5f;
    public  Animator animator;
    public bool isMoving;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public virtual async UniTask Move(Vector3 targetPos)
    {
        isMoving = true;
        animator.SetBool("isMoving", true);

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            await UniTask.Yield();
        }

        transform.position = targetPos;
        isMoving = false;
        animator.SetBool("isMoving", false);       
    }

}
