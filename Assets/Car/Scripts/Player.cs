using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Car car;
    [SerializeField] RocketBoost rocketBoost;
    [SerializeField] AirControl airControl;
    void Update()
    {
        if(!car) return;
        car.Drive(Input.GetAxisRaw("Vertical"));
        car.Turn(Input.GetAxisRaw("Horizontal"));

        rocketBoost.ToggleBoost(Input.GetKey(KeyCode.LeftShift));

        if(Input.GetKeyDown(KeyCode.Space)) airControl.Jump();
        airControl.Pitch(Input.GetAxisRaw("Vertical"));
        airControl.Yaw(Input.GetAxisRaw("Horizontal"));
        airControl.Roll(Input.GetAxisRaw("Roll"));
    }
}
