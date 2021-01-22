using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float m_speed = 10;//飞行速度
    public float m_power = 1.0f;//威力

    private void Update()
    {
        //子弹移动
        transform.Translate(new Vector3(0, 0, m_speed * Time.deltaTime));
    }

    //撞到敌人销毁
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            Destroy(this.gameObject);
        }
    }

    //当子弹飞出屏幕时销毁
    //OnBecameInvisible()在物体离开屏幕时会调用
    private void OnBecameInvisible()
    {
        if (this.enabled)
        {
            Destroy(this.gameObject);
        }
    }
}
