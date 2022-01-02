using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class IMGUIbloodBar : MonoBehaviour {
    public float curBlood = 5f;
    private float targetBlood = 5f;
    private Rect bloodBarArea;
    private Rect addButton;
    private Rect subButton;
    private int status = 0;//1:add -1:sub 0:不变
 
    void Start () {
        bloodBarArea = new Rect(Screen.width/2 - 80, Screen.height/2, 200, 50);
        addButton = new Rect(Screen.width/2 - 80,Screen.height/2 + 30, 40, 20);
        subButton = new Rect(Screen.width/2 + 80,Screen.height/2 + 30, 40, 20);
	}
 
    public void addBlood() {
        targetBlood = targetBlood + 1 > 10f? 10f : targetBlood + 1;
    }
 
    public void subBlood() {
        targetBlood = targetBlood - 1 < 0f? 0f : targetBlood - 1;
    }
 
    private void OnGUI() {
        if (GUI.Button(addButton, " + ")) status = 1;
        if (GUI.Button(subButton, " - ")) status = -1;
        if (status==1) {
            addBlood();
            status = 0;
        }
        else if (status==-1){
            subBlood();
            status = 0;
        }
        curBlood = Mathf.Lerp(curBlood, targetBlood, 0.1f);
        GUI.HorizontalScrollbar(bloodBarArea, 0f, curBlood, 0f, 10f);
    }
}