# shootingAirplane-太空射击游戏

### 导入资源
将资源文件夹复制到Assets文件夹下。

### 创建场景
**火星背景和星空动画**

1.创建新Scene
2.创建一个平面体Plane
3.创建一个材质球Material，指定贴图为mars.png
4.将材质给到平面，并确保Mode为Cutout
5.创建另一个平面体，放大置于火星下方，创建一个材质球，将shader中选择Unlit-Texture，并指定star.png
6.创建Animator Controller，给到星空模型。
7.打开Window-Animation-Animation创建一个动画文件
8.Add Property，选择Material._Main_Tex_ST添加UV属性。
9.进入录制状态，时间轴前进30帧，将ST属性中的w值设置为-1.

**摄像机和灯光**

1.在scene中调整视角角度
2.选中MainCamera，选择GameObject-Align With View
3.Window-Rendering-LightingSetting，选择AmbientSource，将Skybox改为Color，取消Lightmap的Auto选项
4.GameObject-Light-PointLight置于火星上方，调节Range和Intensity

### 创建主角
**创建脚本**

1.将Player.fbx拖动到Hierachy窗口中创建游戏体
2.创建Script-Player.cs
3.将Player给到游戏体

**控制飞船移动**

1.在Player类中添加控制飞行速度的属性

```c#
public float m_speed = 1;//移动速度
```
2.在update中写入控制游戏物体移动的代码
```c#
void Update()
    {
        float movev = 0;//垂直方向移动
        float moveh = 0;//水平方向移动

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
```


**创建子弹**

1.选择rocket.fbx模型，创建游戏体
2.创建Rocket.cs脚本，给到游戏体

```c#
	public float m_speed = 10;//飞行速度
    public float m_power = 1.0f;//威力

	private void Update()
    {
        transform.Translate(new Vector3(0, 0, m_speed * Time.deltaTime));//子弹移动
    }
    //当子弹飞出屏幕时销毁
    //OnBecameInvisible()在物体离开屏幕时会调用
    private void OnBecameInvisible()
    {
        if(this.enabled)
        {
            Destroy(this.gameObject);
        }
    }
```
3.将子弹从Hierachy窗口中拖拽到Project窗口中创建prefab。

**发射子弹**

1.在Player脚本中添加子弹位置组件的属性。
2.在Player的Inspector窗口中将子弹的prefab给到该属性。
3.在脚本中添加代码生成子弹

```c#
	public float rocketTime = 0.1f;//子弹发射的间隔时间
    private float rocketTimer = 0;//控制子弹发射频率
    
    //update中的代码
	rocketTimer += Time.deltaTime;//定时器
    if(rocketTimer >= rocketTime)
    {
        rocketTimer = 0;
        //空格发射子弹
        if (Input.GetKey(KeyCode.Space))
        {
        	//Instantiate函数生成物体
            Instantiate(rocket_transform, m_transform.position, m_transform.rotation);
        }
    }
```

### 创建敌人
**创建敌人**

1.选择Enemy.fbx创建游戏体
2.创建Enemy.cs脚本。给到游戏体

```c#
	public float m_speed = 1;//速度
    public float m_life = 10;//生命值
    public float m_rotationSpeed = 30;//旋转速度
    
	//虚函数，为了功能扩展
    protected virtual void UpdateMove()
    {
        //左右移动
        float rx = Mathf.Sin(Time.time) * Time.deltaTime;
        //前进（-z方向）
        transform.Translate(new Vector3(rx, 0, m_speed * Time.deltaTime));
    }
```
3.创建为prefab。

### 创建碰撞
**添加碰撞体**

1.给敌人添加Box Colllider，并勾选is Trigger。
2.给敌人添加Rigidbody刚体组件，取消Use Gravity，选中Is Kinematic。
3.apply到prefab
4.同理为主角添加上述组件

**触发碰撞**

1.创建新Tag，PlayerRocket和Enemy
2.将敌人的Tag设置为Enemy，将子弹的Tag设置为PlayerRocket，将主角的Tag设置为Player
3.在Enemy脚本，子弹脚本和主角脚本中添加碰撞判断

