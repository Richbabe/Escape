using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//动作管理基类
public class SSActionManger : MonoBehaviour {

    private Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();
    private List<SSAction> waitingAdd = new List<SSAction>();//等待加入动作列表
    private List<int> waitingDelete = new List<int>();//等待删除动作的下标列表

	// Use this for initialization
	protected void Start () {

	}
	
	// 设置为protected类型，只有该类及其派生类能够调用Update()函数
	protected void Update () {
        //Debug.Log(waitingAdd.Count);

        //把等待加入动作列表中的动作加入
		foreach(SSAction ac in waitingAdd)
        {
            actions[ac.GetInstanceID()] = ac;
        }
        waitingAdd.Clear();

        foreach(KeyValuePair<int,SSAction> kv in actions)
        {
            SSAction ac = kv.Value;
            //如果该动作需要删除，则将其下标加入等待删除动作的下标列表
            if (ac.destory)
            {
                waitingDelete.Add(ac.GetInstanceID());
            }
            else if (ac.enable)
            {
                ac.Update();
                //Debug.Log("aaa");
            }
        }

        //删除等待删除的动作
        foreach(int key in waitingDelete)
        {
            SSAction ac = actions[key];
            actions.Remove(key);
            DestroyObject(ac);
        }
        waitingDelete.Clear();
	}

    //当对某一对象添加新的动作时
    public void addAction(GameObject gameObject,SSAction action,ISSActionCallback manager)
    {
        //Debug.Log(gameObject.name + " add Action!");
        //先把该对象现有的动作销毁
        for(int i = 0;i < waitingAdd.Count; i++)
        {
            if (waitingAdd[i].gameobject.Equals(gameObject))
            {
                SSAction ac = waitingAdd[i];
                waitingAdd.RemoveAt(i);
                i--;
                DestroyObject(ac);
            }
        }
        foreach(KeyValuePair<int,SSAction> kv in actions)
        {
            SSAction ac = kv.Value;
            if (ac.gameobject.Equals(gameObject))
            {
                ac.destory = true;
            }
        }

        //添加新的动作
        action.gameobject = gameObject;
        action.transform = gameObject.transform;
        action.callback = manager;
        waitingAdd.Add(action);
        action.Start();
    }
}
