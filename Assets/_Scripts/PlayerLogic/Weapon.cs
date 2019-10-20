using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//武器攻击脚本:实现读取键位发动攻击
public class Weapon : Colliderable
{
    //各等级武器伤害参数:
    [Header("------伤害参数------")]
    public int[] damagePoint = { 1, 2, 3, 4, 5, 6, 7 };                         //武器造成伤害
    public float[] pushForce = { 2.0f, 2.2f, 2.5f, 3.0f, 3.3f, 3.6f, 4.0f };    //武器推力

    //武器等级参数:
    [Header("------等级参数------")]
    public int weaponLevel = 0;             //当前武器等级
    private SpriteRenderer SpriteRenderer;  //当前武器的Spite
    

    //武器控制参数:
    [Header("------控制参数------")]
    public Animator animator;              //动画组件
    private  float swingCoolDown = 0.4f;   //武器攻击冷却时间
    private float lastSwing;

    //Rage技能参数:
    //- Rage技能效果:普攻发出光刃,对敌人造成远程伤害
    //- Rage技能释放要求:在Player内收到伤害积累怒气rage,怒气满值即可释放
    [Header("------Rage技能参数------")]
    public GameObject flamingSword;         //光刃
    public GameObject rageState;            //技能特效
    public bool CanRageSkill = false;       //是否可以放技能
    public bool raging = false;             //是否在放技能中
    public float ragingTime = 4f;           //技能持续时间
    

    private void Awake()
    {
        //出现BUG处: 竞态条件问题
        //出现问题:若在start内写GetComponent<>则出现bug.
        //问题分析:因为GameManger内awake就已经执行 SceneManager.sceneLoaded += LoadState, 即执行weapon.SetWeaponLevel(int.Parse(data[3])),
        //        然而此时Weapon取得SpriteRenderer组件却是在Start内,晚于GameManager内的Awake, 即还没取得组件就进行设置,产生bug
        //解决方案:留意Awake和Start先后
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        rageState.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();

        //读取输入:空格键/鼠标左键实现普攻
        if (GameManager.instance.player.isAlive)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space))
            {
                if (Time.time - lastSwing > swingCoolDown)
                {
                    lastSwing = Time.time;

                    //武器劈砍动作               
                    Swing();

                    //释放技能
                    if (raging)
                    {                     
                        CreateFlamingSword();
                    }
                    else
                        rageState.SetActive(false);                        
                }
            }

            //当满足释放技能条件,且不在技能释放过程中
            if(Input.GetKeyDown(KeyCode.R) && (!raging))
            {
                if (CanRageSkill)
                {
                    raging = true;
                    rageState.SetActive(true);
                    StartCoroutine("WaitingForRestRageSkill");
                }
            }
        }

    }

    //武器碰撞造成伤害函数:
    //注当为idle状态下BoxCollider被禁用,故只有在Swing状态下才可检测
    protected override void OnCollide(Collider2D coll)
    {
        //必须为可被伤害类物体才可进行伤害检验
        if (coll.CompareTag("Fighter"))
        {
            //武器不可伤害到玩家
            if (coll.name == "Player")
                return;

            //其余即为敌人
            Damag dmg = new Damag
            {
                //敌人受到的伤害/击退距离由各等级的武器决定
                damageAmount = damagePoint[weaponLevel],
                origin = transform.position,
                pushForce = pushForce[weaponLevel]
            };

            //发送消息给被碰撞物体,调用其接受伤害函数(Fighter类内)
            coll.SendMessage("ReceiveDamage", dmg);
        }
    }

    //设置Animator状态函数:武器挥动swing
    private void Swing()
    {
        //----------------------------------
        //关于Swing动画的一些建议:
        //1. 武器的Swing动画时间建议大于0.3s,否则间隔过短即便每次都能攻击到敌人但不足以有效推开敌人而被攻击
        //2. 且武器劈砍至水平的时间建议在前半时间内完成,因为涉及武器的碰撞伤害:
        //   水平下的武器碰撞器范围较长,利于和敌人保持距离,并造成伤害
        //
        //后续补充:
        //3. 最好模拟真实情况下的劈砍动作,我将其分为以下几部分:
        //   p1:武器抬起并后仰.此过程速度适中
        //   p2:从90'到0'进行主劈砍动作.此过程前半速度快,后半速度极快
        //   p3:0'到-20'最终劈砍动作.此过程不在于速度,而在于保持帧数(时间)
        //以上设置也可根据难度而刻意为之
        //----------------------------------
        animator.SetTrigger("Swing");
    }

    //创建FlamingSword物体
    private void CreateFlamingSword()
    {
        GameObject go = Instantiate(flamingSword);
    }

    //升级武器
    public void UpgradeWeapon()
    {
        //等级提升,sprite变更
        weaponLevel++;
        SpriteRenderer.sprite = GameManager.instance.weaponSprites[weaponLevel];
    }

    //设置武器等级
    public void SetWeaponLevel(int level)
    {
        weaponLevel = level;
        SpriteRenderer.sprite = GameManager.instance.weaponSprites[weaponLevel];
    }

    IEnumerator WaitingForRestRageSkill()
    {
        yield return new WaitForSeconds(ragingTime);
        raging = false;
        CanRageSkill = false;
        GameManager.instance.player.rage = 0;
        GameManager.instance.OnUIChange();
    }
}