```c#
	//敌人
	private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "PlayerRocket")
        {
            Rocket rocket = other.GetComponent<Rocket>();
            if(rocket != null)
            {
                m_life -= rocket.m_power;
                if(m_life <= 0)
                {
                    Destroy(this.gameObject);
                }
            }
        }
        else if(other.tag == "Player")
        {
            m_life = 0;
            Destroy(this.gameObject);
        }
    }
    
    //子弹
	private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            Destroy(this.gameObject);
        }
    }
    
    //主角
    //碰撞检测，撞到敌人减一血
	private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            m_life -= 1;
            if(m_life <= 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
```
4.在Enemy脚本中添加功能移出屏幕时销毁
```c#
	Renderer m_renderer;//模型渲染组件
    bool m_isActive = false;//是否显示在屏幕上
    
    void Start()
    {
        m_renderer = GetComponent<Renderer>();//获取模型渲染组件
    }
    
    //update中代码
    if(m_isActive && !m_renderer.isVisible)
    {
        Destroy(this.gameObject);
    }
    //
    
    //当敌人进入屏幕时，改变Active值
    private void OnBecameVisible()
    {
        m_isActive = true;
    }
```

### 高级敌人
**创建敌人**

1.通过Enemy2.fbx创建prefab
2.创建SuperEnemy.cs脚本，继承Enemy，重写UpdateMove方法

```c#
//重写移动方法
    protected override void UpdateMove()
    {
        transform.Translate(new Vector3(0, 0, -m_speed * Time.deltaTime));
    }
```
3.将敌人的Tag设置为Enemy，添加BoxCollider和Rigidbody组件，设置同前，修改生命为50.

**敌人发射子弹**

1.使用Rocket.fbx创建一个新的prefab,使用rocket2.png作为贴图，添加Rigidbody和boxCollider组件
2.添加一个EnemyRocket的Tag
3.创建EnemyRocket.cs脚本，并继承Rocket类 。

```c#
	private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Destroy(this.gameObject);
        }
    }
```
4.修改SuperEnemy脚本
```c#
	public Transform m_rocket;//子弹prefab
    public float m_fireTime = 2;//发射子弹频率
    public Transform m_player;//主角位置组件
    float fireTimer = 0;//发射子弹时间
    
    //UpdateMove中的代码
    fireTimer += Time.deltaTime;//定时发射子弹
    if(fireTimer >= m_fireTime)
    {
        fireTimer = 0;
        //当主角存在时，生成子弹，并朝向主角发射
        if (m_player != null)
        {
            Vector3 target = m_player.position - this.transform.position;

            Instantiate(m_rocket, transform.position, Quaternion.LookRotation(target));
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
    //
```
5.为Player脚本中的碰撞检测加入敌人子弹的判断

### 声音与特效
**声音与特效**

1.Import Package-Custom Package-选择shootingFX.unitypackage，导入。
2.主角添加AudioSource组件
3.修改Player脚本

```c#
	public AudioClip m_shootAudio;//射击音效
    public Transform m_explosionFX;//爆炸特效
    protected AudioSource m_audio;//audioSource组件

	//start中添加代码
	//获取声音组件
    m_audio = GetComponent<AudioSource>();
	//
	
	//在生成子弹后添加代码
	//子弹音效播放
    m_audio.PlayOneShot(m_shootAudio);
    //
    
    //在销毁前添加代码
    //生成爆炸特效,identity为固定旋转
    Instantiate(m_explosionFX, m_transform.position, Quaternion.identity);
    //
```
4.将对应的音效和特效给到主角，为爆炸prefab添加一个audioSource组件，并关联对应音效（Spatial Blend值为0代表2D音效，值为1代表3D音效）
5.敌人效果添加相同，这里要注意，高级敌人会发射子弹，但是普通敌人不会，如果只给高级敌人添加射击音效，Enemy类的Start方法要改为虚方法，因为SuperEnemy中不仅要用到Enemy中的Start方法，还要添加获取音源的代码，如果直接写一个start方法，会把父类的start覆盖掉，写为虚方法后可以调用base.start()，这样就可以不影响普通敌人并且直接添加功能了。

### 敌人生成器
**敌人生成器**

1.创建EnemySpawn脚本，使用协程来创建敌人生成器，使生成器每几秒创建一个敌人。协程是迭代器的一种应用，实现异步的效果。

```c#
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
```
2.创建一个空的游戏体作为生成器
3.将敌人生成器制作为两个prefab，分别对应不同的敌人
4.使敌人生成器在场景中显示出来，但是不在游戏中显示出来
5.创建一个Gizmos文件夹，将item.png复制到该文件夹下
6.在EnemySpawn脚本中添加代码
```c#
	//在scene场景中显示图标，但是在游戏中不显示，对应图片必须在Gizmos文件夹中
    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, "item.png", true);
    }
```

### 游戏UI和战斗管理
**UI界面**

1.创建一个Canvas画布
2.在Canvas上创建3个Text组件，记录生命，得分，最高分，调整位置和字体
3.新建一个Canvas画布Canvas_gameover，创建Text组件显示“游戏失败”，再创建一个按钮，文字为“重新开始”

**UI功能**

