using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float m_speed = 1;//移动速度
    public int m_life = 3;//生命值
    private Transform m_transform;//自身位置组件
    public Transform rocket_transform;//子弹位置组件
    public float rocketTime = 0.1f;//子弹发射的间隔时间
    private float rocketTimer = 0;//控制子弹发射频率
    public AudioClip m_shootAudio;//射击音效
    public Transform m_explosionFX;//爆炸特效
    protected AudioSource m_audio;//audioSource组件
    protected Vector3 m_targetPos;//目标位置
    public LayerMask m_inputLayer;//鼠标射线碰撞层

    // Start is called before the first frame update
    void Start()
    {
        //获取游戏物体的位置组件（避免Update中重复获取浪费资源）
        m_transform = this.transform;
        //获取声音组件
        m_audio = GetComponent<AudioSource>();
        //初始化位置
        m_targetPos = m_transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //鼠标
        MoveTo();

        //键盘
        float movev = 0;//垂直方向移动
        float moveh = 0;//水平方向移动

        rocketTimer += Time.deltaTime;//定时器
        if(rocketTimer >= rocketTime)
        {
            rocketTimer = 0;
            //空格发射子弹
            if (Input.GetKey(KeyCode.Space))
            {
                //生成子弹
                Instantiate(rocket_transform, m_transform.position, m_transform.rotation);

                //子弹音效播放
                m_audio.PlayOneShot(m_shootAudio);
            }
        }
        
        //上方向键
        if(Input.GetKey(KeyCode.UpArrow))
        {
            movev += m_speed * Time.deltaTime;
        }
        //下方向键
        if (Input.GetKey(KeyCode.DownArrow))
        {
            movev -= m_speed * Time.deltaTime;
        }
        //左方向键
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveh -= m_speed * Time.deltaTime;
        }
        //右方向键
        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveh += m_speed * Time.deltaTime;
        }

        //移动游戏物体
        m_transform.Translate(new Vector3(moveh, 0, movev));
        //等价于
        //this.transform.position += new Vector3(moveh, 0, movev));
    }

    //鼠标控制移动方法
    void MoveTo()
    {
        //鼠标左键摁下
        if(Input.GetMouseButton(0))
        {
            //鼠标屏幕位置
            Vector3 pos = Input.mousePosition;
            //射线
            Ray r = Camera.main.ScreenPointToRay(pos);
            //记录碰撞信息的类
            RaycastHit info;
            //发射射线，返回值为是否碰撞，info通过out关键字返回
            //返回到一个RaycastHit类的对象，里面记录下了碰撞点的信息
            bool isCast = Physics.Raycast(r, out info, 1000, m_inputLayer);
            if(isCast)
            {
                m_targetPos = info.point;
            }
        }

        //通过MoveTowards计算新位置，并调整。
        Vector3 target = Vector3.MoveTowards(m_transform.position, m_targetPos, m_speed * Time.deltaTime);
        m_transform.position = target;
    }

    //碰撞检测，撞到敌人减一血
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy" || other.tag == "EnemyRocket")
        {
            m_life -= 1;
            //更新UI
            GameManager.instance.ChangeLife(m_life);
            if(m_life <= 0)
            {
                //生成爆炸特效,identity为固定旋转
                Instantiate(m_explosionFX, m_transform.position, Quaternion.identity);
                //销毁
                Destroy(this.gameObject);
            }
        }
    }
}
