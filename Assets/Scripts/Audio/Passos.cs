using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passos : MonoBehaviour
{
    private LayerMask lm;
    private int materialValue;
    private RaycastHit rh;


    void Start()
    {
        lm = LayerMask.GetMask("WhatIsGround");
    }

    void RunEvent()
    {
        Debug.Log("Run event");

        MaterialCheck();
        EventInstance run = AudioManager.instance.CreateEventInstance(FMODEvents.instance.passos);
        RuntimeManager.AttachInstanceToGameObject(run, transform, true);

        run.setParameterByName("terreno", materialValue);

        run.start();
        run.release();
    }

    private void MaterialCheck()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out rh, 0.3f, lm))
        {
            Debug.Log(rh.collider.tag + "" + materialValue);
            switch(rh.collider.tag)
            {
                case "Metal":
                    materialValue = 1;
                    break;

                case "Concreto":
                    materialValue = 0;
                    break;

                case "Mix":
                    materialValue = 2;
                    break;
            }
        }
    }
}
