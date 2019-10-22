# DeathtrapDungeon

* [游戏原型](#1)
* [项目演示](#2)
* [绘图资源](#3)
* [代码实现](#4)
* [注意事项](#5)
* [技术探讨](#6)
* [参考来源](#7)

----------

<h1 id="1">游戏原型</h1>

**死亡地牢**是一款 **2D-Roguelike** 的地牢冒险游戏。手握利刃，斩杀怪物，在凶险的地牢内生存下去。但注意，敌人也并非善茬，保持警惕，取舍果断，足智多谋才是制胜的关键。

博客园：[Unity项目 - DeathtrapDungeon死亡地牢](https://www.cnblogs.com/SouthBegonia/p/11604918.html)
项目地址：[DeathtrapDungeon - SouthBegonia](https://github.com/SouthBegonia/DeathtrapDungeon)
试玩下载：[DeathtrapDounge 提取码:wekp](https://pan.baidu.com/s/1YhGINK1zqLKmD6bp1C29tA)

<h1 id="2">项目演示</h1>

![](https://img2018.cnblogs.com/blog/1688704/201910/1688704-20191014092449918-1863025223.gif)

![](https://img2018.cnblogs.com/blog/1688704/201910/1688704-20191014092511727-133877714.gif)

![](https://img2018.cnblogs.com/blog/1688704/201910/1688704-20191014092530510-377580805.gif)

![](https://img2018.cnblogs.com/blog/1688704/201910/1688704-20191014092616483-819960666.gif)

![](https://img2018.cnblogs.com/blog/1688704/201910/1688704-20191014092640210-874694545.gif)

![](https://img2018.cnblogs.com/blog/1688704/201910/1688704-20191014092708669-1567758134.gif)

![](https://img2018.cnblogs.com/blog/1688704/201910/1688704-20191022220131532-786062296.gif)


<h1 id="3">绘图资源</h1>

- 主要美术资源来自：[Dungeon Tileset - itch](https://0x72.itch.io/16x16-dungeon-tileset)

--------------------

<h1 id="4">代码实现</h1>

**总控系统：**
- GameManager.cs：单例模式，统一管理各类的实例

**基类：** 
- 生命值类：Fighter.cs（生命值，伤害系统）
- 移动类：Mover.cs（移动系统）
- 交互类：Collectable.cs（检测碰撞体是否为Player）

**Player**：
- Player.cs：Rage怒气系统；换皮肤，死亡重生等
- Weapon：武器攻击系统，Rage技能系统

**Enemy**：
- EnemyHitBox.cs：传输伤害到玩家
- Enemy.cs：大部分敌人的类，包含经验值，追逐攻击系统，死亡复活系统等
- Enemy_Chest.cs：宝箱怪
- Trap.cs：陷阱
- Boss0.cs：最终boss

**GUI**：
- UIManager.cs：UI管理
- CharacterMenu.cs：菜单栏系统
- CharacterHUD.cs：玩家生命值，经验值，怒气值显示，死亡页面
- SCUI.cs：异步切换场景加载画面
- FloatingTextManager/FloatingText.cs：浮动文本显示系统

**场景**：
- CameraFollow.cs：相机跟随系统
- SceneTranslate.csa：异步加载场景系统
- Portal.cs：不同场景传送门
- Portal_Door.cs：当前场景传送门

**交互物件**：
- NPCTextPerson.cs：NPC交互
- Chest.cs：宝箱
- Crate.cs：可破坏物件
- HealingFountain：治愈泉水
- Door.cs：开关门

![](https://img2018.cnblogs.com/blog/1688704/201910/1688704-20191022220208959-2106713466.png)
![](https://img2018.cnblogs.com/blog/1688704/201910/1688704-20191022220216548-386181557.png)
![](https://img2018.cnblogs.com/blog/1688704/201910/1688704-20191022220224478-570747477.png)
![](https://img2018.cnblogs.com/blog/1688704/201910/1688704-20191022220238304-1366744285.png)

--------------------

<h1 id="5">技术探讨</h1>

- **关于武器挥动进行攻击的animation**：
	1. 武器的Swing动画时间建议大于0.3s，否则间隔过短即便每次都能攻击到敌人但不足以有效推开敌人而被攻击
	2. 武器劈砍至水平的时间建议在前半时间内完成，因为涉及武器的碰撞伤害；水平下的武器碰撞器范围较长,利于和敌人保持距离,并造成伤害
	3. 最好模拟真实情况下的劈砍动作,我将其分为以下几部分:
		- p1:武器抬起并后仰，此过程速度适中，禁用collider
		- p2:从90'到0'进行主劈砍动作.此过程前半速度快,后半速度极快，启用collider
		- p3:0'到-20'最终劈砍动作.此过程不在于速度,而在于保持帧数(时间)
		- p4:武器收回，速度快，禁用Collider

![](https://img2018.cnblogs.com/blog/1688704/201909/1688704-20190928211647918-820959590.gif)

- **Tilemap的绘制**：
	1. 最好在确定美术资源时即确定整体绘制的像素信息，场景风格等，假若后期要添加其他要素，风格不同很不应景
	2. Tile的类型最好也确定，使用普通tile+animation物体，或是RuleTile实现各类功能
	3. 瓦片的渲染顺序体现在Tilemap的顺序(例如上墙wall层，下墙壁ScreenWall层；上层可被player遮挡，下层可遮挡player)
	4. 非瓦片物体的渲染顺序体现在SpriteRenderer里的SortingLayer和OrderLayer；合理设置遮挡与被遮挡关系
- **2D碰撞问题**：
	- 所有物体最好处于同一z=0平面，否则一些碰撞检测容易出问题
- **Draw Call优化**：
	- Tilemap地图：占用5+DrawCall，主要体现在墙壁层。由于本例采用了两种类型的美术资源，且含有普通Tile和RuleTile，故此处产生较多DrawCall，因此在往后Tilemap绘制时，应注意美术资源和Tile类型的统一性，此外，对于静态物体设置为static
	- 装备栏Menu：占用9+DrawCall，由于装备栏信息众多(Weapon、Player的Sprite切换，游戏数值信息等)。不同Sprite的部件众多、Button功能各异、Text字体不一，因此造成较多DrawCall。此处除了统一字体，减少Panel缩放外暂无更好办法
	- 数值面板HUD：占用4+DrawCall，此处功能为显示画面左上角的数值信息，同样也是存在不同Sprite部件，暂无更优解
	- Animator物件：被当做Object的Animator物件，可将其CullingMode设置为Cull Completely，即不在视野内时停止运作，可有效降低DrawCall
- **GC优化**：
	- 通过Profiler分析脚本中的GC Alloc，找到对应脚本进行代码优化(减少循环内new对象、减少foreach等)，此外对场景中暂时不用的物体进行隐藏
- **内存优化**：
	- 由于是2D游戏，本例目前仅对bgm进行了优化，即将其LoadType变更为Streaming流媒体播放形式，大幅降低了运行时占用内存
	- 其他优化方法：AssetBundle打包、降低贴图大小、采用MipMap等

--------

<h1 id="6">*注意事项</h1>

游戏的核心机制，如战斗，场景切换，物体交互等**已成型**，且Tilemap完善(地面层，上下墙壁层，地表物件层，碰撞层)，除了基本的静态Tiles外，还有几个AnimatedTile(泉水，熔岩等)，及ChestBrush宝箱笔刷方便宝箱的摆放设置等等。如想要自行**定制关卡**，完全可以基于此脚本系统下，对Tilemap的重绘、游戏数值、场景物件摆放、NPC台词等即可。

此外，[itch.io](https://itch.io/game-assets)网站内含有大量优秀的2D绘图资源，可按需合法使用。

![](https://img2018.cnblogs.com/blog/1688704/201909/1688704-20190928211702664-1369000020.gif)

-------------

<h1 id="7">参考来源</h1>
- [Unity与C＃制作RPG游戏工作流程教程](https://www.bilibili.com/video/av45071686/?p=1)
- [16x16 Dungeon Tileset - itch](https://0x72.itch.io/16x16-dungeon-tileset)
- [2d-extras - UnityTechnology](https://github.com/Unity-Technologies/2d-extras)
- [Unity优化美术资源的设置 - ww38362087](https://blog.csdn.net/ww386362087/article/details/81365595)
- [Unity优化减少DrawCall:批处理 - linuxheik](https://blog.csdn.net/linuxheik/article/details/80688109)