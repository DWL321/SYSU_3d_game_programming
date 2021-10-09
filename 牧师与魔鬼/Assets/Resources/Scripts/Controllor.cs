using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mygame;
public class Controllor : MonoBehaviour, ISceneController, IUserAction
{
    public LandModel from_land;            //左侧起点陆地
    public LandModel to_land;              //右侧终点陆地
    public BoatModel boat;                  //船
    private RoleModel[] roles;              //角色
    UserGUI user_gui;                       //GUI界面

    void Start ()
    {
        SSDirector director = SSDirector.GetInstance();
        director.CurrentScenceController = this;
        user_gui = gameObject.AddComponent<UserGUI>() as UserGUI;
        transform.position = transform.rotation * (new Vector3(0, 1, -14));//设置Main Camera位置
        LoadResources();
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
        boat.BoatMove();
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
            role.GoLand(land);
            land.AddRole(role);
        }
        else//如果不在船上就上船
        {                                
            LandModel land = role.GetLandModel();
            if (boat.GetEmptyIndex() == -1 || land.GetLandType() != boat.GetBoatState()) return;   //船没有空位或此角色不在船停靠的那侧陆地，就不上船

            land.DeleteRole(role.GetName());
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
        int from_priests = (from_land.GetRoleType())[0];
        int from_devils = (from_land.GetRoleType())[1];
        int to_priests = (to_land.GetRoleType())[0];
        int to_devils = (to_land.GetRoleType())[1];
        //全部角色均到达终点则获胜
        if (to_priests + to_devils == 6)
            return 2;
        //一侧魔鬼多于牧师则失败
        int[] boat_role_type = boat.GetRoleType();
        if (boat.GetBoatState() == 1)         //船在起点岸边
        {
            from_priests += boat_role_type[0];
            from_devils += boat_role_type[1];
        }
        else                             //船在终点岸边
        {
            to_priests += boat_role_type[0];
            to_devils += boat_role_type[1];
        }
        if ((from_priests > 0 && from_priests < from_devils)||(to_priests > 0 && to_priests < to_devils)) //失败
        {      
            return 1;
        }
        //游戏继续
        return 0;                                             
    }
}