1.创建脚本GameManager.cs（单例模式，单例模式通常应用于只创建一个实例的类，可以通过其类内部的静态实例调用，可以省去重复创建实例的过程，有多种写法，分为线程安全或不安全）

```c#
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
        m_lifeText = m_canvas_scoreUI.transform.Find("Text_life").GetComponent<Text>();
        m_scoreText = m_canvas_scoreUI.transform.Find("Text_score").GetComponent<Text>();
        m_bestText = m_canvas_scoreUI.transform.Find("Text_best").GetComponent<Text>();
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
```
2.创建一个空的游戏体GameManager并将脚本给到该空物体，关联对应的物体和资源
3.再Enemy脚本中添加分数，在OnTriggerEnter中调用GameManager中的方法记录分数
```c#
	public int m_point = 10;//分数
	
	
	//更新UI,在生成爆炸特效之前
	GameManager.instance.AddScore(m_point);
```
4.在Player脚本中调用GameManager中的方法改变生命值，更新UI
```c#
	//更新UI，在修改生命值之后
    GameManager.instance.ChangeLife(m_life);
```

### 关卡跳转
**关卡跳转**

1.创建一个新的Scene-start
2.创建一个TitleScene的脚本，将脚本给到摄像机（因为摄像机一直存在）

```c#
	//按钮的回调方法
    public void OnButtonGameStart()
    {
        //读取关卡level1
        SceneManager.LoadScene("level1");
    }
```
3.创建一个UI-Canvas，在其上创建一个Image，并给予一个图片（图片要从Texture类型转到Sprite类型）
4.创建一个按钮，在编辑器中单击OnClick()下的+，添加回调方法（将摄像机最为接收对象，选择TitleScene中的方法）
5.在File-Build Setting中添加关卡

### 鼠标控制主角
**鼠标控制**

1.创建一个3d object-Quad，使其铺满整个屏幕并使y轴坐标与主角相近
2.取消选中Mesh Renderer隐藏物体的显示
3.新建一个Layer，将该物体设置为新的Layer
4.修改Player脚本

```c#
	protected Vector3 m_targetPos;//目标位置
    public LayerMask m_inputLayer;//鼠标射线碰撞层
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
```

### 精准的碰撞检测
**碰撞检测**

1.通过Enemy2b.fbx制作prefab
2.将col子物体添加Mesh Collider，并取消选中Mesh Renderer，隐藏显示，只用作碰撞检测
3.创建脚本EnemyRenderer，给到Enemy2子物体。

```c#
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
```

### 自动创建Prefab
**代码创建**

1.创建一个Editor的文件夹，在其中创建一个ProcessModel的脚本，实现自动创建prefab的功能
2.ProcessModel继承AssetPostprocessor类，该类用来处理资源导入，并引入UnityEditor的命名空间，这样就能使用Unity编辑器的API

