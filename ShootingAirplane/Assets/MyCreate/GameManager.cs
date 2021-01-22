using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;//静态实例

    public Transform m_canvas_scoreUI;//分数UI
    public Transform m_canvas_gameoverUI;//游戏失败UI
    public Text m_lifeText;//生命值文本框
    public Text m_scoreText;//分数文本框
    public Text m_bestText;//最高分文本框

    protected int m_score;//分数
    public int m_best;//最高分
    protected Player m_player;//主角实例

    public AudioClip m_audioClip;//背景音乐片段
    protected AudioSource m_audio;//音源

    private void Start()
    {
        instance = this;//静态实例获取本身，此处应该有判断防止线程不安全（省去）

        m_audio = this.gameObject.AddComponent<AudioSource>();//获得音源组件
        m_audio.clip = m_audioClip;//设置声音片段
        m_audio.loop = true;//循环
        m_audio.Play();//播放

        //通过Tag寻找主角实例
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        //通过代码寻找到对应的文本框
        //这三行代码会报错，空指针错误，但是我并没有找到原因，于是注释掉在编辑器中手动关联对应的文本框
        //m_lifeText = m_canvas_scoreUI.transform.Find("Text_life").GetComponent<Text>();
        //m_scoreText = m_canvas_scoreUI.transform.Find("Text_score").GetComponent<Text>();
        //m_bestText = m_canvas_scoreUI.transform.Find("Text_best").GetComponent<Text>();
        //初始化文本框内容
        m_lifeText.text = string.Format("Life : {0}", m_player.m_life);
        m_scoreText.text = string.Format("Score : {0}", m_score);
        m_bestText.text = string.Format("Best : {0}", m_best);

        //获取重新开始Button
        var restarButton = m_canvas_gameoverUI.Find("Button_gameover").GetComponent<Button>();
        //为按钮添加事件
        restarButton.onClick.AddListener(delegate ()
        {
            //重新开始当前关卡
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });

        //隐藏游戏失败UI
        m_canvas_gameoverUI.gameObject.SetActive(false);
    }

    public void AddScore(int point)
    {
        m_score += point;
        //更新最高分
        if(m_best < m_score)
        {
            m_best = m_score;
        }
        //更新UI
        m_scoreText.text = string.Format("Score : {0}", m_score);
        m_bestText.text = string.Format("Best : {0}", m_best);
    }

    public void ChangeLife(int life)
    {
        //更新UI
        m_lifeText.text = string.Format("Life : {0}", life);

        if(life <= 0)
        {
            //生命为0时显示游戏失败UI
            m_canvas_gameoverUI.gameObject.SetActive(true);
        }
    }
}
