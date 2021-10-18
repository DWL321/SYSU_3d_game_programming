using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mygame;
public class UserGUI : MonoBehaviour {
 
    private IUserAction action;
    public int state = 0;//游戏状态：0-正在进行中；1-获胜；2-失败
    bool ShowRule = false;//显示游戏规则
    private GUIStyle title_style;
    private GUIStyle result_style;
    private GUIStyle rule_style;
    private GUIStyle button_style;

    void init(){
        title_style = new GUIStyle();
        title_style.normal.textColor = Color.red;
        title_style.fontSize = 45;
        result_style = new GUIStyle();
        result_style.normal.textColor = Color.red;
        result_style.fontSize = 30;
        rule_style = new GUIStyle();
        rule_style.fontSize=20;
        button_style= new GUIStyle("button");
        button_style.fontSize=15;
    }
    void Start()
    {
        action = SSDirector.GetInstance().CurrentScenceController as IUserAction;
        init();
    }
    void rule(){
        if (ShowRule)
            ShowRule = false;
        else
            ShowRule = true;
    }
    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2 - 130, 10, 200, 50), "牧师与魔鬼", title_style);
        if (GUI.Button(new Rect(10, 10, 60, 30), "Rule", button_style)) rule();
        if(ShowRule)
        {
        GUI.Label(new Rect(Screen.width / 2 - 260, 70, 200, 20), "点击牧师(白)或魔鬼(黑)可控制角色上船或上岸，点击船开船", rule_style);
        GUI.Label(new Rect(Screen.width / 2 - 160, 95, 300, 20), "船至多载2人，至少由1人驾驶。", rule_style);
        GUI.Label(new Rect(Screen.width / 2 - 250, 120, 240, 20), "让全部牧师和魔鬼渡河，每一边魔鬼数量都不能多于牧师", rule_style);
        }
        if (state == 1)
        {
            GUI.Label(new Rect(Screen.width / 2-110, Screen.height / 2-70, 100, 50), "GAME OVER!", result_style);
            if (GUI.Button(new Rect(Screen.width / 2 - 70, Screen.height / 2, 100, 50), "Restart", button_style))
            {
                action.Restart();
                state = 0;
            }
        }
        else if (state == 2)
        {
            GUI.Label(new Rect(Screen.width / 2 - 80, Screen.height / 2 - 70, 100, 50), "YOU WIN!", result_style);
            if (GUI.Button(new Rect(Screen.width / 2 - 70, Screen.height / 2, 100, 50), "Restart", button_style))
            {
                action.Restart();
                state = 0;
            }
        }
    }
}