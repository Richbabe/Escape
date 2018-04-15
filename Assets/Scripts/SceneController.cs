using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Escape
{
    //定义方向类
    public class Direction
    {
        public const int UP = 0;
        public const int DOWN = 2;
        public const int LEFT = -1;
        public const int RIGHT = 1;
    }

    //定义栅栏位置
    public class FenchLocation
    {
        public const float FenchHori = 12.42f;//中间栅栏位置,z值
        public const float FenchVertLeft = -3.0f;//左边栅栏位置,x值
        public const float FenchVertRight = 3.0f;//右边栅栏位置,x值
    }

    public interface UserAction
    {
        //控制英雄移动
        void heroMove(int dir);
    }

    public interface IAddAction
    {
        //添加巡逻动作
        void addRandomMovement(GameObject sourceObj, bool isActive);
        //添加抓捕动作
        void addDirectMovement(GameObject sourceObj);
    }

    public interface GameStatus
    {
        //获得英雄所在区域
        int getHeroStandOnArea();
        //英雄逃脱并得分
        void heroEscapeAndScore();
        //敌人碰到英雄 游戏结束
        void EnemyHitHeroAndGameover();
    }

    public class SceneController : System.Object, UserAction, IAddAction, GameStatus
    {
        private static SceneController instance;
        private GameModel myGameModel;
        private GameEventManager myGameEventManager;

        public static SceneController getInstance()
        {
            if(instance == null)
            {
                instance = new SceneController();
            }
            return instance;
        }

        internal void setGameModel(GameModel _myGameModel)
        {
            if (myGameModel == null)
            {
                myGameModel = _myGameModel;
            }
        }
        internal void setGameEventManager(GameEventManager _myGameEventManager)
        {
            if(myGameEventManager == null)
            {
                myGameEventManager = _myGameEventManager;
            }
        }

        //实现UserAction接口，控制英雄运动
        public void heroMove(int dir)
        {
            myGameModel.heroMove(dir);
        }

        //实现IAddAction接口，为enemy添加动作
        public void addRandomMovement(GameObject sourceObj, bool isActive)
        {
            myGameModel.addRandomMovement(sourceObj, isActive);
        }
        public void addDirectMovement(GameObject sourceObj)
        {
            myGameModel.addDirectMovement(sourceObj);
        }

        //实现GameStatus接口
        public int getHeroStandOnArea()
        {
            return myGameModel.getHeroStandOnArea();
        }

        public void heroEscapeAndScore()
        {
            myGameEventManager.heroEscapeAndScore();
        }

        public void EnemyHitHeroAndGameover()
        {
            myGameEventManager.EnemyHitHeroAndGameover();
        }

    }
}

