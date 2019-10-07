using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//场景切换脚本:
//
//产生问题：切换场景后bgm感觉音量变小，声调有所变化
//尝试解决1:把AudioListener挪到Player上
//结果1：还行，bgm播放较正常
public class Portal : Colliderable
{
    public string sceneNames;   //所加载的场景

    public Canvas SCUI;         //场景切换Canvas
    public Slider SCUISlider;   //SCUI下的Slider进度条
    private float target = 0;   //场景加载值
    private float dtimer = 0;
    AsyncOperation op = null;   //异步操作协同程序
    

    protected override void Start()
    {
        base.Start();
        GetComponent<BoxCollider2D>().enabled = true;
        SCUI.gameObject.SetActive(false);
    }

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.name == "Player")
        {
            //储存各类信息
            GameManager.instance.SaveState();

            //切换场景：旧方法直接切换
            //SceneManager.LoadScene(sceneName);

            //新场景切换：异步加载
            ChangeScene();
        }
    }

    protected override void Update()
    {
        base.Update();

        SCUISlider.value = Mathf.Lerp(SCUISlider.value, target, dtimer * 0.01f);
        dtimer += Time.deltaTime;

        if (SCUISlider.value > 0.99f)
        {
            SCUISlider.value = 1;

            //进度条完，开启场景显示
            op.allowSceneActivation = true;
        }
    }

    public void ChangeScene()
    {
        //开启场景切换时的UI：黑布+进度条
        SCUI.gameObject.SetActive(true);
        GetComponent<BoxCollider2D>().enabled = false;
        SCUISlider.value = 0;

        //设置异步加载的场景
        op = SceneManager.LoadSceneAsync(sceneNames);
        op.allowSceneActivation = false;
        
        //开启异步加载协程
        StartCoroutine(processLoading());
    }
 

    IEnumerator processLoading()
    {
        while (true)
        {
            //取得当前操作进度(float型)
            target = op.progress;

            if (target >= 0.9f)
            {
                //加载进度完毕，跳出协程
                target = 1;
                yield break;
            }
            //等待下一帧执行
            yield return 0;
        }
    }
}
