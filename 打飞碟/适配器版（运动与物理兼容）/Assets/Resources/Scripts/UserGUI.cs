using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserGUI : MonoBehaviour
{
    private IUserAction action;
    //每个GUI的style
    GUIStyle bold_style = new GUIStyle();
    GUIStyle score_style = new GUIStyle();
    GUIStyle text_style = new GUIStyle();
    GUIStyle over_style = new GUIStyle();
    public  int life = 10;                   //血量
    private int high_record = 1;      //最高纪录round
    private int high_score = 0;       //最高得分
    private int score = 0;            //本round得分
    private bool game_start = false;       //游戏开始
    private int round = 1;                 
    private int trial = 1;

    void init(){
        bold_style.normal.textColor = new Color(1, 0, 0);
        bold_style.fontSize = 16;
        text_style.normal.textColor = new Color(0,0,0, 1);
        text_style.fontSize = 16;
        score_style.normal.textColor = new Color(1,0,1,1);
        score_style.fontSize = 16;
        over_style.normal.textColor = new Color(1, 0, 0);
        over_style.fontSize = 25;
    }
    void Start ()
    {
        action = SSDirector.GetInstance().CurrentScenceController as IUserAction;
        init();
    }
	void UpdateData(){
        score=action.GetScore();
        round=action.GetRound();
        trial=action.GetTrail();
        high_record = high_record > round ? high_record : round;
        high_score = high_score > score ? high_score : score;
    }
	void OnGUI ()
    {
        if (game_start)
        {
            //用户射击
            if (Input.GetButtonDown("Fire1"))
            {
                Vector3 pos = Input.mousePosition;
                if(life > 0)action.Hit(pos);
            }
            UpdateData();            
            GUI.Label(new Rect(10, 5, 200, 50), "分数:", text_style);
            GUI.Label(new Rect(55, 5, 200, 50), score.ToString(), score_style);
            GUI.Label(new Rect(Screen.width - 380, 5, 200, 50), "round:", text_style);
            GUI.Label(new Rect(Screen.width - 330, 5, 200, 50), round.ToString(), score_style);
            GUI.Label(new Rect(Screen.width - 265, 5, 200, 50), "trial:", text_style);
            GUI.Label(new Rect(Screen.width - 225, 5, 200, 50), trial.ToString(), score_style);
            GUI.Label(new Rect(Screen.width - 160, 5, 50, 50), "生命:", text_style);
            //显示当前血量
            for (int i = 0; i < life; i++)
            {
                GUI.Label(new Rect(Screen.width - 115 + 10 * i, 5, 50, 50), "X", bold_style);
            }
            //游戏结束
            if (life == 0)
            {
                GUI.Label(new Rect(Screen.width / 2 - 70, Screen.height / 2 - 75, 100, 100), "游戏结束", over_style);
                GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 25, 150, 50), "最高纪录: 进入round", text_style);
                GUI.Label(new Rect(Screen.width / 2 + 50, Screen.height / 2 - 25, 50, 50), high_record.ToString(), score_style);
                GUI.Label(new Rect(Screen.width / 2 - 65, Screen.height / 2 - 5, 150, 50), "最高得分: ", text_style);
                GUI.Label(new Rect(Screen.width / 2 + 15, Screen.height / 2 - 5, 50, 50), high_score.ToString(), score_style);
                if (GUI.Button(new Rect(Screen.width / 2 - 60, Screen.height / 2 +25 , 100, 50), "重新开始"))
                {
                    life = 10;
                    action.ReStart();
                    return;
                }
                action.GameOver();
            }
        }
        else
        {
            GUI.Label(new Rect(Screen.width / 2 - 60, Screen.height / 2 - 160, 100, 100), "Hit UFO!", over_style);
            GUI.Label(new Rect(Screen.width / 2 - 230, Screen.height / 2 - 90, 400, 100), "游戏规则：", text_style);
            GUI.Label(new Rect(Screen.width / 2 - 230, Screen.height / 2 - 70, 400, 100), "1.游戏有无限多个round，每个round包括10次trial", text_style);
            GUI.Label(new Rect(Screen.width / 2 - 230, Screen.height / 2 - 50, 400, 100), "2.点击飞碟得分，红色飞碟3分，绿色飞碟2分，紫色飞碟1分", text_style);
            GUI.Label(new Rect(Screen.width / 2 - 230, Screen.height / 2 - 30, 400, 100), "3.每有一个飞碟没击中则生命-1", text_style);
            GUI.Label(new Rect(Screen.width / 2 - 230, Screen.height / 2 - 10, 400, 100), "4.round结束后生命>0则进入下一个round，否则从round 1重新开始", text_style);
            if (GUI.Button(new Rect(Screen.width / 2 - 70, Screen.height / 2 + 70, 100, 50), "游戏开始"))
            {
                game_start = true;
                action.BeginGame();
            }
        }
    }
    public void ReduceBlood()
    {
        if(life > 0)
            life--;
    }
}
