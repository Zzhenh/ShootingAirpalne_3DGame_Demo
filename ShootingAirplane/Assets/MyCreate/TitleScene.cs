using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    //按钮的回调方法
    public void OnButtonGameStart()
    {
        //读取关卡level1
        SceneManager.LoadScene("level1");
    }
}
