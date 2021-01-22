using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRocket : Rocket
{
    //碰撞检测，碰到主角销毁
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Destroy(this.gameObject);
        }
    }
}
