using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyActionManager : SSActionManager
{

    public UFOFlyAction fly;                            //飞碟飞行的动作
    public RoundController scene_controller;             //当前场景的场景控制器

    protected void Start()
    {
        scene_controller = (RoundController)SSDirector.GetInstance().CurrentScenceController;
        scene_controller.action_manager = this;     
    }
    //飞碟飞行
    public void UFOFly(GameObject disk, float angle, float power, int round)
    {
        fly = UFOFlyAction.GetSSAction(disk.GetComponent<DiskData>().direction, angle, power);
        fly.gravity = -0.04f - 0.01f * round;
        this.RunAction(disk, fly, this);
    }
}
