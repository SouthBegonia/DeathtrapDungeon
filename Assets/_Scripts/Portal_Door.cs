using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//传送门脚本:进入挂载脚本的物体,传送到boxOUT物体附近
/* 使用方法: 设置门的Rotation.z, 使得Player始终从+y轴进入,+y轴出来. 
 *          也可以从 /_Prefabs/Scene下直接使用PortalDoor预制体
 *  设置参数:
 *  - 上下传送门: R(0,0,180)  R(0,0,0)
 *  - 左右传送门: R(0,0,-90)  R(0,0,90)
 * 
 */
public class Portal_Door : Colliderable
{
    public BoxCollider2D boxOUT;            //传送到的地点

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnCollide(Collider2D coll)
    {      
        if (coll.name == "Player")
        {
            //朝向boxOUT的y轴正方向移动一定距离防止再次出发传送
            Vector3 vector = boxOUT.transform.position;

            //修正Z轴:保持Player与其他始终在Z=0平面,防止portal_Door的传送出bug
            vector.z = 0;
            
            //上下传送门:
            if (boxOUT.gameObject.transform.rotation == Quaternion.Euler(0,0,0))
                vector.y += 0.2f;
            if(boxOUT.gameObject.transform.rotation == Quaternion.Euler(0, 0, 180))
                vector.y -= 0.2f;

            //左右传送门:
            if (boxOUT.gameObject.transform.rotation == Quaternion.Euler(0, 0, -90))
                vector.x += 0.2f;
            if (boxOUT.gameObject.transform.rotation == Quaternion.Euler(0, 0, 90))
                vector.x -= 0.2f;

            //设置位置到出口
            GameManager.instance.player.transform.position = vector;
        }
    }
}
