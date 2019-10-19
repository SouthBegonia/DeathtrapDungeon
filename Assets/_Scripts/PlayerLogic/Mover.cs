using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//可移动对象的类
public abstract class Mover : Fighter
{
    private BoxCollider2D BoxCollider;
    private Vector3 moveDelta;
    private RaycastHit2D hit;

    //横纵坐标移动速度的比例
    protected float ySpeed = 0.75f;         
    protected float xSpeed = 1.0f;

    //记录物体的原始大小(因为部分敌人初始时即被放大或缩小)
    private Vector3 originalSize;

    protected virtual void Start()
    {
        BoxCollider = GetComponent<BoxCollider2D>();
        originalSize = transform.localScale;
    }

    //接受input及速度倍数而移动的函数
    protected virtual void UpdateMotor(Vector3 input,float SPMultiple)
    {
        //移动坐标:各轴向方向x速度倍数
        moveDelta = new Vector3(input.x * xSpeed * SPMultiple, input.y * ySpeed * SPMultiple, 0);

        //变更方向:正向/逆向
        if (moveDelta.x > 0)
            transform.localScale = originalSize;
        else if (moveDelta.x < 0)
            transform.localScale = new Vector3(originalSize.x * -1, originalSize.y, originalSize.y);

        //被击退移动
        //被击退距离随pushRecoverSpeed系数呈线性减小
        moveDelta += pushDirection;
        pushDirection = Vector3.Lerp(pushDirection, Vector3.zero, pushRecoverySpeed);

        //定向移动:上下左右
        //仅当物件处在Blocking层或者Actor层才、且拥有BoxCollider2D时才可被检测到
        //机理：产生一个大小与boxcollider2D相等的检测矩阵，延伸矩阵朝向至将要抵达的坐标，若存在指定层的物体，则返回与之接触的第一个物体；
        //      反之为空，表示可以移动到目的地
        //
        //下列潜藏的巨大BUG：
        //出现问题：当被敌人攻击，造成击退效果、且同时Player近贴墙壁是时，会被怼出去
        //问题分析：出界只能是pushDirection移动判定的问题
        //解决方案：添加判定，当与墙碰撞时，收到的pushDirection为0
        hit = Physics2D.BoxCast(transform.position, BoxCollider.size, 0, new Vector2(0, moveDelta.y), Mathf.Abs(moveDelta.y * Time.deltaTime), LayerMask.GetMask("Actor", "Blocking"));
        if (hit.collider == null)
            transform.Translate(0, moveDelta.y * Time.deltaTime, 0);
        else pushDirection = Vector3.zero;

        hit = Physics2D.BoxCast(transform.position, BoxCollider.size, 0, new Vector2(moveDelta.x, 0), Mathf.Abs(moveDelta.x * Time.deltaTime), LayerMask.GetMask("Actor", "Blocking"));
        if (hit.collider == null)
            transform.Translate(moveDelta.x * Time.deltaTime, 0, 0);
        else pushDirection = Vector3.zero;
    }
}
