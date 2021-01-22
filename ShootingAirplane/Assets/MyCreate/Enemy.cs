using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float m_speed = 1;//速度
    public float m_life = 10;//生命值
    public float m_rotationSpeed = 30;//旋转速度
    public int m_point = 10;//分数

    public Renderer m_renderer;//模型渲染组件
    public bool m_isActive = false;//是否显示在屏幕上

    public Transform m_explosionFX;//爆炸特效

    // Start is called before the first frame update
    protected virtual void Start()
    {
        m_renderer = GetComponent<Renderer>();//获取模型渲染组件
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMove();//调用移动函数

        //当敌人进入屏幕再移出屏幕时，销毁
        if(m_isActive && !m_renderer.isVisible)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PlayerRocket")//碰到子弹
        {
            Rocket rocket = other.GetComponent<Rocket>();//获取子弹组件
            if(rocket != null)
            {
                m_life -= rocket.m_power;//减去相应生命值
                if(m_life <= 0)//判断销毁
                {
                    //更新UI
                    GameManager.instance.AddScore(m_point);
                    //生成爆炸特效
                    Instantiate(m_explosionFX, this.transform.position, Quaternion.identity);
                    
                    Destroy(this.gameObject);
                }
            }
        }
        else if(other.tag == "Player")//撞到主角，销毁
        {
            m_life = 0;
            //生成爆炸特效
            Instantiate(m_explosionFX, this.transform.position, Quaternion.identity);

            Destroy(this.gameObject);
        }
    }

    //当敌人进入屏幕时，改变Active值
    private void OnBecameVisible()
    {
        m_isActive = true;
    }

    //虚函数，为了功能扩展
    protected virtual void UpdateMove()
    {
        //左右移动
        float rx = Mathf.Sin(Time.time) * Time.deltaTime;
        //前进（-z方向）
        transform.Translate(new Vector3(rx, 0, -m_speed * Time.deltaTime));
    }
}
