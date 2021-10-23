﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundController : MonoBehaviour, ISceneController, IUserAction
{
    public IActionManager action_manager;
    public DiskFactory disk_factory;
    public UserGUI user_gui;
    public ScoreRecorder score_recorder;

    private Queue<GameObject> disk_queue = new Queue<GameObject>();          //游戏场景中的飞碟队列
    private List<GameObject> disk_notshot = new List<GameObject>();          //没有被打中的飞碟队列
    private int round = 1;                                                   //回合
    private int trial = 1;
    private float interval = 2.1f;                                                //发射一个飞碟的时间间隔
    private bool playing_game = false;                                       //游戏中
    private bool game_over = false;                                          //游戏结束
    private bool game_start = false;                                         //游戏开始       
    public bool isPhy = true;                                               //是否使用物理运动

    void Start ()
    {
        SSDirector director = SSDirector.GetInstance();     
        director.CurrentScenceController = this;             
        disk_factory = Singleton<DiskFactory>.Instance;
        score_recorder = Singleton<ScoreRecorder>.Instance;
        action_manager = gameObject.AddComponent<ActionManagerAdapter>() as IActionManager;
        user_gui = gameObject.AddComponent<UserGUI>() as UserGUI;
        transform.position = transform.rotation * (new Vector3(0, 3, -10));//设置Main Camera位置
    }
	
	void Update ()
    {
        if(game_start)
        {
            //游戏结束，取消定时发送飞碟
            if (game_over)
            {
                CancelInvoke("LoadResources");//取消调用LoadResources
            }
            //设定一个定时器，发送飞碟，游戏开始
            if (!playing_game)
            {
                InvokeRepeating("LoadResources", 1f, interval);//1秒后调用LoadResources，每speed秒调用一次
                playing_game = true;
            }
            //发送飞碟
            SendDisk();
        }
    }

    public void UpdateTrial(){
        if (trial==10)
        {
            //回合升级
            round++;
            trial=1;
            //缩小飞碟发送间隔
            interval = round>20 ? interval : interval -  0.1f;
            CancelInvoke("LoadResources");
            playing_game = false;
        }
        else trial++;   
    }
    public void LoadResources()
    {
        disk_queue.Enqueue(disk_factory.GetDisk(round)); 
    }

    private void SendDisk()
    {
        float position_x = 9;                       
        if (disk_queue.Count != 0)
        {
            GameObject disk = disk_queue.Dequeue();
            disk_notshot.Add(disk);
            disk.SetActive(true);
            //设置被隐藏了或是新建的飞碟的位置
            float ran_y = Random.Range(3f, 6f);
            float ran_x = Random.Range(-1f, 1f) < 0 ? -1 : 1;
            disk.GetComponent<DiskData>().direction = new Vector3(ran_x, ran_y, 0);
            Vector3 position = new Vector3(-disk.GetComponent<DiskData>().direction.x * position_x, ran_y, 0);
            disk.transform.position = position;
            //设置飞碟初始所受的力和角度，飞碟速度总体随round增大而加快。
            float power = Random.Range(0.9f + 0.1f * round, 1.35f + 0.15f * round);
            float angle = Random.Range(15f, 28f);
            action_manager.playDisk(disk, angle, power,round,isPhy);
        }

        for (int i = 0; i < disk_notshot.Count; i++)
        {
            GameObject temp = disk_notshot[i];
            //飞碟飞出摄像机视野也没被打中
            if (temp.transform.position.y < -3 && temp.gameObject.activeSelf == true)
            {
                if(user_gui.life>1)UpdateTrial();
                disk_factory.FreeDisk(disk_notshot[i]);
                disk_notshot.Remove(disk_notshot[i]);
                //玩家血量-1
                user_gui.ReduceBlood();
            }
        }
    }

    public void Hit(Vector3 pos)
    {
        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray);
        bool not_hit = false;
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            //射线打中物体
            if (hit.collider.gameObject.GetComponent<DiskData>() != null)
            {
                //射中的物体要在没有打中的飞碟列表中
                for (int j = 0; j < disk_notshot.Count; j++)
                {
                    if (hit.collider.gameObject.GetInstanceID() == disk_notshot[j].gameObject.GetInstanceID())
                    {
                        not_hit = true;
                    }
                }
                if(!not_hit)
                {
                    return;
                }
                UpdateTrial();
                disk_notshot.Remove(hit.collider.gameObject);
                //记分员记录分数
                score_recorder.Record(hit.collider.gameObject);
                //显示爆炸粒子效果
                Transform explode = hit.collider.gameObject.transform.GetChild(0);
                explode.GetComponent<ParticleSystem>().Play();
                //等0.1秒后执行回收飞碟
                StartCoroutine(WaitingParticle(0.08f, hit, disk_factory, hit.collider.gameObject));
                break;
            }
        }
    }
    //获得分数
    public int GetScore()
    {
        return score_recorder.score;
    }
    //获得round
    public int GetRound()
    {
        return round;
    }
    //获得分数
    public int GetTrail()
    {
        return trial;
    }
    //重新开始
    public void ReStart()
    {
        game_over = false;
        playing_game = false;
        score_recorder.score = 0;
        round = 1;
        interval = 2f;
        trial = 1;
    }
    //设定游戏结束
    public void GameOver()
    {
        game_over = true;
    }
    //暂停几秒后回收飞碟
    IEnumerator WaitingParticle(float wait_time, RaycastHit hit, DiskFactory disk_factory, GameObject obj)
    {
        yield return new WaitForSeconds(wait_time);
        //等待之后执行的动作  
        hit.collider.gameObject.transform.position = new Vector3(0, -9, 0);
        disk_factory.FreeDisk(obj);
    }
    public void BeginGame()
    {
        game_start = true;
    }
}
