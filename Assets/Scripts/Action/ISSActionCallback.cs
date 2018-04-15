using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SSActionEventType : int { Started, Competeted }//事件类型，保存动作是开始执行还是执行完毕
public enum SSActionTargetType : int { Normal, Catching}//动作目的地类型，保存动作是巡逻还是追捕

//动作处理接口，所有动作管理器都必须实现这个接口来实现事件调度，同时组合动作也需要实现它才能进行动作的切换
public interface ISSActionCallback {
    //动作结束时回调，需要告知是哪种动作
    void SSActionEvent(SSAction source, SSActionEventType eventType = SSActionEventType.Competeted,
        SSActionTargetType intParam = SSActionTargetType.Normal,
        string strParam = null, Object objParam = null);
}
