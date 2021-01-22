using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public Transform m_enemy;//敌人prefab

    // Start is called before the first frame update
    void Start()
    {
        //执行协程方法
        StartCoroutine(SpawnEnemy());
    }

    //生成敌人方法
    //返回值为迭代器类型IEnumerator
    IEnumerator SpawnEnemy()
    {
        while(true)
        {
            //迭代器通过yield返回，效果为等待5到15秒。
            yield return new WaitForSeconds(Random.Range(5, 15));
            //生成一个敌人
            Instantiate(m_enemy, this.transform.position, Quaternion.identity);
        }
    }

    //在scene场景中显示图标，但是在游戏中不显示，对应图片必须在Gizmos文件夹中
    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "item.png", true);
    }
}
