using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSAction : ScriptableObject {

    public bool enable = true;//动作是否有效
    public bool destory = false;//动作是否销毁

    public GameObject gameobject { get; set; }
    public Transform transform { get; set; }
    public ISSActionCallback callback { get; set; }

    protected SSAction() { }

	// 虚方法，通过重写实现多态
	public virtual void Start () {
        throw new System.NotImplementedException();
	}

    // 虚方法，通过重写实现多态
    public virtual void Update () {
        //Debug.Log("aaa");
        throw new System.NotImplementedException();
    }
}
