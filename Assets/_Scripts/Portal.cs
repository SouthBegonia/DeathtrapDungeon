using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//场景切换脚本:
public class Portal : Colliderable
{
    public string sceneNames;

    public Canvas SCUI;         //场景切换Canvas
    public Slider SCUISlider;   //SCUI下的Slider进度条
    private float target = 0;
    private float dtimer = 0;
    AsyncOperation op = null;
    

    protected override void Start()
    {
        base.Start();
        GetComponent<BoxCollider2D>().enabled = true;
        SCUI.gameObject.SetActive(false);
        //SCUISlider.gameObject.SetActive(false);
    }

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.name == "Player")
        {
            //储存各类信息
            GameManager.instance.SaveState();

            //切换场景：旧方法直接切换
            //string sceneName = sceneNames[Random.Range(0, sceneNames.Length)];
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
            op.allowSceneActivation = true;
        }
    }

    public void ChangeScene()
    {
        //SCUISlider.gameObject.SetActive(true);
        SCUI.gameObject.SetActive(true);
        GetComponent<BoxCollider2D>().enabled = false;
        op = SceneManager.LoadSceneAsync(sceneNames);
        op.allowSceneActivation = false;
        SCUISlider.value = 0;

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
                target = 1;
                yield break;
            }
            yield return 0;
        }
    }
}
