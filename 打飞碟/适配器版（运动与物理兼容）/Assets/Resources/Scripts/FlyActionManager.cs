using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyActionManager : SSActionManager
{

    public UFOFlyAction fly;                            //飞碟飞行的动作

    protected void Start()
    {
    }
    //飞碟飞行
    public void UFOFly(GameObject disk, float angle, float power, int round)
    {
        fly = UFOFlyAction.GetSSAction(disk.GetComponent<DiskData>().direction, angle, power);
        fly.gravity = -0.04f - 0.01f * round;
        this.RunAction(disk, fly, this);
    }
}
