using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss0 : Enemy
{
    public bool ___________;
    public Transform[] fireballs;
    public float[] fireballSpeed = { 2.5f, -2.5f };
    private SpriteRenderer spriteRenderer;

    public float fireballDistance = 0.25f;      //火球间距

    private float startTriggerLength;
    private float startChaseLength;

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();

        //记录追逐参数原始值
        startTriggerLength = triggerLength;
        startChaseLength = chaseLength;

        //修改该boss的受伤免疫时间
        ImmuneTime = 0.2f;
    }

    protected override void Update()
    {
        base.Update();

        for(int i = 0; i < fireballs.Length; i++)
        {
            fireballs[i].position = transform.position + new Vector3(-Mathf.Cos(Time.time * fireballSpeed[i]) * fireballDistance, Mathf.Sin(Time.time * fireballSpeed[i]) * fireballDistance, 0);
        }

        //当boss残血时,绕身的火球加速,boss移速增加,追逐范围/距离增加
        if (((float)hitPoint / (float)maxHitPoint) <= 0.2f)
        {
            fireballSpeed[0] = 4f;
            fireballSpeed[1] = -4f;

            speedMultiple = 1f;
            triggerLength = startTriggerLength * 2;
            chaseLength = startChaseLength * 2;

            spriteRenderer.color = Color.red;
        }
    }
}
