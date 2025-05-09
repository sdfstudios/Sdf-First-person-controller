using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AboveCheck : MonoBehaviour
{
    //player under than what during crouch
    public bool above;
    [SerializeField] float size = 1f; 
    //used when the player was over something before releasing the crouch button and then exited
    bool justnow;
    public static Action PlayerGetUp;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        above = Physics.Raycast(gameObject.transform.position, Vector3.up, size);
    
        if(!justnow && above)
        {
            justnow = true;
        }
        else if(justnow && !above && !FirstPersonController.inCrouchingStatic)
        {
            PlayerGetUp.Invoke();
            justnow = false;
        }
    }



}
