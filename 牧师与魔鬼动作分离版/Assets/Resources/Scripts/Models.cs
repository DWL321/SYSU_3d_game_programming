using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mygame
{ 

    //加载场景
    public interface ISceneController                      
    {
        void LoadResources();
    }
    //用户交互事件
    public interface IUserAction                           
    {
        void MoveBoat();                                   //移动船
        void Restart();                                    //重新开始
        void MoveRole(RoleModel role);                     //移动人物
        int Check();                                       //检测游戏状态
    }

    public class SSDirector : System.Object
    {
        private static SSDirector _instance;
        public ISceneController CurrentScenceController { get; set; }
        public static SSDirector GetInstance()
        {
            //单例模式
            if (_instance == null)
            {
                _instance = new SSDirector();
            }
            return _instance;
        }
    }

    public class BoatModel
    {
        GameObject boat;                                          
        Vector3[] from_empty_pos;                                    //船在起点陆地的空位位置
        Vector3[] to_empty_pos;                                      //船在终点陆地的空位位置
        Click click;
        int boat_state = 1;                                          //0：船在终点；1：船在起点
        RoleModel[] roles = new RoleModel[2];                        //船上的角色
        public float move_speed = 250; 

        public BoatModel()
        {
            boat = Object.Instantiate(Resources.Load("Prefabs/Boat", typeof(GameObject)), new Vector3(-5.5F, 0, 0), Quaternion.identity) as GameObject;
            boat.name = "boat";
            click = boat.AddComponent(typeof(Click)) as Click;
            click.SetBoat(this);
            from_empty_pos = new Vector3[] { new Vector3(-6, 1.5F, 0), new Vector3(-5, 1.5F, 0) };
            to_empty_pos = new Vector3[] { new Vector3(5, 1.5F, 0), new Vector3(6, 1.5F, 0) };
        }

        public GameObject getGameObject() { return boat; }

        //检测船是否为空
        public bool IsEmpty()
        {
            if (roles[0] != null||roles[1] != null)return false;
            else return true;
        }

        public Vector3 BoatMoveToPosition()
        {
            if (boat_state == 0)
            {
                boat_state = 1;
                return new Vector3(-5.5F, 0, 0);
            }
            else
            {
                boat_state = 0;
                return new Vector3(5.5F, 0, 0);
            }
        }

        public int GetBoatState(){ return boat_state;}

        public RoleModel DeleteRole(string role_name)
        {
            int i=-1;
            if (roles[0] != null && roles[0].GetName() == role_name)i=0;
            else if (roles[1] != null && roles[1].GetName() == role_name)i=1;
            if(i == 0||i == 1){
                RoleModel role = roles[i];
                roles[i] = null;
                return role;
            }
            return null;
        }

        public int GetEmptyIndex()
        {
            if(roles[0] == null)return 0;
            else if(roles[1] == null)return 1;
            else return -1;
        }

        //获得船上空位的坐标
        public Vector3 GetEmptyPosition()
        {
            Vector3 pos;
            if (boat_state == 0)
                pos = to_empty_pos[GetEmptyIndex()];
            else
                pos = from_empty_pos[GetEmptyIndex()];
            return pos;
        }

        public void AddRole(RoleModel role)
        {
            roles[GetEmptyIndex()] = role;
        }

        public GameObject GetBoat(){ return boat; }

        public void Reset()
        {
            if (boat_state == 0)
                boat.transform.position =BoatMoveToPosition();
            roles = new RoleModel[2];
        }

        //计算船上牧师和魔鬼的数量
        public int[] GetRoleType()
        {
            int[] count = { 0, 0 };
            if(roles[0]!=null&&roles[0].GetType() == 1)count[1]++;
            else if(roles[0]!=null&&roles[0].GetType() == 0)count[0]++;
            if(roles[1]!=null&&roles[1].GetType() == 1)count[1]++;
            else if(roles[1]!=null&&roles[1].GetType() == 0)count[0]++;
            return count;
        }
    }

    public class RoleModel
    {
        GameObject role;
        int role_type;             //0为牧师，1为魔鬼
        Click click;
        bool on_boat;              //是否在船上  
        LandModel land_model = (SSDirector.GetInstance().CurrentScenceController as Controllor).from_land;
        public float move_speed = 250;
        public RoleModel(string role_name)
        {
            if (role_name == "priest")
            {
                role = Object.Instantiate(Resources.Load("Prefabs/Priest", typeof(GameObject)), Vector3.zero, Quaternion.identity) as GameObject;
                role_type = 0;
            }
            else
            {
                role = Object.Instantiate(Resources.Load("Prefabs/Devil", typeof(GameObject)), Vector3.zero, Quaternion.identity) as GameObject;
                role_type = 1;
            }
            click = role.AddComponent(typeof(Click)) as Click;
            click.SetRole(this);
        }

        public GameObject getGameObject() { return role; }
        public int GetType() { return role_type;}
        public LandModel GetLandModel(){return land_model;}
        public string GetName() { return role.name; }
        public bool IsOnBoat() { return on_boat; }
        public void SetName(string name) { role.name = name; }
        public void SetPosition(Vector3 pos) { role.transform.position = pos; }

        public void GoLand(LandModel land)
        {  
            role.transform.parent = null;
            land_model = land;
            on_boat = false;
        }

        public void GoBoat(BoatModel boat)
        {
            role.transform.parent = boat.GetBoat().transform;
            land_model = null;          
            on_boat = true;
        }

        public void Reset()
        {
            land_model = (SSDirector.GetInstance().CurrentScenceController as Controllor).from_land;
            GoLand(land_model);
            SetPosition(land_model.GetEmptyPosition());
            land_model.AddRole(this);
        }
    }

    public class LandModel
    {
        GameObject land;                                
        Vector3[] positions;                            //保存每个角色放在陆地上的位置
        int land_type;                                  //终点陆地标志为0，起点陆地标志为1
        RoleModel[] roles = new RoleModel[6];           //陆地上有的角色

        public LandModel(string land_name)
        {
            positions = new Vector3[] {new Vector3(-6.5F,2,0), new Vector3(-7.5F,2,0), new Vector3(-8.5F,2,0),new Vector3(-9.5F,2,0), new Vector3(-10.5F,2,0), new Vector3(-11.5F,2,0)};
            if (land_name == "from")
            {
                land = Object.Instantiate(Resources.Load("Prefabs/Land", typeof(GameObject)), new Vector3(11, -0.5F, 0), Quaternion.identity) as GameObject;
                land_type = 1;
            }
            else
            {
                land = Object.Instantiate(Resources.Load("Prefabs/Land", typeof(GameObject)), new Vector3(-11, -0.5F, 0), Quaternion.identity) as GameObject;
                land_type = 0;
            }
        }

        public int GetEmptyIndex()                      //得到陆地上哪一个位置是空的
        {
            for (int i = 0; i < 6; i++)
            {
                if (roles[i] == null)
                    return i;
            }
            return -1;
        }

        public int GetLandType() { return land_type; }

        //得到陆地上空位置
        public Vector3 GetEmptyPosition()               
        {
            Vector3 pos = positions[GetEmptyIndex()];
            if(land_type == 0)pos.x = -pos.x;                  //两个陆地是x坐标对称，起点为负，终点为正
            return pos;
        }

        public void AddRole(RoleModel role)             
        {
            roles[GetEmptyIndex()] = role;
        }

        public RoleModel DeleteRole(string role_name)      
        { 
            for (int i = 0; i < 6; i++)
            {
                if (roles[i] != null && roles[i].GetName() == role_name)
                {
                    RoleModel role = roles[i];
                    roles[i] = null;
                    return role;
                }
            }
            return null;
        }

        //计算陆地上牧师和魔鬼的数量
        public int[] GetRoleType()
        {
            int[] count = { 0, 0 };                    //count[0]是牧师数，count[1]是魔鬼数
            for (int i = 0; i < 6; i++)
            {
                if (roles[i] != null)
                {
                    if (roles[i].GetType() == 0)
                        count[0]++;
                    else
                        count[1]++;
                }
            }
            return count;
        }

        public void Reset()
        {
            roles = new RoleModel[6];
        }
    }

    public class Click : MonoBehaviour
    {
        IUserAction action;
        RoleModel role = null;
        BoatModel boat = null;
        public void SetRole(RoleModel role)
        {
            this.role = role;
        }
        public void SetBoat(BoatModel boat)
        {
            this.boat = boat;
        }
        void Start()
        {
            action = SSDirector.GetInstance().CurrentScenceController as IUserAction;
        }

        //鼠标点击角色或船时调用控制器中的相关函数        
        void OnMouseDown()
        {
            if (boat != null)
                action.MoveBoat();
            else if(role != null)
                action.MoveRole(role);
        }
    }
}