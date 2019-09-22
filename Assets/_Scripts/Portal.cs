using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//场景切换脚本:
public class Portal : Colliderable
{
    public string sceneName;

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.name == "Player")
        {
            //储存各类信息
            GameManager.instance.SaveState();

            //切换场景
            //string sceneName = sceneNames[Random.Range(0, sceneNames.Length)];
            SceneManager.LoadScene(sceneName);
        }
    }
}
