using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadDisplayControl : MonoBehaviour
{
    public GameObject enemys;
    public GameObject trans;
    private int num;

    private void Update()
    {
        //当敌人全部消灭，开启道路
        num = enemys.GetComponentsInChildren<SpriteRenderer>().Length;
        if (num == 0)
        {
            DisplayRoad();
        }
    }

    public void DisplayRoad()
    {
        trans.gameObject.SetActive(true);
    }
}
