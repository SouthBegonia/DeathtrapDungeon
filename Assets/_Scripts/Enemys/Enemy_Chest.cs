using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Chest : Enemy
{
    public bool _________________;
    public Sprite[] sprites;

    //宝箱怪在追逐Player时才原形毕露
    protected override void Update()
    {
        base.Update();
        if (chasing)
            gameObject.GetComponent<SpriteRenderer>().sprite = sprites[1];
        else
            gameObject.GetComponent<SpriteRenderer>().sprite = sprites[0];
    }

    protected override void Death()
    {
        //宝箱怪的死亡销毁就行
        Destroy(gameObject);

        //玩家获得经验,显示+xp的UI
        GameManager.instance.GrantXP(xpValue);
        GameManager.instance.ShowText("+" + xpValue + " xp", 30, Color.magenta, transform.position, Vector3.up * 40, 1.0f);
    }
}
