using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : EnemyHitBox
{
    public float lateToStart = 1f;  //几秒后启动陷阱(实现非同步的陷阱阵列)
    private Animator animator;      //Trap的animator

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();

        //开启协程,延时后启用陷阱
        StartCoroutine("StartTrap");
    }

    protected override void OnCollide(Collider2D coll)
    {
        base.OnCollide(coll);
    }

    IEnumerator StartTrap()
    {
        yield return new WaitForSeconds(lateToStart);
        animator.SetTrigger("start");
    }
}