```c#
using UnityEngine;
using UnityEditor;

/*AssetPostprocessor:这是一个编辑器类，
 * 如果想使用它你需要把它放到工程目录下的Assets/Editor文件夹下。
 * 编辑器类在UnityEditor命名空间下。
 * 所以当使用C#脚本时，你需要在脚本前面加上 "using UnityEditor"引用
 * 它是资源导入的一个管理器，
 * 在资源导入之前和之后可以根据导入的资源做一些设置和一些数据的修改，
 * 比如网格，纹理的压缩，模型添加组件等。
 * 当资源导入之前和之后都会发送通知，可以根据不同的资源类型，
 * 在导入之前和之后做不同的处理。
*/
public class ProcessModel : AssetPostprocessor
{
    //在子类中加入这个函数，以便在模型载入之后获得一个通知。(机翻）
    //导入模型后会自动调用
    //参数即为导入的模型
    void OnPostprocessModel(GameObject input)
    {
        //只处理一种模型
        if (input.name != "Enemy2b")
            return;

        //取得导入模型的相关信息
        //ModelImporter让你从编辑器脚本修改模型导入设置。
        //assetImporter是AssetPostprocessor类中的属性，是资源的引用
        ModelImporter importer = assetImporter as ModelImporter;
        /*AssetDatabase:资源数据库
         * 资源数据库(AssetDatabase) 是允许您访问工程中的资源的 API。
         * 此外，其提供方法供您查找和加载资源，还可创建、删除和修改资源。
         * Unity 编辑器(Editor) 在内部使用资源数据库(AssetDatabase) 追踪资源文件，
         * 并维护资源和引用资源的对象之间的关联。
         * Unity 需要追踪工程文件夹发生的所有变化，
         * 如需访问或修改资源数据，您应始终使用资源数据库(AssetDatabase) API，
         * 而非文件系统。 
         * 资源数据库(AssetDatabase) 接口仅适用于编辑器，不可用于内置播放器。
         * 和所有其他编辑器类一样，其只适用于置于编辑器(Editor) 文件夹中的脚本
         * （只在主要的资源(Assets) 文件夹中创建名为“编辑器”的文件夹
         * （不存在该文件夹的情况下））。
        */
        //LoadAssetAtPath：通过路径载入资源
        GameObject tar = AssetDatabase.LoadAssetAtPath<GameObject>(importer.assetPath);
        GameObject _tar = GameObject.Instantiate(tar, tar.transform.position, tar.transform.rotation);
        /*
         * 在Unity使用Prefab过程中，我们有时候需要进行Prefab实例断开引用和替换引用。
         * 实现这些需求，使用到的核心的类是PrefabUtility。
         * PrefabUtility是一个静态类，主要用于进行Prefab的相关处理。
         */
        //创建prefab，这里使用SaveAsPrefabAsset，需要先实例化，再销毁，否则会报错（原因不明，据说是bug）。
        //此处有错误，prefab是空引用，后面修改Tag就会报空引用错误，原因不明,未修改
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(_tar,"Assets/MyCreate/Enemy2c.prefab");
        GameObject.DestroyImmediate(_tar);
        //设置标签
        prefab.tag = "Enemy";

        //在子物体中查找碰撞模型
        foreach(Transform obj in prefab.GetComponentInChildren<Transform>())
        {
            if(obj.name == "col")
            {
                //取消碰撞模型的显示
                MeshRenderer r = obj.GetComponent<MeshRenderer>();
                r.enabled = false;

                //添加MeshCollider组件
                if(obj.gameObject.GetComponent<MeshCollider>() == null)
                {
                    obj.gameObject.AddComponent<MeshCollider>();
                }
                //设置标签
                obj.tag = "Enemy";
            }
        }

        //添加Rigidbody组件并设置
        Rigidbody rigid = prefab.AddComponent<Rigidbody>();
        rigid.useGravity = false;
        rigid.isKinematic = true;

        //添加AudioSource组件
        prefab.AddComponent<AudioSource>();

        //载入敌人子弹的prefab
        GameObject rocket = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/MyCreate/EnemyRocket.prefab");

        //载入爆炸特效的prefab
        GameObject fx = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/FX/Explosion.prefab");

        //添加高级敌人脚本并设置
        SuperEnemy enemy = prefab.AddComponent<SuperEnemy>();
        enemy.m_life = 50;
        enemy.m_point = 50;
        enemy.m_rocket = rocket.transform;
        enemy.m_explosionFX = fx.transform;
    }
}
```

### 发布游戏
**发布游戏**

1.Edit-Project Settings-Player设置游戏名，公司名，图标。
2.Resolution设置游戏窗口的大小，Run In Background使游戏运行在后台，Display Resolution Dialog设置为Enable，游戏在启动时会出现一个设置分辨率的窗口
3.Icon设置不同大小的图标，专业版选中Show Splash Screen可以消除Unity图标
4.打开Build Setting窗口，单机Build编译为.exe文件

### 对象池
最后一节提到了一个对象池功能，学习中就略过了，之后有时间再学习此部分内容

### 重要知识点
**知识点**

1.transform.Translate方法用于移动位置，该方法有两个参数（第二个可选），第一个是Vector3类型的向量，第二个参数为坐标系，有Self自身坐标系和World世界坐标系。
2.Vector3与坐标的区别。Vector3是向量，Vector3类也提供向量的很多运算。
3.获得目标的方向：运用向量减法，目标位置减自身位置得出从自身指向目标的向量。
4.Quaternion四元数是Unity用来表示角度的类，但使用起来不方便，Unity也提供欧拉角。如：

```c#
	Vector3 v = target - pos;//获得方向
	Quaternion.LookRotation(v);//返回四元数
	Vector3.Angle(Vector3.forward, v);//返回欧拉角
```
5.Rigidbody（刚体）和Collider（碰撞体）的关系是什么。Rigidbode组件提供所有的物理模拟功能，包括重力，推进力等等，Collider组件提供用于碰撞检测的多边形数据，包括立方体，球体等等。在物理模拟或碰撞检测中，Rigidbody是必须的组件，但参与物理模拟的对象只有添加了Collider组件才能计算碰撞。
6.对象池。系统回收的过程称为垃圾回收。频繁的创建或删除对象，会造成频繁的垃圾回收，影响性能。为了避免垃圾回收，对游戏运行中的对象需要有一个预估并提前创建出来，称作创建对象池。当需要创建新的实例时，就从对象池中取出一个已经创建好的对象来使用，当实例需要销毁时，就将它放回对象池。