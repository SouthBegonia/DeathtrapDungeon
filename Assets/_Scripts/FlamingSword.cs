using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//光刃脚本:挂在于FlamingSword预制体上.在Weapon能生成克隆体,克隆体上的该脚本进行运动控制及伤害判定
//
//出现问题:为何继承Colliderable类时无法取得boxCollider组件??
//解决方法:此处照搬Colliderable类内的逻辑
public class FlamingSword : MonoBehaviour
{
    private Vector3 originalSize;       //光刃的原始大小
    private float nowTime;              //计时器
    public float lifeTime = 0.5f;       //光刃的持续时间

    public ContactFilter2D filter;
    private BoxCollider2D boxCollider;
    private Collider2D[] hits = new Collider2D[10];

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        //刀光的初始坐标在Player前方
        Vector3 pos = GameManager.instance.player.transform.position;
        pos.y = GameManager.instance.player.transform.position.y - 0.02f;
        
        //修正方向
        originalSize = GetComponent<Transform>().localScale;
        if (GameManager.instance.player.transform.localScale.x > 0)
        {
            transform.localScale = originalSize;
            pos.x += 0.1f;
        }           
        else if (GameManager.instance.player.transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(originalSize.x * -1, originalSize.y, originalSize.y);
            pos.x -= 0.1f;
        }

        nowTime = Time.time;
        gameObject.transform.position = pos;
    }

    private void Update()
    {
        if (Time.time - nowTime > lifeTime)
            Destroy(gameObject);

        //获取与此Collider碰撞的所有碰撞器的表
        boxCollider.OverlapCollider(filter, hits);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null)
                continue;

            OnCollide(hits[i]);

            hits[i] = null;
        }

        //光刃的移动
        //此处建议速度调快:因为速度慢可能会与Enemy造成多次碰撞,形成多段伤害
        transform.position += Vector3.right * Time.deltaTime * transform.localScale.x * 3f;
    }

    private void OnCollide(Collider2D coll)
    {
        if (coll.tag == "Fighter")
        {
            if (coll.name == "Player")
                return;

            Damag dmg = new Damag
            {
                //敌人受到的伤害/击退距离为当前武器的2倍              
                damageAmount = GameManager.instance.weapon.damagePoint[GameManager.instance.weapon.weaponLevel] * 2,
                origin = transform.position,
                pushForce = GameManager.instance.weapon.pushForce[GameManager.instance.weapon.weaponLevel] * 2
            };

            //发送消息给被碰撞物体,调用其接受伤害函数(Fighter类内)
            coll.SendMessage("ReceiveDamage", dmg);
        }
    }
}
