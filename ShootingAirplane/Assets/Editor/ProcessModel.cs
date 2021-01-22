using System.Collections;
using System.Collections.Generic;
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
        GameObject _tar = MonoBehaviour.Instantiate(tar, tar.transform.position, tar.transform.rotation);
        /*
         * 在Unity使用Prefab过程中，我们有时候需要进行Prefab实例断开引用和替换引用。
         * 实现这些需求，使用到的核心的类是PrefabUtility。
         * PrefabUtility是一个静态类，主要用于进行Prefab的相关处理。
         */
        //创建prefab,此处有错误，prefab是空引用，后面修改Tag就会报空引用错误，原因不明
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(_tar,"Assets/MyCreate/Enemy2c.prefab");
        //GameObject prefab = PrefabUtility.CreatePrefab("Assets/MyCreate/Enemy2c.prefab", _tar);
        MonoBehaviour.DestroyImmediate(_tar);
        
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
