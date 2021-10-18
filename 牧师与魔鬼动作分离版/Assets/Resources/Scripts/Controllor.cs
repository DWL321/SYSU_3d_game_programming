using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mygame;
using UmpireSpace;

public class Controllor : MonoBehaviour, ISceneController, IUserAction
{  
    public LandModel from_land;            //左侧起点陆地
    public LandModel to_land;              //右侧终点陆地
    public BoatModel boat;                  //船
    private RoleModel[] roles;              //角色
    UserGUI user_gui;                       //GUI界面
    Umpire umpire;                          //裁判
    public CCActionManager actionManager;          //动作管理器

    void Start ()
    {
        SSDirector director = SSDirector.GetInstance();
        director.CurrentScenceController = this;
        user_gui = gameObject.AddComponent<UserGUI>() as UserGUI;
        transform.position = transform.rotation * (new Vector3(0, 1, -14));//设置Main Camera位置
        LoadResources();
        umpire=new Umpire(from_land,to_land,boat);
        actionManager = gameObject.AddComponent<CCActionManager>() as CCActionManager;  
    }
	//创建水、陆地、牧师、魔鬼、船
    public void LoadResources()              
    {
        GameObject water = Instantiate(Resources.Load("Prefabs/Water", typeof(GameObject)), new Vector3(0, -1, 0), Quaternion.identity) as GameObject;
        water.name = "water";       
        from_land = new LandModel("from");
        to_land = new LandModel("to");
        boat = new BoatModel();
        roles = new RoleModel[6];

        //初始时角色全置于左侧起点陆地
        for (int i = 0; i < 6; i++)
        {
            RoleModel role;
            if(i<3){
                role = new RoleModel("priest");
                role.SetName("priest" + i);
            }
            else{
                role = new RoleModel("devil");
                role.SetName("devil" + (i-3));
            }
            role.SetPosition(from_land.GetEmptyPosition());
            role.GoLand(from_land);
            from_land.AddRole(role);
            roles[i] = role;
        }
    }

    //在船上有人且游戏正在进行中的状态下移动船，并检查游戏是否结束
    public void MoveBoat()                  
    {
        if (boat.IsEmpty() || user_gui.state != 0) return;
        //通知动作管理器执行开船动作
        actionManager.moveBoat(boat.getGameObject(),boat.BoatMoveToPosition(),boat.move_speed);

        user_gui.state = Check();
    }

    //移动角色，并检查游戏是否结束
    public void MoveRole(RoleModel role)    
    {
        if (user_gui.state != 0) return;
        if (role.IsOnBoat())//如果在船上就上岸
        {
            LandModel land;
            if (boat.GetBoatState() == 0)
                land = to_land;
            else
                land = from_land;
            boat.DeleteRole(role.GetName());
            //通知动作管理器执行角色上岸动作，先竖直移动，后水平移动
            Vector3 end_pos = land.GetEmptyPosition();                                       
            Vector3 middle_pos = new Vector3(role.getGameObject().transform.position.x, end_pos.y, end_pos.z);
            actionManager.moveRole(role.getGameObject(), middle_pos, end_pos, role.move_speed); 

            role.GoLand(land);
            land.AddRole(role);
        }
        else//如果不在船上就上船
        {                                
            LandModel land = role.GetLandModel();
            if (boat.GetEmptyIndex() == -1 || land.GetLandType() != boat.GetBoatState()) return;   //船没有空位或此角色不在船停靠的那侧陆地，就不上船

            land.DeleteRole(role.GetName());
            //通知动作管理器执行角色上船动作，先水平移动，后竖直移动
            Vector3 end_pos = boat.GetEmptyPosition();                                             
            Vector3 middle_pos = new Vector3(end_pos.x, role.getGameObject().transform.position.y, end_pos.z); 
            actionManager.moveRole(role.getGameObject(), middle_pos, end_pos, role.move_speed);  

            role.GoBoat(boat);
            boat.AddRole(role);
        }
        user_gui.state = Check();
    }

    public void Restart()
    {
        from_land.Reset();
        to_land.Reset();
        boat.Reset();
        for (int i = 0; i < 6; i++)
        {
            roles[i].Reset();
        }
    }
    public int Check()
    {
        return umpire.Check();
    }
}