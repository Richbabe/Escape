using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Escape;

public class GameEventManager : MonoBehaviour {
    //计分的委托和事件
    public delegate void GameScoreAction();
    public static event GameScoreAction myGameScoreAction;

    //游戏结束的委托和事件
    public delegate void GameOverAction();
    public static event GameOverAction myGameOverAction;

    private SceneController scene;

	// Use this for initialization
	void Start () {
        scene = SceneController.getInstance();
        scene.setGameEventManager(this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //英雄逃离敌人，得分
    public void heroEscapeAndScore()
    {
        if(myGameScoreAction != null)
        {
            myGameScoreAction();
        }
    }

    //敌人捕捉到英雄，游戏结束
    public void EnemyHitHeroAndGameover()
    {
        if(myGameOverAction != null)
        {
            myGameOverAction();
        }
    }
}
