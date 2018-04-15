using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//组合动作实现，由多个SSAction组成的链表，线性顺序
public class CCSequenceActions : SSAction, ISSActionCallback {

    public List<SSAction> actionList;//动作列表
    public int repeatTimes = -1;//序列动作重复次数,-1表示无线循环
    public int subActionIndex = 0;//顺序动作里某个动作的下标

    public static CCSequenceActions CreateSSAction(List<SSAction> sequence,int repeat = 0)
    {
        CCSequenceActions action = ScriptableObject.CreateInstance<CCSequenceActions>();
        action.repeatTimes = repeat;
        action.actionList = sequence;
        return action;
    }

    // 多态，重写基类Start函数
    public override void Start () {
        //执行动作前，为每个动作注入当前动作游戏对象，并将自己作为动作事件的接收者
		foreach (SSAction action in actionList)
        {
            action.gameobject = this.gameobject;
            action.transform = this.transform;
            action.callback = this;
            action.Start();
        }
	}

    // 多态，重写基类Update函数
    public override void Update () {
		if(actionList.Count == 0)
        {
            return;
        }
        else if(subActionIndex < actionList.Count)
        {
            actionList[subActionIndex].Update();
        }
	}

    //提供给子动作回调，提醒动作序列执行下一个动作，如果序列动作完成，通知该动作的动作管理者
    public void SSActionEvent(SSAction source, SSActionEventType eventType = SSActionEventType.Competeted,
        SSActionTargetType intParam = SSActionTargetType.Normal,
        string strParam = null, Object objParam = null)
    {
        source.destory = false;
        this.subActionIndex++;//执行下一个动作
        //执行完整个序列，判断是否循环
        if(this.subActionIndex >= actionList.Count)
        {
            this.subActionIndex = 0;//将下标重置0
            if(repeatTimes > 0)
            {
                repeatTimes--;//循环次数减少
            }
            if(repeatTimes == 0)
            {
                this.destory = true;
                this.callback.SSActionEvent(this);//序列动作完成，通知该动作管理者
            }
        }
    }

    private void OnDestroy()
    {
        
    }
}
