using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface AddAction {
    void addRandomMovement(GameObject sourceObj, bool isActive);//添加敌人随机动作
    void addDirectMovement(GameObject sourceObj);//添加敌人追捕英雄动作
}
