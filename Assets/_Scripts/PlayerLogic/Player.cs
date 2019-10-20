using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Mover
{
    private SpriteRenderer spriteRenderer;      //玩家当前Sprite
    public bool isAlive = true;                 //玩家是否存活

    [Header("------怒气系统------")]
    public float rage = 0;                      //怒气
    public float maxRage = 50;                  //怒气最值

    //转向攻击矫正系统
    private float temp = 0f;

    protected override void Start()
    {
        base.Start();
        GetComponent<BoxCollider2D>().enabled = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
        ImmuneTime = 0.75f;
        Player.DontDestroyOnLoad(gameObject);
        OnRageChange(0f);
    }

    private void FixedUpdate()
    {
        //获取移动值,使用公用移动函数UpdateMotor(),按 指定位置/移动速度倍数 进行移动
        if (isAlive)
        {           
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");

            //检测移动前后帧Player方向是否相同
            //若不同则将Weapon的Swing动画立即停止并转换到idle
            //否则正常，等待Swing动画播放完毕切换到idle
            //实现功能：攻击时转向则立即停止攻击动作
            if (transform.localScale.x == temp)
                GameManager.instance.weapon.animator.SetBool("SameDirection", true);
            else GameManager.instance.weapon.animator.SetBool("SameDirection", false);
            temp = transform.localScale.x;

            UpdateMotor(new Vector3(x, y, 0), 1);

            //注:若是用以下方法读取输入,会使得FixedUpdate内的GCAlloc增加数倍
            //
            //moveTo.x = Input.GetAxisRaw("Horizontal");
            //moveTo.y = Input.GetAxisRaw("Vertical");
            //UpdateMotor(moveTo, 1);
        }
        else
            pushDirection = Vector3.zero;
        //上述else出现问题：
        //问题描述：当player死亡重生时，会开场就被击退一段距离
        //问题分析：原因是player遭受致命一击后立马死亡，但是并未被击退，即当次pushDirection残余到重生后
        //解决方案：在上面添加else消除pushDirection，同理Enemy中也得修改
    }

    //Sprite变换函数:
    public void SwapSprite(int SkinID)
    {
        GetComponent<SpriteRenderer>().sprite = GameManager.instance.playerSprites[SkinID];
    }

    //等级提升函数:提高生命值上限,且恢复当前生命值
    public void OnLevelUp()
    {
        maxHitPoint += 10;
        hitPoint = maxHitPoint;

        GameManager.instance.OnUIChange();
    }
    
    //设置等级函数(仅供GameManger内调用)
    public void SetLevel(int level)
    {
        for (int i = 0; i < level; i++)
            OnLevelUp();
    }

    //Player受伤函数: 减血,怒气值增加,刷新生命值UI
    protected override void ReceiveDamage(Damag dmg)
    {
        if (!isAlive)
            return;

        //如果不在免疫时间内,则会被造成伤害
        if (Time.time - lastImmune > ImmuneTime)
        {
            lastImmune = Time.time;
            hitPoint -= dmg.damageAmount;
            pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;

            //怒气值系统:
            //如果正在释放技能中,则无法积累怒气
            if (!GameManager.instance.weapon.raging)
                OnRageChange(dmg.damageAmount); 
        }

        if (hitPoint <= 0)
        {
            hitPoint = 0;
            Death();
        }

        GameManager.instance.OnUIChange();
    }
    
    //怒气积累系统:
    public void OnRageChange(float alter)
    {
        if (rage < maxRage)
            rage += alter;
        if (rage >= maxRage)
            rage = maxRage;
        
        if(rage==maxRage)
            GameManager.instance.weapon.CanRageSkill = true;
    }

    //恢复生命值函数: 生命值恢复,显示恢复数值UI及刷新生命值UI
    public void Heal(int healingAmount)
    {
        if (hitPoint == maxHitPoint)
            return;

        hitPoint += healingAmount;
        if (hitPoint > maxHitPoint)
            hitPoint = maxHitPoint;

        GameManager.instance.ShowText("+" + healingAmount.ToString() + "hp", 25, Color.green, transform.position, Vector3.up * 30, 1.0f);
        GameManager.instance.OnUIChange();
    }

    //Player死亡函数:
    protected override void Death()
    {
        //变更生命状态并倒地
        isAlive = false;
        transform.localEulerAngles = new Vector3(0, 0, 90);

        //显示死亡面板
        GameManager.instance.UIManager.ShowDeathAnimation();

        //等待一定时间后复活并重新开始
        StartCoroutine("WaitingForRespawn");
    }

    //Player复活函数:
    public void Respawn()
    {
        //配置复活时的参数
        Heal(maxHitPoint);
        isAlive = true;
        transform.localEulerAngles = Vector3.zero;
    }

    IEnumerator WaitingForRespawn()
    {
        yield return new WaitForSeconds(6);
        GameManager.instance.Respawn();
        GameManager.instance.OnUIChange();
    }
}