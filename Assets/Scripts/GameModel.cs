using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Escape;

public class GameModel : SSActionManger, ISSActionCallback{
    public GameObject EnemyItem, HeroItem, sceneModelItem;
    private GameObject hero;//英雄对象
    private GameObject sceneModel;//场景对象
    private List<GameObject> EnemyList;//敌人
    private List<int> EnemyLastDir;//保存每个敌人最后的运动方向
    private SceneController scene;

    private const float ENEMY_SPEED_NORMAL = 0.05f;//巡逻状态下敌人的移动速度
    private const float ENEMY_SPEED_CATCHING = 0.08f;//追捕状态下敌人的移动速度

    void Awake()
    {
        EnemyFatory.getInstance().initItem(EnemyItem);
    }

    protected new void Start()
    {
        scene = SceneController.getInstance();
        scene.setGameModel(this);
        hero = Instantiate(HeroItem);
        sceneModel = Instantiate(sceneModelItem);
        genEnemy();
    }

    protected new void Update()
    {
        base.Update();
    }

    //生产敌人
    void genEnemy()
    {
        EnemyList = new List<GameObject>(6);
        EnemyLastDir = new List<int>(6);
        Vector3[] EnemyPos = EnemyFatory.getInstance().getPosSet();//获得敌人位置
        for(int i = 0;i < 6; i++)
        {
            GameObject temp = EnemyFatory.getInstance().getEnemy();
            temp.transform.position = EnemyPos[i];
            temp.name = "enemy" + i;
            EnemyLastDir.Add(-2);//添加一个初始方向，该方向并不存在（-2在Direction中没有定义）
            EnemyList.Add(temp);
            addRandomMovement(temp, true);//为每个敌人添加一个随机方向
        }
    }

    //控制英雄运动函数
    public void heroMove(int dir)
    {
        hero.transform.rotation = Quaternion.Euler(new Vector3(0, dir * 90, 0));//调整英雄朝向
        switch (dir)
        {
            case Direction.UP:
                hero.transform.position += new Vector3(0, 0, 0.1f);//上
                break;
            case Direction.DOWN:
                hero.transform.position += new Vector3(0, 0, -0.1f);//下
                break;
            case Direction.LEFT:
                hero.transform.position += new Vector3(-0.1f, 0, 0);//左
                break;
            case Direction.RIGHT:
                hero.transform.position += new Vector3(0.1f, 0, 0);//右
                break;
        }
    }

    //动作结束后选择添加巡逻动作或者追捕英雄动作
    public void SSActionEvent(SSAction source, SSActionEventType eventType = SSActionEventType.Competeted,
        SSActionTargetType intParam = SSActionTargetType.Normal,
        string strParam = null, Object objParam = null)
    {
        //如果当前是巡逻，则添加巡逻,否则添加抓捕
        if(intParam == SSActionTargetType.Normal)
        {
            addRandomMovement(source.gameobject, true);
        }
        else
        {
            addDirectMovement(source.gameobject);
        }
    }

    //添加巡逻动作函数,isActive表示是否碰到非英雄物体变换方向
    public void addRandomMovement(GameObject sourceObj,bool isActive)
    {
        int index = getIndexOfObj(sourceObj);//获得当前敌人的序号
        int randomDir = getRandomDirection(index, isActive);//获得一个随机方向
        EnemyLastDir[index] = randomDir;//将该随机方向赋值给当前敌人

        sourceObj.transform.rotation = Quaternion.Euler(new Vector3(0, randomDir * 90, 0));//更新敌人头朝方向
        Vector3 target = sourceObj.transform.position;//目标位置
        //根据随机方向计算目标位置
        switch (randomDir)
        {
            //上
            case Direction.UP:
                target += new Vector3(0, 0, 1);
                break;
            //下
            case Direction.DOWN:
                target += new Vector3(0, 0, -1);
                break;
            //左
            case Direction.LEFT:
                target += new Vector3(-1, 0, 0);
                break;
            //右
            case Direction.RIGHT:
                target += new Vector3(1, 0, 0);
                break;
        }
        //Debug.Log(sourceObj.name + target.x + " " + target.y + " " + target.z);
        addSingleMoving(sourceObj, target, ENEMY_SPEED_NORMAL, false);
    }

    //获得敌人的序号
    int getIndexOfObj(GameObject sourceObj)
    {
        string name = sourceObj.name;
        char index = name[name.Length - 1];//获得char类型的序号(敌人名字为Enemy+index)
        int result = index - '0';
        return result;
    }

