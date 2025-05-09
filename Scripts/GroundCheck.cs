using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public bool Inground;  
    public static Action Exittrigger;
    //if the player has not been on the ground for a long time.used to play landing audio
    bool NoGroundLongTime;

    void Start()
    {
        
    }

    void Update()
    {
        if(Inground == true)
        {
            NoGroundLongTime = false;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Inground = true;

        if (NoGroundLongTime) { Exittrigger?.Invoke(); }
        else { StopCoroutine(startlanding()); }
        NoGroundLongTime = false;
    }

    public void OnTriggerStay(Collider other)
    {
        Inground = true;
    }



    public void OnTriggerExit(Collider other)
    {
        Inground = false;
        StartCoroutine(startlanding());
    }

   IEnumerator startlanding()
    {
        yield return new WaitForSeconds(0.5f);
        NoGroundLongTime = true;
    }
        


}
