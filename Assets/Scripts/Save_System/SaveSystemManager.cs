using UnityEngine;
using System.Linq;
using Ink.Parsed;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SaveSystemManager : MonoBehaviour
{
    [Header("Save Store Config")]
    [SerializeField] private string saveName;
    [SerializeField] private bool useEncryption;
    [SerializeField] private bool criarNovoJogo;
    private FileSaveHandler fileSaveHandler;
    private InfosSave infosSave;
    private List<ISave> saveObjects;

    public static SaveSystemManager instance {get; private set;}

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Mais de um SaveSystem encontrado. Deletando este...");
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);

        this.fileSaveHandler = new FileSaveHandler(Application.persistentDataPath, saveName, useEncryption);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        EventBus.Instance.onSalvarJogo += SalvarJogo;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        EventBus.Instance.onSalvarJogo -= SalvarJogo;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        this.saveObjects = FindAllSaveObjects();
    }

    public void OnSceneUnloaded(Scene scene)
    {
        //SalvarJogo();
    }

    public void NovoJogo()
    {
        this.infosSave = new InfosSave();
        Debug.Log("Criando um Novo Jogo");
        SalvarJogo();
    }

    public void CarregarJogo()
    {
        this.infosSave = fileSaveHandler.Carregar();

        if(this.infosSave == null && criarNovoJogo)
        {
            NovoJogo();
        }

        if(this.infosSave == null)
        {
            Debug.LogError("Nenhum arquivo de save encontrado. É nescessário criar um novo jogo");
            return;
        }
       
        foreach(ISave saveObj in saveObjects)
        {
            saveObj.CarregarSave(infosSave);
        }
    }

    public void SalvarJogo()
    {
        Debug.Log("Salvando o jogo");

        if (this.infosSave == null)
        {
            Debug.LogError("Nenhum arquivo de save encontrado. É nescessário criar um novo jogo");
            return;
        }

        foreach (ISave saveObj in saveObjects)
        {
            saveObj.SalvarSave(ref infosSave);
        }

        fileSaveHandler.Salvar(infosSave);
    }

    private List<ISave> FindAllSaveObjects()
    {
        IEnumerable<ISave> savesObjects = FindObjectsOfType<MonoBehaviour>().OfType<ISave>();

        return new List<ISave>(savesObjects);
    }

    public bool TemSave()
    {
        if(infosSave == null)
        {
            //Debug.Log("Não tem save");
            return false;
        }
        else
        {
            //Debug.Log("Tem save");
            return true;
        }
    }
}
