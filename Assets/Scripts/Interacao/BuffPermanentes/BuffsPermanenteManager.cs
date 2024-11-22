using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffsPermanenteManager : MonoBehaviour
{
    [SerializeField] private Collider mesaTrigger;
    [SerializeField] private LinhaBuffPermanente[] levelAtual;
    [SerializeField] private GameObject itemInventario;
    [SerializeField] private GameObject posItens;
    private GameObject[] itens;

    private StatusJogador statusJogador;
    private VidaPlayer vidaPlayer;
    private InventarioSystem inventarioSystem;
    private PlayerMovement player;
    private AmmoSystem ammoSystem;

    private void Awake()
    {
        statusJogador = FindObjectOfType<StatusJogador>();
        vidaPlayer = FindObjectOfType<VidaPlayer>();
        inventarioSystem = FindObjectOfType<InventarioSystem>();
        player = FindObjectOfType<PlayerMovement>();
        ammoSystem = FindObjectOfType<AmmoSystem>();

        itens = new GameObject[inventarioSystem.listaItens.Length];

        for (int i = 0; i < itens.Length; i++)
        {
            itens[i] = Instantiate(itemInventario, posItens.transform);
            itens[i].GetComponent<ItemInventario>().imagem.sprite = inventarioSystem.spritesItens[i].sprite;
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

    public void AtualizaInventario()
    {
        for(int i = 0; i < itens.Length; i++)
        {
            itens[i].GetComponent<ItemInventario>().AtualizaTexto(inventarioSystem.listaItens[i].nomeItem, inventarioSystem.quantidadeTotalItem[i]);
        }
    }

    public void QualBuff(int idBuff, int levelBuff)
    {
        switch (idBuff)
        {
            case 0:

                //fun��o para aprimorar pistola com base no level do buff


                levelAtual[0].levelAtualBuff++;

                break;

            case 1:

                //fun��o para aprimorar shotgun com base no level do buff
                levelAtual[1].levelAtualBuff++;

                break;

            case 2:

                //fun��o para aprimorar shuriken com base no level do buff
                levelAtual[2].levelAtualBuff++;

                break;

            case 3:

                //fun��o para aprimorar vida maxima com base no level do buff
                VidaPermanente(levelBuff);

                levelAtual[3].levelAtualBuff++;

                break;

            case 4:

                //fun��o para aprimorar cura por sala com base no level do buff
                levelAtual[4].levelAtualBuff++;

                break;

            case 5:

                //fun��o para aprimorar energia da manopla com base no level do buff
                EnergiaPermanente(levelBuff);

                levelAtual[5].levelAtualBuff++;

                break;

            case 6:

                //fun��o para aprimorar coleta de recursos com base no level do buff
                levelAtual[6].levelAtualBuff++;

                break;

            case 7:

                //fun��o para aprimorar dash com base no level do buff
                DashPermanente(levelBuff);

                levelAtual[7].levelAtualBuff++;

                break;

            case 8:

                //fun��o para aprimorar proffessores com base no level do buff
                levelAtual[8].levelAtualBuff++;

                break;

            case 9:

                //fun��o para aprimorar quantidade de buffs na manopla com base no level do buff
                levelAtual[9].levelAtualBuff++;

                break;

            case 10:

                //fun��o para desbloquear ult com base no level do buff
                levelAtual[10].levelAtualBuff++;

                break;

        }
    }

    #region Fun��es que ativam os buffs permanentes

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

    private void DashPermanente(int levelBuff)
    {
            switch (levelBuff)
        {
            case 1:
                player.dashCooldown = 3f;
                break;

            case 2:
                player.dashCooldown = 2.5f;
                break;

            case 3:
                player.dashCooldown = 2f;
                break;
        }
    }

    private void EnergiaPermanente(int levelBuff)
    {
        switch (levelBuff)
        {
            case 1:
                ammoSystem.municaoTotal = 11;
                break;

            case 2:
                ammoSystem.municaoTotal = 12;
                break;

            case 3:
                ammoSystem.municaoTotal = 13;
                break;

            case 4:
                ammoSystem.municaoTotal = 15;
                break;
        }
    }
    #endregion
}