    //获得巡逻时的随机方向
    int getRandomDirection(int index,bool isActive)
    {
        int randomDir = Random.Range(-1, 3);
        //当发生碰撞或者敌人离开所属区域时，不走同方向
        if (!isActive)
        {
            while(EnemyLastDir[index] == randomDir || EnemyOutOfArea(index, randomDir))
            {
                randomDir = Random.Range(-1, 3);//重置随机方向
            }
        }
        //当非碰撞时，不走反方向
        else
        {
            while(EnemyLastDir[index] == 0 && randomDir == 2
                || EnemyLastDir[index] == 2 && randomDir == 0
                || EnemyLastDir[index] == 1 && randomDir == -1
                || EnemyLastDir[index] == -1 && randomDir == 1
                || EnemyOutOfArea(index, randomDir))
            {
                randomDir = Random.Range(-1, 3);//重置随机方向
            }
        }
        return randomDir;
    }

    //判断敌人走出了自己的区域
    bool EnemyOutOfArea(int index, int randomDir)
    {
        Vector3 enemyPos = EnemyList[index].transform.position;//获得敌人的位置
        float posX = enemyPos.x;//敌人位置的x坐标
        float posZ = enemyPos.z;//敌人位置的z坐标
        //判断敌人在按照randomDir运动后是否离开其所属空间
        switch (index)
        {
            //从左往右，从上往下对敌人排序
            //0号敌人，判断往右或者往下会不会离开所属空间
            case 0:
                if(randomDir == 1 && posX + 1 > FenchLocation.FenchVertLeft 
                    || randomDir == 2 && posZ - 1 < FenchLocation.FenchHori)
                {
                    return true;
                }
                break;
            //1号敌人，判断往右或者往左或者往下会不会离开所属空间
            case 1:
                if(randomDir == 1 && posX + 1 > FenchLocation.FenchVertRight
                    || randomDir == -1 && posX - 1 < FenchLocation.FenchVertLeft
                    || randomDir == 2 && posZ - 1 < FenchLocation.FenchHori)
                {
                    return true;
                }
                break;
            //2号敌人，判断往左或者往下会不会离开所属空间
            case 2:
                if(randomDir == -1 && posX - 1 < FenchLocation.FenchVertRight
                    || randomDir == 2 && posZ - 1 < FenchLocation.FenchHori)
                {
                    return true;
                }
                break;
            //3号敌人，判断往右或者往上会不会离开所属空间
            case 3:
                if(randomDir == 1 && posX + 1 > FenchLocation.FenchVertLeft
                    || randomDir == 0 && posZ + 1 > FenchLocation.FenchHori)
                {
                    return true;
                }
                break;
            //4号敌人，判断往右或者往左或者往上会不会离开所属空间
            case 4:
                if(randomDir == 1 && posX + 1 > FenchLocation.FenchVertRight
                    || randomDir == -1 && posX - 1 < FenchLocation.FenchVertLeft
                    || randomDir == 0 && posZ + 1 > FenchLocation.FenchHori)
                {
                    return true;
                }
                break;
            //5号敌人，判断往做或者往上会不会离开所属空间
            case 5:
                if(randomDir == -1 && posX - 1 < FenchLocation.FenchVertRight
                    || randomDir == 0 && posZ + 1 > FenchLocation.FenchHori)
                {
                    return true;
                }
                break;
        }
        return false;
    }

    //添加追捕英雄动作
    public void addDirectMovement(GameObject sourceObj)
    {
        int index = getIndexOfObj(sourceObj);//获得当前敌人的序号
        EnemyLastDir[index] = -2;//初始化其方向

        sourceObj.transform.LookAt(hero.transform.position);//设置敌人头朝向
        Vector3 direction = hero.transform.position - sourceObj.transform.position;//追捕方向为英雄位置向量减去敌人位置向量
        Vector3 target = new Vector3(direction.x / 4.0f, 0, direction.z / 4.0f);//标准化
        target += sourceObj.transform.position;
        addSingleMoving(sourceObj, target, ENEMY_SPEED_CATCHING, true);
    }

    //添加单一动作
    void addSingleMoving(GameObject sourceObj,Vector3 target,float speed,bool isCatching)
    {
        this.addAction(sourceObj, CCMoveToAction.CreateSSAction(target, speed, isCatching), this);
    }

    //添加组合动作
    void addCombineMoving(GameObject sourceObj,Vector3[] target,float[] speed,bool isCatching)
    {
        List<SSAction> acList = new List<SSAction>();
        for(int i = 0;i < target.Length; i++)
        {
            acList.Add(CCMoveToAction.CreateSSAction(target[i], speed[i], isCatching));
        }
        CCSequenceActions MoveSeq = CCSequenceActions.CreateSSAction(acList);
        this.addAction(sourceObj, MoveSeq, this);
    }

    //获取hero所在区域
    public int getHeroStandOnArea()
    {
        return hero.GetComponent<HeroStatus>().standOnArea;
    }
}
