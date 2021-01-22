using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRenderer : MonoBehaviour
{
    public Enemy m_enemy;//敌人实例

    // Start is called before the first frame update
    void Start()
    {
        //通过父物体寻找，因为该脚本会挂载在子物体Enemy2上
        m_enemy = GetComponentInParent<Enemy>();
    }

    //进入屏幕
    private void OnBecameVisible()
    {
        //更新Enemy脚本状态
        m_enemy.m_isActive = true;
        //使Enemy获得Renderer
        m_enemy.m_renderer = this.GetComponent<Renderer>();
    }
}
