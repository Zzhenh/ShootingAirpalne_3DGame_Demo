using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperEnemy : Enemy
{
    public Transform m_rocket;//子弹prefab
    public float m_fireTime = 2;//发射子弹频率
    protected Transform m_player;//主角位置组件
    float fireTimer = 0;//发射子弹时间

    public AudioClip m_shootAudio;
    protected AudioSource m_audio;

    protected override void Start()
    {
        base.Start();
        m_audio = GetComponent<AudioSource>();
    }

    //重写移动方法
    protected override void UpdateMove()
    {
        fireTimer += Time.deltaTime;//定时发射子弹
        if(fireTimer >= m_fireTime)
        {
            fireTimer = 0;
            //当主角存在时，生成子弹，并朝向主角发射
            if (m_player != null)
            {
                Vector3 target = m_player.position - this.transform.position;

                //Quaternion的LookRotation方法返回一个角度
                Instantiate(m_rocket, transform.position, Quaternion.LookRotation(target));
                //播放子弹射击音效
                m_audio.PlayOneShot(m_shootAudio);
            }
            else
            {
                //主角不存在时，寻找主角，并记录
                //这里使用protected标识符使主角位置组件不可在编辑器中设置
                //通过Find方法寻找（原因不明）
                GameObject obj = GameObject.FindGameObjectWithTag("Player");
                if (obj != null)
                {
                    m_player = obj.transform;
                }
            }
        }

        //移动
        transform.Translate(new Vector3(0, 0, -m_speed * Time.deltaTime));
    }
}
