using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Passos : MonoBehaviour
{
    private enum TipoPassos
    {
        PLAYER,
        INIMIGO_BASE,
        INIMIGO_BOMBA,
        INIMIGO_SNIPER
    }


    private LayerMask lm;
    private int materialValue;
    private RaycastHit rh;

    [SerializeField] private TipoPassos passo;


    void Start()
    {
        lm = LayerMask.GetMask("WhatIsGround");
    }

    void RunEvent()
    {
        switch (passo) 
        {
            case TipoPassos.PLAYER:

                Debug.Log("Run event");

                MaterialCheck();
                EventInstance run = AudioManager.instance.CreateEventInstance(FMODEvents.instance.passos);
                RuntimeManager.AttachInstanceToGameObject(run, transform, true);

                run.setParameterByName("terreno", materialValue);

                run.start();
                run.release();

                break;

            case TipoPassos.INIMIGO_BASE:

                EventInstance runIB = AudioManager.instance.CreateEventInstance(FMODEvents.instance.inimigoBasePassos);
                RuntimeManager.AttachInstanceToGameObject(runIB, transform, true);

                runIB.start();
                runIB.release();

                break;

            case TipoPassos.INIMIGO_BOMBA:

                EventInstance runIBOMBA = AudioManager.instance.CreateEventInstance(FMODEvents.instance.inimigoBombaPassos);
                RuntimeManager.AttachInstanceToGameObject(runIBOMBA, transform, true);
                runIBOMBA.start();
                runIBOMBA.release();

                break;

            case TipoPassos.INIMIGO_SNIPER:

                EventInstance runIS = AudioManager.instance.CreateEventInstance(FMODEvents.instance.inimigoSniperPassos);
                RuntimeManager.AttachInstanceToGameObject(runIS, transform, true);
                runIS.start();
                runIS.release();

                break;


        }


        
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
