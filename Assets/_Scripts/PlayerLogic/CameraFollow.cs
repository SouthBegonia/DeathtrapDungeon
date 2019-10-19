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
 *   
 *   ------------------------------------
 *   - 出现问题1-1: 移动时画面出现可见的白色竖线，尤其出现在boss关卡(player多次碰撞情况)
 *   - 问题分析: 出现的白线时SolidClear下的BackGround颜色，但出现这种问题有可能是Tilemap之间存在间隙
 *   - 解决方案1: 修改美术资源的pixels per unit为99
 *   - 方案效果1：没啥用，bug依旧
 *   - 解决方案2：MainCamera中ClearFlags模式变更为DontClear而不是SolidClear
 *   - 方案效果2：的确见不到白线了，DontClear机理:每帧绘制在下一帧之上，造成涂片效果
 *   - 出现问题1-2：在对CameraFollow进行差值跟踪后，还是出现了白线，因此很可能不是Camera的问题，而是Tile绘制问题
 *   
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

    private Vector3 delts = Vector3.zero;
    private Vector3 destination;

    private void Start()
    {
        lookAt = GameObject.Find("Player").transform;
    }

    private void FixedUpdate()
    {
        //移动的差值delts:
        //实现在一定范围内Player移动时相机不跟随,但是当超出一定范围时相机跟随移动
        //也就是说:根据相机与Player的间距值,判断是否跟随移动
        delts = Vector3.zero;

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

        delts.z = 0f;

        //新方法：差值平滑过渡
        destination = Vector3.Lerp(transform.position, transform.position + delts, 0.2f);

        /* 此处容易出现问题：camera渲染层次混乱
        * 问题分析：下列代码应当设置为z<0，否则z=0就会与地图等元素处于同一平面，渲染顺序出错
        * 解决：留意camera的渲染问题：transform.z<0 和 depth=-1
        */
        destination.z = -1f;


        //旧方法：设定相机的最新位置，无差值平滑过渡
        //transform.position += new Vector3(delts.x, delts.y, 0);
        transform.position = destination;
    }

    private void LateUpdate()
    {
        //transform.position = destination;
    }
}
