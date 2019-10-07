using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//相机跟随脚本:挂在于相机1
/* 相机1: MainCamera,设定为display1
 *   - 实现效果:脚本控制相机跟随范围,但是较为僵硬
 * 
 * 
 * 相机2: Player下的CM vcam1虚拟相机,设定为display2
 *   - 实现效果:相机动态跟随效果好
 *   ------------------------------------
 *   - 出现问题1:移动时画面出现可见的白色竖线
 *   - 问题分析:
 *   - 解决方案:修改美术资源的pixels per unit为99
 *   ------------------------------------
 *   - 出现问题2:当Player死亡重生后相机方向为-90
 *   - 问题分析:Player死亡时倒地,其rotation.z=-90,导致跟随的虚拟相机也偏移
 *   - 解决方案:将虚拟相机归于GameManger物体下
 */

public class CameraFollow : MonoBehaviour
{
    private Transform lookAt;           //相机跟随的目标(Player)
    public float boundX = 0.3f;         //X轴差值范围
    public float boundY = 0.15f;        //Y轴差值范围

    private void Start()
    {
        lookAt = GameObject.Find("Player").transform;
    }

    private void LateUpdate()
    {
        //移动的差值delts:
        //实现在一定范围内Player移动时相机不跟随,但是当超出一定范围时相机跟随移动
        //也就是说:根据相机与Player的间距值,判断是否跟随移动
        Vector3 delts = Vector3.zero;

        //X轴的差值:若相机与Player间距超过范围值,则对差值delts赋值
        float deltaX = lookAt.position.x - transform.position.x;
        if (deltaX > boundX || deltaX < -boundX)
        {
            if (transform.position.x < lookAt.position.x)
                delts.x = deltaX - boundX;
            else
                delts.x = deltaX + boundX;
        }

        //Y轴的差值:若相机与Player间距超过范围值,则对差值delts赋值
        float deltaY = lookAt.position.y - transform.position.y;
        if (deltaY > boundY || deltaY < -boundY)
        {
            if (transform.position.y < lookAt.position.y)
                delts.y = deltaY - boundY;
            else
                delts.y = deltaY + boundY;
        }

        //设定相机的最新位置
        transform.position += new Vector3(delts.x, delts.y, 0);
    }
}
