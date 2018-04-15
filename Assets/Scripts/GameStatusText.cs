using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//加载在text上的脚本
public class GameStatusText : MonoBehaviour {
    private int score = 0;
    private int textType;//0为score，1为gameover
    private bool isGameOver = false;//是否游戏结束

	// Use this for initialization
	void Start () {
        getTextType();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void getTextType()
    {
        if (gameObject.name.Contains("Score"))
        {
            textType = 0;
        }
        else
        {
            textType = 1;
        }
    }

    private void OnEnable()
    {
        GameEventManager.myGameScoreAction += gameScore;
        GameEventManager.myGameOverAction += gameOver;
    }

    private void OnDisable()
    {
        GameEventManager.myGameScoreAction -= gameScore;
        GameEventManager.myGameOverAction -= gameOver;
    }

    void gameScore()
    {
        if(textType == 0 && !isGameOver)
        {
            Debug.Log("+fen");
            score++;
            this.gameObject.GetComponent<Text>().text = "Score: " + score;
        }
    }
    
    void gameOver()
    {
        if(textType == 1)
        {
            Debug.Log("die!!!");
            this.gameObject.GetComponent<Text>().text = "YOU DIED!";
            isGameOver = true;
        }
    }
}
