using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Escape;

//挂载在英雄上的脚本
public class HeroStatus : MonoBehaviour {

    public int standOnArea = -1;//英雄所在区域

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        checkHeroArea();
	}

    //检测英雄所在区域
    void checkHeroArea()
    {
        float posX = this.gameObject.transform.position.x;//英雄的x坐标
        float posZ = this.gameObject.transform.position.z;//英雄的z坐标
        //在上半部分
        if(posZ >= FenchLocation.FenchHori)
        {
            //第一个区域
            if(posX < FenchLocation.FenchVertLeft)
            {
                standOnArea = 0;
            }
            //第三个区域
            else if(posX > FenchLocation.FenchVertRight)
            {
                standOnArea = 2;
            }
            //第二个区域
            else
            {
                standOnArea = 1;
            }
        }
        //下半部分
        else
        {
            //第四个区域
            if (posX < FenchLocation.FenchVertLeft)
            {
                standOnArea = 3;
            }
            //第六个区域
            else if (posX > FenchLocation.FenchVertRight)
            {
                standOnArea = 5;
            }
            //第五个区域
            else
            {
                standOnArea = 4;
            }
        }
        //Debug.Log(standOnArea);
    }
}
