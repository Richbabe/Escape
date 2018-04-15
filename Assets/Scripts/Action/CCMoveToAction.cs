using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCMoveToAction : SSAction {

    public Vector3 target;//目的地
    public float speed;//移动速度
    public bool isCatching;//判断此动作是否为追捕

    public static CCMoveToAction CreateSSAction(Vector3 p_target,float p_speed,bool p_isCathing)
    {
        CCMoveToAction ccAction = ScriptableObject.CreateInstance<CCMoveToAction>();
        ccAction.target = p_target;
        ccAction.speed = p_speed;
        ccAction.isCatching = p_isCathing;
        return ccAction;
    }

	// 多态，重写基类Start函数
	public override void Start () {
		
	}

    // 多态，重写基类Update函数
    public override void Update() {
        this.transform.position = Vector3.MoveTowards(this.transform.position, target, speed);//朝目的地移动
        //Debug.Log(this.name + "move!");
        //如果到达目的地
        if(this.transform.position == target)
        {
            this.destory = true;//销毁动作
            //如果不是追捕动作，回调函数传递参数
            if (!isCatching)
            {
                this.callback.SSActionEvent(this);
            }
            //如果是追捕动作，回调函数传递参数
            else
            {
                this.callback.SSActionEvent(this, SSActionEventType.Competeted,SSActionTargetType.Catching);
            }
        }
	}
}
