using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Mover
{
    private SpriteRenderer spriteRenderer;      //玩家当前Sprite
    public bool isAlive = true;                 //玩家是否存活

    protected override void Start()
    {
        base.Start();

        spriteRenderer = GetComponent<SpriteRenderer>();
        ImmuneTime = 0.75f;
        Player.DontDestroyOnLoad(gameObject);
    }

    private void FixedUpdate()
    {
        //获取移动值,使用公用移动函数UpdateMotor(),按 指定位置/移动速度倍数 进行移动
        if (isAlive)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");

            UpdateMotor(new Vector3(x, y, 0), 1);
        }
    }

    //替换Sprite函数:
    public void SwapSprite(int SkinID)
    {
        GetComponent<SpriteRenderer>().sprite = GameManager.instance.playerSprites[SkinID];
    }

    //Player受伤函数: 减血,刷新生命值UI
    protected override void ReceiveDamage(Damag dmg)
    {
        if (!isAlive)
            return;

        base.ReceiveDamage(dmg);
        GameManager.instance.OnHitpointChange();
    }

    //Player的升级效果函数:提高生命值上限,恢复当前生命值
    public void OnLevelUp()
    {
        maxHitPoint += 10;
        hitPoint = maxHitPoint;

        GameManager.instance.OnHitpointChange();
      
    }
    public void SetLevel(int level)
    {
        for (int i = 0; i < level; i++)
            OnLevelUp();
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
        GameManager.instance.OnHitpointChange();
    }

    //Player死亡函数:
    protected override void Death()
    {
        //变更生命状态并倒地
        isAlive = false;
        transform.localEulerAngles = new Vector3(0, 0, 90);

        //死亡惩罚:当前所持金币清零
        GameManager.instance.pesos = 0;
        GameManager.instance.SaveState();

        //显示死亡面板
        GameManager.instance.deathMenuAnim.gameObject.SetActive(true);
        GameManager.instance.deathMenuAnim.SetTrigger("Show");

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
        GameManager.instance.menu.UpdateMenu();
        GameManager.instance.hud.UpdateHUD();
        GameManager.instance.deathMenuAnim.gameObject.SetActive(false);
    }
}