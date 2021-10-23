﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManagerAdapter : MonoBehaviour,IActionManager
{
    public FlyActionManager action_manager;
    public PhysisFlyActionManager phy_action_manager;
    public void playDisk(GameObject disk, float angle, float power,int round,bool isPhy)
    {
        if(isPhy)
        {
            phy_action_manager.UFOFly(disk, angle, power,round);
        }
        else
        {
            action_manager.UFOFly(disk, angle, power,round);
        }
    }
    // Use this for initialization
    void Start ()
    {
        action_manager = gameObject.AddComponent<FlyActionManager>() as FlyActionManager;
        phy_action_manager = gameObject.AddComponent<PhysisFlyActionManager>() as PhysisFlyActionManager;
    }

}
