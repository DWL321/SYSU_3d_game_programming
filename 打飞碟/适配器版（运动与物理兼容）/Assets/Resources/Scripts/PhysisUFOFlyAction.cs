using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PhysisUFOFlyAction : SSAction
{
    private Vector3 start_vector;                              //初速度向量
    public float power;
    public int round;
    private PhysisUFOFlyAction() { }
    public static PhysisUFOFlyAction GetSSAction(Vector3 direction, float angle, float power)
    {
        //初始化物体将要运动的初速度向量
        PhysisUFOFlyAction action = CreateInstance<PhysisUFOFlyAction>();
        if (direction.x == -1)
        {
            action.start_vector = Quaternion.Euler(new Vector3(0, 0, -angle)) * Vector3.left * power;
        }
        else
        {
            action.start_vector = Quaternion.Euler(new Vector3(0, 0, angle)) * Vector3.right * power;
        }
        action.power = power;
        return action;
    }

    public override void FixedUpdate()
    {
        //判断是否超出范围
        if (this.transform.position.y < -3)
        {
            this.destroy = true;
            this.callback.SSActionEvent(this);
        }
    }
    public override void Update() { }
    public override void Start()
    {
        //使用重力加一个向上的加速度给飞碟一个随round增大而增大的向下的加速度
        gameobject.GetComponent<Rigidbody>().useGravity = true;
        gameobject.GetComponent<Rigidbody>().AddForce(new Vector3(0, 9.77f - 0.01f * round, 0),ForceMode.Acceleration);
        gameobject.GetComponent<Rigidbody>().velocity = power * 7 * start_vector;
    }
}