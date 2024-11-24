using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffsPermanenteManager : MonoBehaviour , ISave
{
    [Header("Config Manager")]
    public GameObject buffsPermanenteUI;
    [SerializeField] private Collider mesaTrigger;
    [SerializeField] private LinhaBuffPermanente[] levelAtual;
    [SerializeField] private GameObject itemInventario;
    [SerializeField] private GameObject posItens;
    [SerializeField] private GameObject posItens2;
    public Scrollbar scrollbar;
    public bool uiLigada;
    private GameObject[] itens;
    private bool mudaSpawn;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI tituloTXT;
    [SerializeField] private TextMeshProUGUI descricaoTXT;
    [SerializeField] private TextMeshProUGUI msgErroTXT;
    private bool msgErroAtiva;
    private float tempoMsgAtiva;

    //Outros Scripts
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
    }
    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && uiLigada)
        {
            uiLigada = false;

            buffsPermanenteUI.SetActive(false);
            mesaTrigger.enabled = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            
            EventBus.Instance.PauseGame();
            Time.timeScale = 1;
            EventBus.Instance.PodePausar(true);
        }

        if (msgErroAtiva)
        {
            msgErroTXT.gameObject.transform.position = new Vector3(msgErroTXT.gameObject.transform.position.x, msgErroTXT.gameObject.transform.position.y + (Time.unscaledDeltaTime / 0.03f), msgErroTXT.gameObject.transform.position.z);

            tempoMsgAtiva -= Time.unscaledDeltaTime;

            if(tempoMsgAtiva <= 0)
            {
                msgErroTXT.color = new Color(msgErroTXT.color.r, msgErroTXT.color.g, msgErroTXT.color.b, msgErroTXT.color.a - (Time.unscaledDeltaTime / 0.7f));

                if(msgErroTXT.color.a <= 0)
                {
                    msgErroAtiva = false;
                    msgErroTXT.enabled = false;
                } 
            }
        }
    }

    public void CarregaInventario()
    {
        itens = new GameObject[inventarioSystem.listaItens.Length];

        for (int i = 0; i < itens.Length; i++)
        {
            if (!mudaSpawn)
            {
                itens[i] = Instantiate(itemInventario, posItens.transform);
                mudaSpawn = true;
            }
            else
            {
                itens[i] = Instantiate(itemInventario, posItens2.transform);
                mudaSpawn = false;
            }

            itens[i].GetComponent<ItemInventario>().imagem.sprite = inventarioSystem.spritesItens[i].sprite;
        }
    }

    public void AtualizaInventario()
    {
        for(int i = 0; i < itens.Length; i++)
        {
            itens[i].GetComponent<ItemInventario>().AtualizaTexto(inventarioSystem.listaItens[i].nomeItem, inventarioSystem.quantidadeTotalItem[i]);
        }
    }

    public void AtualizaUI(string titulo, string descricao, string buff)
    {
        tituloTXT.text = titulo;
        descricaoTXT.text = descricao + "\n" + "\n" + buff;
    }

    public void MsgErro(string msgErro, Vector3 posMsg)
    {
        msgErroTXT.text = msgErro;
        msgErroTXT.transform.position = posMsg;

        tempoMsgAtiva = 1.5f;
        msgErroTXT.color = new Color(msgErroTXT.color.r, msgErroTXT.color.g, msgErroTXT.color.b, 1);
        msgErroTXT.enabled = true;

        msgErroAtiva = true;
    }

    public void MostraDebito(int[] debitos, ItemDropado[] nomesItens, bool ativaDebito)
    {
        for (int j = 0; j < nomesItens.Length; j++)
        {
            for (int i = 0; i < itens.Length; i++)
            {
                if (nomesItens[j].nomeItem == inventarioSystem.listaItens[i].nomeItem)
                {
                    itens[i].GetComponent<ItemInventario>().MostraDebito(debitos[j], ativaDebito);
                }
            }
        }
    }

    public void QualBuff(int idBuff, int levelBuff)
    {
        switch (idBuff)
        {
            case 0:

                //função para aprimorar pistola com base no level do buff
                PistolaPermanente(levelBuff);

                break;

            case 1:

                //função para aprimorar shotgun com base no level do buff
                ShotgunPermanente(levelBuff);

                break;

            case 2:

                //função para aprimorar shuriken com base no level do buff
                ShurikenPermanente(levelBuff);

                break;

            case 3:

                //função para aprimorar vida maxima com base no level do buff
                VidaPermanente(levelBuff);

                break;

            case 4:

                //função para aprimorar cura por sala com base no level do buff
                CuraPermanente(levelBuff);

                break;

            case 5:

                //função para aprimorar energia da manopla com base no level do buff
                EnergiaPermanente(levelBuff);

                break;

            case 6:

                //função para aprimorar coleta de recursos com base no level do buff

                break;

            case 7:

                //função para aprimorar dash com base no level do buff
                DashPermanente(levelBuff);

                break;

            case 8:

                //função para aprimorar proffessores com base no level do buff
                
                break;

            case 9:

                //função para aprimorar quantidade de buffs na manopla com base no level do buff
                
                break;

            case 10:

                //função para desbloquear ult com base no level do buff
                
                break;

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

        statusJogador.AtualizaStatus();
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

        statusJogador.AtualizaStatus();
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

        statusJogador.AtualizaStatus();
    }

    private void PistolaPermanente(int levelBuff)
    {
        switch (levelBuff)
        {
            case 1:
                statusJogador.danoBasePistola = 11;
                break;

            case 2:
                statusJogador.danoBasePistola = 12;
                break;

            case 3:
                statusJogador.danoBasePistola = 13;
                break;

            case 4:
                statusJogador.danoBasePistola = 15;
                break;
        }

        statusJogador.AtualizaStatus();
    }

    private void ShotgunPermanente(int levelBuff)
    {
        switch (levelBuff)
        {
            case 0:
                //LIBERAR A SHOTGUN
                break;

            case 1:
                statusJogador.danoBaseShotgun = 11;
                break;

            case 2:
                statusJogador.danoBaseShotgun = 12;
                break;

            case 3:
                statusJogador.danoBaseShotgun = 13;
                break;

            case 4:
                statusJogador.danoBaseShotgun = 15;
                break;
        }
        statusJogador.AtualizaStatus();

    }

    private void ShurikenPermanente(int levelBuff)
    {
        switch (levelBuff)
        {
            case 0:
                //LIBERAR A SHURIKEN
                break;

            case 1:
                statusJogador.danoBaseShuriken = 33;
                break;

            case 2:
                statusJogador.danoBaseShotgun = 36;
                break;

            case 3:
                statusJogador.danoBaseShotgun = 39;
                break;

            case 4:
                statusJogador.danoBaseShotgun = 45;
                break;
        }

        statusJogador.AtualizaStatus();
    }

    private void CuraPermanente(int levelBuff)
    {
        switch (levelBuff)
        {
            case 1:
                statusJogador.curaPorSala = 1f;
                break;

            case 2:
                statusJogador.curaPorSala = 2f;
                break;

            case 3:
                statusJogador.curaPorSala = 3f;
                break;
        }

        statusJogador.AtualizaStatus();
    }
    #endregion

    #region Save&Load

    public void CarregarSave(InfosSave save)
    {
        for(int i = 0; i < levelAtual.Length; i++)
        {
            levelAtual[i].desbloqueado = save.buffDesbloqueado[i];
            if (levelAtual[i].desbloqueado)
            {
                levelAtual[i].levelAtualBuff = save.levelBuffPermanente[i];  
                QualBuff(levelAtual[i].idBuff, levelAtual[i].levelAtualBuff);
                levelAtual[i].AtulizaBuffsDesbloqueados(levelAtual[i].levelAtualBuff);
            }
        }

        CarregaInventario();
    }

    public void SalvarSave(ref InfosSave save)
    {
        for (int i = 0; i < levelAtual.Length; i++)
        {   
            save.buffDesbloqueado[i] = levelAtual[i].desbloqueado;
            if (save.buffDesbloqueado[i])
            { 
                save.levelBuffPermanente[i] = levelAtual[i].levelAtualBuff - 1;
            }
        }
    }

    #endregion
}
