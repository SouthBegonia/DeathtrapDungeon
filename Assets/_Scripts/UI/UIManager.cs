using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("------包含面板------")]
    public CharacterMenu characterMenu;             //装备菜单(左下角)
    public CharacterHUD characterHUD;               //生命值经验值菜单(左上角)
    public FloatingTextManager floatingTextManager; //文本显示

    [Header("------特殊状态机------")]
    public Animator deathMenuAnim;                  //死亡界面动画

    
    private void Start()
    {
        deathMenuAnim.gameObject.SetActive(false);
        UIUpdate();
    }

    //更新游戏数值UI
    public void UIUpdate()
    {
        characterMenu.UpdateMenu();
        characterHUD.UpdateHUD();
    }

    //通用显示Text信息函数:
    public void ShowText(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration)
    {
        floatingTextManager.Show(msg, fontSize, color, position, motion, duration);
    }

    //隐藏死亡动画
    public void HideDeathAnimation()
    {
        deathMenuAnim.SetTrigger("Hide");
        deathMenuAnim.gameObject.SetActive(false);
    }

    //播放死亡动画
    public void ShowDeathAnimation()
    {
        deathMenuAnim.gameObject.SetActive(true);
        deathMenuAnim.SetTrigger("Show");
    }

    public void QuitGame()
    {   
        Application.Quit();
    }
}
