using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicTakToe : MonoBehaviour
{
    private int turn = 1;    //1 - Player 1, 0 - Player 2 
    private int[,] board = new int[3,3];   //标记玩家落子
    private int result = 0;  //0代表未完成，1 - Player 1 wins，2 - Player 2 wins，3-平局
    //背景
    public Texture2D background;  
    //字体
    private GUIStyle fontStyle;
    private GUIStyle fontStyle2;
    private GUIStyle fontStyle3;
    private GUIStyle fontStyle4;  

    //游戏基础界面初始化
    void Init(){
        //标题字体设置
        fontStyle = new GUIStyle();  
        fontStyle.normal.background = null;   
        fontStyle.normal.textColor= new Color(1, 0, 0);    
        fontStyle.fontSize = 30;   

        //游戏状态字体设置
        fontStyle2 = new GUIStyle();  
        fontStyle2.normal.background = null;   
        fontStyle2.normal.textColor= new Color(1, 0, 1);    
        fontStyle2.fontSize = 20;   

        //player&turn字体设置
        fontStyle3 = new GUIStyle();  
        fontStyle3.normal.background = null;   
        fontStyle3.normal.textColor= new Color(0, 0, 1);    
        fontStyle3.fontSize = 20;   

        //dot字体设置
        fontStyle4 = new GUIStyle();  
        fontStyle4.normal.background = null;   
        fontStyle4.normal.textColor= new Color(0, 0, 1);    
        fontStyle4.fontSize = 100;   

        //背景图片设置
        GUI.Label (new Rect (0, 0, 1000,500), background);
        //显示标题、玩家轮次提示
        GUI.Label (new Rect (180, 15, 200, 100), "Let's  play  TicTacToe  !",fontStyle);  
        GUI.Label (new Rect (560, 200, 100, 50), "Player1",fontStyle3); 
        GUI.Label (new Rect (560, 270, 100, 50), "Player2",fontStyle3);
        GUI.Label (new Rect (510, 135, 100, 50), "Turns:", fontStyle3);  
    }

    //重置棋盘
    void Reset() { 
        result = 0;
        turn = 1;    
        for (int i=0; i<3; ++i) {    
            for (int j=0; j<3; ++j) {    
                board[i,j] = 0;    
            }    
        }  
    }  

    //初始化
    void Start () {  
        Init();
        Reset ();  
    }  

    void OnGUI() {   
        //游戏基础界面初始化
        Init();
        //显示轮次
        ShowTurn();
        //设置重置游戏按钮
        if (GUI.Button (new Rect (50, 180, 100, 50), "RESET"))  
            Reset ();  
        //显示棋盘状态
        ShowBoard();
        //显示游戏状态
        ShowResult();
        //AI plays 1 step
        if (GUI.Button (new Rect (50, 250, 100, 50), "AI plays 1 step") && result == 0) {
            AiPlay();
        }
    }  

    //显示轮次
    void ShowTurn(){
        if (turn == 1) {
            GUI.Label (new Rect (510, 155, 50, 50), "·", fontStyle4);
        } else {
            GUI.Label (new Rect (510, 225, 50, 50), "·", fontStyle4);
        }
    }

    //若游戏未结束，当玩家点击棋盘上未落子位置时添加标记，记录下棋者
    void mark(int i,int j){
        if (result == 0 && board [i, j] == 0) {    
            if (turn == 1)    
                board [i, j] = 1;  
            else    
                board [i, j] = 2;    
            turn = 1-turn;    
        }  
    }

    //根据标记显示棋子，当玩家点击棋盘上未落子位置时添加标记
    void ShowBoard(){
        for (int i=0; i<3; ++i) {    
            for (int j=0; j<3; ++j) {    
                if (board [i, j] == 1)    
                    GUI.Button (new Rect (185 + i * 100, 120 + j * 100, 100, 100), "X");  
                if (board [i, j] == 2)  
                    GUI.Button (new Rect (185 + i * 100, 120 + j * 100, 100, 100), "O");  
                if (GUI.Button (new Rect (185 + i * 100, 120 + j * 100, 100, 100), "")) {    
                    mark(i,j);  
                }    
            }  
        }
    }

    //显示游戏状态
    void ShowResult(){
        GUI.Label (new Rect (265, 70, 100, 50), "Result:",fontStyle2);
        result = check ();    
        if (result == 1) {    
            GUI.Label (new Rect (330, 70, 100, 50), "Player1 wins!", fontStyle2);    
        } else if (result == 2) {    
            GUI.Label (new Rect (330, 70, 100, 50), "Player2 wins!", fontStyle2);    
        } else if (result == 3) {
            GUI.Label (new Rect (330, 70, 100, 50), "No one wins", fontStyle2); 
        } else {
            GUI.Label (new Rect (330, 70, 100, 50), "Playing...", fontStyle2);
        } 
    }

    //随机下一步棋
    void AiPlay(){
        //随机选取一个棋盘上的位置
        int i = (int)Random.Range (0, 2);
        int j = (int)Random.Range (0, 2);
        int cnt = 0;
        //从该位置开始从左往右、从上往下依次扫描棋盘上的9个位置，直到找到一个空位置或全部位置扫描完为止
        while (board [i, j] != 0) {
            i++;
            j=(j + i / 3)%3;
            i%=3;
            cnt++;
            if (cnt == 9)
                break;
        }
        //检测是否找到空位置
        int flag = 0;
        if (board [i, j] == 0)
            flag = 1;
        //若找到空位则标记下棋者
        if (turn == 1 && flag == 1)    
            board [i, j] = 1;  
        else if(turn == 0 && flag == 1)   
            board [i, j] = 2;    
        //交换轮次
        if (flag == 1)
            turn = 1-turn;    
    }

    //Check the result
    int check() {    
        //检查是否存在一行下满三颗同种棋
        for (int i=0; i<3; ++i) {    
            if (board[i,0]!=0 && board[i,0]==board[i,1] && board[i,1]==board[i,2]) {    
                return board[i,0];    
            }    
        }    
        //检查是否存在一列下满三颗同种棋
        for (int j=0; j<3; ++j) {    
            if (board[0,j]!=0 && board[0,j]==board[1,j] && board[1,j]==board[2,j]) {    
                return board[0,j];    
            }    
        }    
        //检查是否存在一条对角线下满三颗同种棋
        if (board[1,1]!=0 && board[0,0]==board[1,1] && board[1,1]==board[2,2] || board[0,2]==board[1,1] && board[1,1]==board[2,0]) {    
            return board[1,1];    
        }    
        //检查棋盘是否下满
        int count = 0;
        for (int i = 0; i < 3; i++) {
            for (int j = 0; j < 3; j++) {
                if (board [i, j] != 0)
                    count++;
            }
        }
        //若棋盘下满且无人获胜则平局
        if (count == 9)
            return 3;
        //若棋盘未下满且无人获胜则游戏继续
        return 0;    
    }    
}
