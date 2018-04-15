using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Escape;

//挂载在敌人上的脚本
public class EnemyBehaviour : MonoBehaviour {
    private IAddAction addAciton;
    private GameStatus gameStatus;

    public int enemyIndex;//该敌人的序号
    public bool isCatching;//是否感知到英雄

    // Use this for initialization
    void Start() {
        addAciton = SceneController.getInstance() as IAddAction;
        gameStatus = SceneController.getInstance() as GameStatus;
        enemyIndex = getEnemyIndex();
        isCatching = false;
    }

    // Update is called once per frame
    void Update() {
        enemyMove();
    }

    int getEnemyIndex()
    {
        string name = this.gameObject.name;
        char index = name[name.Length - 1];//获得char类型的序号(敌人名字为Enemy+index)
        int result = index - '0';
        return result;
    }

    void enemyMove()
    {
        //如果英雄在自己区域
        if(gameStatus.getHeroStandOnArea() == enemyIndex)
        {
            //如果是巡逻状态则切换为抓捕状态
            if (!isCatching)
            {
                isCatching = true;
                addAciton.addDirectMovement(this.gameObject);
            }
        }
        //如果英雄不在自己区域
        else
        {
            //如果是抓捕状态则切换为巡逻状态，玩家逃脱成功，得分！
            if (isCatching)
            {
                gameStatus.heroEscapeAndScore();
                isCatching = false;
                addAciton.addRandomMovement(this.gameObject, false);
            }
        }
    }

    //检测敌人碰撞函数，如果碰撞则反方向运动
    void OnCollisionStay(Collision collision)
    {
        //撞击墙、栅栏、其他敌人
        if (collision.gameObject.name.Contains("chr_zombie3") || collision.gameObject.name.Contains("fence")
            || collision.gameObject.tag.Contains("FenceAround")){
            isCatching = false;
            //Debug.Log("zhuangqiang!!!");
            addAciton.addRandomMovement(this.gameObject, false);
        }

        //撞击英雄，游戏结束！
        if (collision.gameObject.name.Contains("Hero"))
        {
            gameStatus.EnemyHitHeroAndGameover();
            Debug.Log("GAME OVER!!!!");
        }
    }
}
