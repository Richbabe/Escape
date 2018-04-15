using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.Escape;

namespace Com.Escape
{
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
}


