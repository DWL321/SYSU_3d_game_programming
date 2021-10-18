using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mygame;

namespace UmpireSpace
{
    public class Umpire
    {
        public LandModel from_land;            //左侧起点陆地
        public LandModel to_land;              //右侧终点陆地
        public BoatModel boat;                  //船

        public Umpire(LandModel FromLand,LandModel ToLand,BoatModel Boat)
        {
            from_land=FromLand;
            to_land=ToLand;
            boat=Boat;
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
}
