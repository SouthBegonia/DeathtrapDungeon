using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//敌人基类:包括击杀获得经验值,追逐Player功能等
/* 目前已有的功能:
 *  - 追逐玩家进行攻击伤害
 *  - 死亡一段时间后复活
 * 
 * 更多可加入的功能:
 *  - 未追逐Player时随机走动
 *  - 敌人间的相互厮杀
 * 
 */
public class Enemy : Mover
{
    private bool isAlive = true;            //Enenmy是否存活
    [Header("------复活系统----")]
    public bool canRespawn = true;          //Eneny是否可以复活
    public float timeToRespawn = 10f;       //多少秒后Enemy复活

    [Header("------经验值----")]
    public int xpValue = 1;                 //击杀获得经验值


    //追逐逻辑: 
    [Header("------追逐逻辑----")]
    public float speedMultiple = 0.75f;     //Enemy的速度为正常速度的speedMultiple倍
    public float triggerLength = 1.0f;      //在多少距离内能触发追逐
    public float chaseLength = 1.0f;        //能追逐到多远的距离
    public bool chasing;                    //是否追逐
    public bool collidingWithPlayer;        //是否与玩家碰撞中(防穿透)

    private Transform playTransform;        //玩家标识
    private Vector3 startingPosition;       //Enemy原始坐标

    //Enemy状态标记
    [Header("------状态标记----")]
    public SpriteRenderer enemyStateSprite;
    public List<Sprite> stateSprites;

    //hitbox碰撞器
    //[Header("------其他----")]
    public ContactFilter2D filter;
    private BoxCollider2D hitBox;
    private Collider2D[] hits = new Collider2D[10];

    //DEBUG相关
    public bool drawTriggerLength;          //绘制Trigger范围

    protected override void Start()
    {
        base.Start();
        playTransform = GameManager.instance.player.transform;
        startingPosition = transform.position;

        //留意此处：是取得第一个孩子(Hitbox)
        hitBox = transform.GetChild(0).GetComponent<BoxCollider2D>();
        CloseStateSprite();
    }

    protected virtual void Update()
    {
        collidingWithPlayer = false;

        if (drawTriggerLength)
        {
            //可视化查看triggerLength的范围      
            Debug.DrawLine(transform.position, new Vector3(transform.position.x + triggerLength, transform.position.y, transform.position.z), Color.green);
            Debug.DrawLine(transform.position, new Vector3(transform.position.x - triggerLength, transform.position.y, transform.position.z), Color.green);
            Debug.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y + triggerLength, transform.position.z), Color.green);
            Debug.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - triggerLength, transform.position.z), Color.green);
        }

        //取得重叠的碰撞体
        hitBox.OverlapCollider(filter, hits);
        if (hitBox == null)
            return;
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null)
                continue;

            //如果检测到范围内存在Player
            if (hits[i].CompareTag("Fighter") && hits[i].name == "Player")
            {
                collidingWithPlayer = true;
            }
            hits[i] = null;
        }


    }

    private void FixedUpdate()
    {
        //仅当Enemy存活时才进行追逐判定
        if (isAlive)
            ChasingTarget();
        else
            pushDirection = Vector3.zero;
    }

    //追逐玩家函数:
    protected virtual void ChasingTarget()
    {
        //若Player在Enemy原始坐标chaseLength范围内时,可能被追逐
        if ((Vector3.Distance(playTransform.position, startingPosition) < chaseLength) && GameManager.instance.player.isAlive)
        {
            //再 若Player与Enemy范围过近(triggerLength内),则Enemy开始追逐
            if (Vector3.Distance(playTransform.position, startingPosition) < triggerLength)
                chasing = true;

            //追逐状态:
            if (chasing)
            {
                OpenStateSprite();

                //若Enemy与Player处于非碰撞状态,则继续进行追逐移动
                //否则保持与Player碰撞状态,无需再移动
                if (!collidingWithPlayer)
                {
                    UpdateMotor((playTransform.position - transform.position).normalized, speedMultiple);
                }
            }
            else
            {
                //非追逐状态: Enemy回到原始坐标
                UpdateMotor((startingPosition - transform.position), speedMultiple);
                CloseStateSprite();
            }
        }
        else
        {
            //否则Enemy与Player距离过远,不再保持追逐状态,Enemy回到原始坐标
            UpdateMotor((startingPosition - transform.position), speedMultiple);
            chasing = false;
            CloseStateSprite();
        }
    }

    //开启状态显示：半血以上/半血以下
    private void OpenStateSprite()
    {
        enemyStateSprite.enabled = true;
        if ((float)hitPoint / (float)maxHitPoint < 0.5)
            enemyStateSprite.sprite = stateSprites[1];
        else
            enemyStateSprite.sprite = stateSprites[0];
    }

    private void CloseStateSprite()
    {
        enemyStateSprite.enabled = false;
    }

    //Enemy死亡函数
    protected override void Death()
    {
        //玩家获得经验,显示+xp的UI
        GameManager.instance.GrantXP(xpValue);
        GameManager.instance.ShowText("+" + xpValue + " xp", 30, Color.magenta, transform.position, Vector3.up * 40, 1.0f);

        //按需决定是否可以复活
        if (canRespawn)
        {
            isAlive = false;
            hitBox.enabled = false;
            CloseStateSprite();
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            StartCoroutine("WaitingForRespawn");
        }
        else
            Destroy(gameObject);
    }

    IEnumerator WaitingForRespawn()
    {
        yield return new WaitForSeconds(timeToRespawn);
        isAlive = true;
        hitBox.enabled = true;
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        OpenStateSprite();
        hitPoint = maxHitPoint;
        gameObject.transform.position = startingPosition;
    }
}
