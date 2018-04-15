using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Escape;

public class UserInput : MonoBehaviour {
    private UserAction action;

	// Use this for initialization
	void Start () {
        action = SceneController.getInstance() as UserAction;
	}
	
	// Update is called once per frame
	void Update () {
        detectKeyInput();
	}

    //检测用户键盘输入
    void detectKeyInput()
    {
        //按下W键英雄向上运动
        if (Input.GetKey(KeyCode.W))
        {
            action.heroMove(Direction.UP);
        }
        //按下S键英雄向下运动
        if (Input.GetKey(KeyCode.S))
        {
            action.heroMove(Direction.DOWN);
        }
        //按下A键英雄向左运动
        if (Input.GetKey(KeyCode.A))
        {
            action.heroMove(Direction.LEFT);
        }
        //按下D键英雄向右运动
        if (Input.GetKey(KeyCode.D))
        {
            action.heroMove(Direction.RIGHT);
        }
    }
}
