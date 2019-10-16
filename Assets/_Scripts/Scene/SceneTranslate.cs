using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTranslate : MonoBehaviour
{
    public Canvas SCUI;         //场景切换Canvas
    private Slider SCUISlider;   //SCUI下的Slider进度条
    private float target = 0;   //场景加载值
    private float dtimer = 0;

    AsyncOperation op = null;   //异步操作协同程序

    private void Start()
    {
        if (SCUI == null)
            SCUI = GameObject.Find("SCUI").GetComponent<Canvas>();
        
        SCUI.gameObject.SetActive(false);
        SCUISlider = SCUI.GetComponentInChildren<Slider>();
    }

    private void Update()
    {
        UpdateSlider();
    }

    public void ChangeToScene(string sceneName)
    {
        //开启场景切换时的UI：黑布+进度条
        SCUI.gameObject.SetActive(true);
        SCUISlider.value = 0;

        //开启异步加载协程
        StartCoroutine(ProcessLoading(sceneName));
    }

    private void UpdateSlider()
    {
        if (op != null)
        {
            if (op.progress >= 0.9f)
            {
                target = 1;
            }
            else
                target = op.progress;

            if (SCUISlider.value > 0.99)
            {
                SCUISlider.value = 1;
                SCUI.gameObject.SetActive(false);
                op.allowSceneActivation = true;
                return;
            }
            else
            {
                SCUISlider.value = Mathf.Lerp(SCUISlider.value, target, dtimer * 0.05f);
                dtimer += Time.deltaTime;
            }
        }
    }

    IEnumerator ProcessLoading(string sceneName)
    {
        //设置异步加载的场景
        op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;
        yield return op;
    }
}
