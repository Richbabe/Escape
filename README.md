---
layout:     post
title:      Escape
subtitle:   一款用u3d制作的小游戏
date:       2018-04-15
author:     Richbabe
header-img: img/u3d技术博客背景.jpg
catalog: true
tags:
    - Unity
---
## 引言
为什么要做这个游戏？理由很简单，我想通过这个游戏重温一下大二下学期学的Unity基础知识以及面向对象的几个经典的设计模式，同时为接下来实训要做的AR游戏做准备。废话不多说，我们先来看看我们要做的这个游戏长什么样。

## 游戏规则
![image](https://github.com/Richbabe/Richbabe.github.io/blob/master/img/u3d%E9%A1%B9%E7%9B%AEgif%E5%9B%BE/Escape%E6%B8%B8%E6%88%8F%E8%A7%84%E5%88%99.jpg?raw=true)

## 游戏效果
![image](https://github.com/Richbabe/Richbabe.github.io/blob/master/img/u3d%E9%A1%B9%E7%9B%AEgif%E5%9B%BE/Escape.gif?raw=true)

## 游戏UML图
![image](https://github.com/Richbabe/Richbabe.github.io/blob/master/img/u3d%E9%A1%B9%E7%9B%AEgif%E5%9B%BE/Escape-UML.png?raw=truee)
从游戏的UML图可以看出来我们这个游戏用了如下的设计模式：
* MVC模式
* 单实例模式
* 接口与门面模式
* 动作管理器模式
* 游戏对象工厂模式
* 观察者模式（也成为发布与订阅模式）

接下来我们我们来看看这些模式在游戏中是怎么实现的。

## 游戏的具体实现

### MVC模式和单实例模式的结合

#### MVC模式
MVC模式中的M指的是模型Model，在unity中即GameObject;V指的是视图View，在unity中即Camera；C指的是控制Control，在unity中指的是MonoBehaviour。所谓MVC模式，即把模型、视图以及控制分离，不让他们互相干扰到对方。在UML图中，我们可以看到C的主体是SceneController，其控制着整个游戏脚本的运行，管理场景所有的游戏对象，协调游戏对象之间的通讯等。而GameModel则是M的主体，其作用是加载游戏的模型、定义游戏对象的动作等。

#### 单实例模式
顾名思义，即整个游戏生命周期中只有一个实例，这个实例只需要生成一次，并且知道游戏结束才需要销毁。单实例模式一般应用于管理器类，或者是一些需要持久化存在的对象。在UML图中，我们可以看到Scen
Controller和EnemyFactory是单实例的，这是因为这两个实例在整个游戏生命周期只出现了一次。

首先来看看SceneController类是怎么实现单实例的：

```
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
```
可以看到，当第一次调用SceneController.getInstance()时，会创建一个新的SceneController类对象，保存在instance中。当我们在其他类中调用SceneController.getInstance()时，就能得到同一个SceneController对象，即instance。通过instance，我们就可以轻易实现类与类之间的通信。

而要实现MVC，我们必须把模型和控制分在两个类写，因此我们声明一个GameModel类负责管理模型（英雄、敌人和场景）及其动作。接着让SceneController关联GameModel，即在SceneController中声明一个Model类，这样我们便能在SceneController中调用GameModel类的函数。

---

### 接口与门面模式
即用户的键盘输入与英雄的运动之间通过一个接口来实现，从UML图中我们可以看到该接口为UserAction，其代码如下：

```
public interface UserAction
    {
        //控制英雄移动
        void heroMove(int dir);
    }
```
接着来看看处理用户键盘输入的类UserInput：

```
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
```
可以看到在这个类中我们定义了当在键盘输入WASD时，我们接口UserAction会调用其方法heroMove()。而heroMove具体怎么实现，则是在继承UserAction接口的GameModel中定义：

```
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
```
至于使用接口与门面模式的好处，在于我们通过一套接口UserAction来定义用户输入与英雄运动的渠道，这样实现英雄运动的程序员只需要在GameModel类实现UserAction接口中的heroMove()函数，他的代码就可以被任何关联这个接口的用户输入类所使用。实现用户输入类的程序员也不需要知道heroMove()方法是怎么实现的，他只用在用户输入类中声明一个UserAction接口并在需要的时候调用接口中的heroMove()函数就行。

---

### 动作管理器模式
**注意，该游戏的第一个难点出现了！我们该如何实现敌人的循环走动呢？**

简单来说，我们需要在敌人的一个动作结束后，在动作结束回调函数中再添加动作。在“敌人交互”部分我们可以看到有个AddAction接口，其声明了两个方法：
1. addRandomMovement(当敌人为非追捕状态时，添加随机移动动作)
2. addDirectMovement（当敌人为追捕状态时，计算追捕方向，即英雄位置减去敌人位置，添加追捕动作）

这两个方法通过GameModel继承AddAction接口并在GameModel中实现
* addRandomMovement
```
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
```
* addDirecMovement

```
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
```

知道了所要添加的动作后，我们该如何添加呢？此时便引出管理敌人运动的动作管理器！

动作管理器的概念如下：
* 动作管理器就是一个对象，管理整个场景中所有的动作
* 一个SceneController（场景管理器）只配备一个动作管理器对象。
* 动作管理器可以添加动作（添加时要指定动作所作用的GameObject），监测已完成的动作并清除。

**为什么要使用动作管理器呢？**

因为如果为所有会动的物体都实现一个运动方法，势必是一种资源的浪费，如果把动作的共性提取出来，用一个管理器统一管理，这样代码的复用性和可读性都会提高。

首先我们要声明一个动作基类SSAction：

```
ublic class SSAction : ScriptableObject {

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
```
动作基类SSAction有以下特点：
* ScriptableObject是不需要绑定GameObject对象（不用挂载）的可编程基类，这些对象受Unity引擎场景管理
* 使用virtual申明虚方法，通过重写实现多态，这样继承者就明确使用Start和Update编程游戏对象行为
* 利用接口实现消息通知，避免与动作管理者直接依赖
* Update和Start不会自身调用，需要通过动作管理者来调用，因为他不是MonoBehaviour的子类

然后我们定义动作事件结束接口ISSActionCallback:

```
//动作处理接口，所有动作管理器都必须实现这个接口来实现事件调度，同时组合动作也需要实现它才能进行动作的切换
public enum SSActionEventType : int { Started, Competeted }//事件类型，保存动作是开始执行还是执行完毕
public enum SSActionTargetType : int { Normal, Catching}//动作目的地类型，保存动作是巡逻还是追捕

public interface ISSActionCallback {
    //动作结束时回调，需要告知是哪种动作
    void SSActionEvent(SSAction source, SSActionEventType eventType = SSActionEventType.Competeted,
        SSActionTargetType intParam = SSActionTargetType.Normal,
        string strParam = null, Object objParam = null);
}
```
动作事件结束接口有以下特点：
* 事件类型定义了两个枚举变量，一个用来保存动作是开始执行还是执行完毕，一个是用来保存动作是巡逻还是追捕
* 当动作结束时，调用事件ISSActionCallback接口的函数SSActionEvent，再根据传回的参数决定该添加哪一种动作（addRandomMovement还是DirectMovement）。SSActionEvent在GameModel中实现（GameModel实现ISSActionCallback接口）：

```
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
```

接着我们定义单个动作的实现CCMoveToAction：

```
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
```
该类主要是实现敌人单个动作的移动，当移动到目的地时，我们需要结束该动作并将动作结束的消息以及参数发送给动作管理者让其做接下来的判断，是应该追捕还是巡逻。

然后定义组合动作的实现CCSequenceActions：

```
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
```
由于这个游戏没有用到组合动作，因此我在这里不做详细介绍。

最后，定义动作管理者SSActionManger，用来接受场景控制命令和管理动作的自动执行：

```
//动作管理基类
public class SSActionManger : MonoBehaviour {

    private Dictionary<int, SSAction> actions = new Dictionary<int, SSAction>();
    private List<SSAction> waitingAdd = new List<SSAction>();//等待加入动作列表
    private List<int> waitingDelete = new List<int>();//等待删除动作的下标列表

	// Use this for initialization
	protected void Start () {

	}
	
	// 设置为protected类型，只有该类及其派生类能够调用Update()函数
	protected void Update () {
        //Debug.Log(waitingAdd.Count);

        //把等待加入动作列表中的动作加入
		foreach(SSAction ac in waitingAdd)
        {
            actions[ac.GetInstanceID()] = ac;
        }
        waitingAdd.Clear();

        foreach(KeyValuePair<int,SSAction> kv in actions)
        {
            SSAction ac = kv.Value;
            //如果该动作需要删除，则将其下标加入等待删除动作的下标列表
            if (ac.destory)
            {
                waitingDelete.Add(ac.GetInstanceID());
            }
            else if (ac.enable)
            {
                ac.Update();
                //Debug.Log("aaa");
            }
        }

        //删除等待删除的动作
        foreach(int key in waitingDelete)
        {
            SSAction ac = actions[key];
            actions.Remove(key);
            DestroyObject(ac);
        }
        waitingDelete.Clear();
	}

    //当对某一对象添加新的动作时
    public void addAction(GameObject gameObject,SSAction action,ISSActionCallback manager)
    {
        //Debug.Log(gameObject.name + " add Action!");
        //先把该对象现有的动作销毁
        for(int i = 0;i < waitingAdd.Count; i++)
        {
            if (waitingAdd[i].gameobject.Equals(gameObject))
            {
                SSAction ac = waitingAdd[i];
                waitingAdd.RemoveAt(i);
                i--;
                DestroyObject(ac);
            }
        }
        foreach(KeyValuePair<int,SSAction> kv in actions)
        {
            SSAction ac = kv.Value;
            if (ac.gameobject.Equals(gameObject))
            {
                ac.destory = true;
            }
        }

        //添加新的动作
        action.gameobject = gameObject;
        action.transform = gameObject.transform;
        action.callback = manager;
        waitingAdd.Add(action);
        action.Start();
    }
}
```
**注意！这里游戏的第二个难点出现了！怎么保证敌人在所有时刻都只有一个动作？**
我们只需要在为敌人添加动作时销毁其现有动作即可，因此SSActionManager中的添加动作函数addAction()才应该那么写。

---

### 游戏对象工厂模式
游戏对象工厂模式也叫对象池模式，其出现的原因为：
* 游戏对象的创建与销毁高成本，必须减少销毁次数。如：游戏中的子弹
* 屏蔽创建与销毁的业务逻辑，是程序易于扩展

我们通过游戏对象工厂模式来生产敌人，因此声明一个EnemyFactory类来实现：

```
public class EnemyFatory : System.Object
    {
        private static EnemyFatory instance;
        private List<GameObject> UsingEnemy = new List<GameObject>();//储存正在使用中的敌人
        private List<GameObject> FreeEnemy = new List<GameObject>();//储存空闲的敌人
        private GameObject enemyPrefab;
        private Vector3[] EnemyPosSet = new Vector3[] { new Vector3(-6, 0, 16), new Vector3(-1, 0, 19),
        new Vector3(6, 0, 16), new Vector3(-5, 0, 7), new Vector3(0, 0, 7), new Vector3(6, 0, 7) };//敌人位置

        public static EnemyFatory getInstance()
        {
            if (instance == null)
            {
                instance = new EnemyFatory();
            }
            return instance;
        }

        public void initItem(GameObject EnemyItem)
        {
            enemyPrefab = EnemyItem;
            enemyPrefab.SetActive(false);
        }
       
        //获得敌人函数
        public GameObject getEnemy()
        {
            GameObject temp;
            //如果空闲列表中没有敌人，则初始化一个
            if(FreeEnemy.Count == 0)
            {
                temp = Object.Instantiate(enemyPrefab) as GameObject;
                temp.SetActive(true);
            }
            //如果空闲列表中有敌人，则从空闲列表中取出一个
            else
            {
                temp = FreeEnemy[0];
                temp.SetActive(true);
                FreeEnemy.RemoveAt(0);
            }
            //将该敌人加入正在使用列表
            UsingEnemy.Add(temp);
            return temp;
        }

        //回收一个敌人
        public void recycle(GameObject enemy)
        {
            enemy.SetActive(false);
            UsingEnemy.Remove(enemy);
            FreeEnemy.Add(enemy);
        }

        //回收所有敌人
        public void recycleAll()
        {
            while (UsingEnemy.Count != 0)
            {
                recycle(UsingEnemy[0]);
            }
        }

        public Vector3[] getPosSet()
        {
            return EnemyPosSet;
        }
    }
```
这个类的特点如下：
* 最关键的点是维护UsingEnemy和FreeEnemy两个list
* 我们在销毁敌人的时候不调用系统的destroy函数，而是仅仅使用setActive(false)，下次需要敌人的时候让它出现在应该的位置就可以了，这样做的目的是避免了unity GC机制的开销。
* 职责分离，有利于代码的模块化、减少耦合。比如说不要在工厂中直接给产生的敌人添加动作（因为管理动作不是工厂的职责），而要将敌人传递给GameModel以后，让GameModel去调用动作管理器来添加。这样就将工厂和动作管理器之间的耦合降低了。将来你想要给敌人添加更多种运动方式的时候，只需要更改动作管理器类就可以了，完全不用管工厂类。否则，你会发现敌人一旦生成就会按照旧方式来运动，这样你就要修改更多的代码（既要改动工厂类、又要改动动作管理器类）。

---

### 观察者模式（发布与订阅模式）
观察者模式的概念如下：
* 发布者（事件源）/Publisher：事件或消息的拥有者
* 主题（渠道）/Subject：消息发布媒体
* 接收器（地址）/Handle：处理消息的方法
* 订阅者（接受者）/Subscriber：对主题感兴趣的人

我们使用订阅者模式来传递玩家得分和游戏结束的信息，具体做法是通过GameEventManager类发布上述信息，GameStatusText订阅信息。当玩家得分或者游戏结束时，都会调用GameEventManager的得分/游戏结束方法（具体实现可能后期会更改）。接着我们通过挂载在UI.text上的GamesStatusText订阅两种信息即可，这样就可以在触发玩家得分、游戏结束时自己改变文字内容。

具体代码实现如下：

GameEventManager：

```
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
```

GameStatusText:

```
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

```

---

到这里，整个游戏的设计模式实现我已经基本讲完了，但是这个游戏还有**最后两个难点**：

（1）敌人如何感知玩家并进行追踪？如何感知到玩家逃离并继续巡逻？

给玩家Hero设置一个区域变量信息，每个敌人时刻检测该信息，来判断是否在自己区域，从而有对应的行动。
具体做法时在Hero上挂载一个脚本HeroStatus.cs，该脚本中有个standOnArea的整形变量，时刻根据玩家位置改变该变量的值。敌人在Update()方法里时刻检测该值来判断玩家是否进入自己区域，若是，则添加抓捕动作addDirectMovement，而原动作也自动销毁。同时在敌人上挂载一个脚本EnemyBehaviour.cs，该脚本中有个isCatching的变量，表示敌人是否处于追捕状态，若处于追捕状态，而此时玩家不在自己的区域，说明在刚一瞬间，玩家逃离了自己区域，此时会添加随机动作addRandomMovement，继续巡逻。具体逻辑看代码实现，这里就不PO出来了。

（2）当敌人在移动过程中碰到障碍物（如墙、栅栏、其他敌人）时怎么办。在这里我通过在挂载在敌人上的标本EnmeyBehaviour中的OnCollisionStay函数实现（不使用OnCollisionEnter只能触发进入碰撞的那一瞬间）。如果发生碰撞，则添加随机动作：

```
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
```

## 结语
在做这个游戏前我把以前的PPT看了很久但还是对设计模式不太理解，当是在我打代码的时候我才发现只有亲自去实践才能理解这些设计模式的精髓，看来还是需要多打代码才能成为一个优秀的游戏开发者！最后放上这个游戏的源代码，有需要自取（别忘了点颗星哦！）：

[Escape源码](https://github.com/Richbabe/Escape)
















