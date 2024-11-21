using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffsPermanenteManager : MonoBehaviour
{
    [SerializeField] private Collider mesaTrigger;
    [SerializeField] private int[] levelAtual;

    private StatusJogador statusJogador;
    private VidaPlayer vidaPlayer;

    private void Start()
    {
        statusJogador = FindObjectOfType<StatusJogador>();
        vidaPlayer = FindObjectOfType<VidaPlayer>();
    }
    public void QualBuff(int idBuff, int levelBuff)
    {
        switch (idBuff)
        {
            case 0:

                //função para aprimorar pistola com base no level do buff


                levelAtual[0]++;

                break;

            case 1:

                //função para aprimorar shotgun com base no level do buff
                levelAtual[1]++;

                break;

            case 2:

                //função para aprimorar shuriken com base no level do buff
                levelAtual[2]++;

                break;

            case 3:

                //função para aprimorar vida maxima com base no level do buff
                VidaPermanente(levelBuff);

                levelAtual[3]++;

                break;

            case 4:

                //função para aprimorar cura por sala com base no level do buff
                levelAtual[4]++;

                break;

            case 5:

                //função para aprimorar energia da manopla com base no level do buff
                levelAtual[5]++;

                break;

            case 6:

                //função para aprimorar coleta de recursos com base no level do buff
                levelAtual[6]++;

                break;

            case 7:

                //função para aprimorar dash com base no level do buff
                levelAtual[7]++;

                break;

            case 8:

                //função para aprimorar proffessores com base no level do buff
                levelAtual[8]++;

                break;

            case 9:

                //função para aprimorar quantidade de buffs na manopla com base no level do buff
                levelAtual[9]++;

                break;

            case 10:

                //função para desbloquear ult com base no level do buff
                levelAtual[10]++;

                break;

        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
            mesaTrigger.enabled = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            EventBus.Instance.PauseGame();
            Time.timeScale = 1;
        }
    }

    public bool LevelDesbloqueado(int idBuff, int levelBuff)
    {
        //Debug.Log(levelAtual[idBuff] + " = " + levelBuff);

        if(levelAtual[idBuff] == levelBuff)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    #region Funções que ativam os buffs permanentes

    private void VidaPermanente(int levelBuff)
    {
        switch(levelBuff)
        {
            case 1:
                vidaPlayer.vidaMaxima = 33;
                break;

            case 2:
                vidaPlayer.vidaMaxima = 36;
                break;

            case 3:
                vidaPlayer.vidaMaxima = 39;
                break;

            case 4:
                vidaPlayer.vidaMaxima = 45;
                break;
        }
    }



    #endregion
}
