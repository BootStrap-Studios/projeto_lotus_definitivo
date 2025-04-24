using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnInimigos : MonoBehaviour
{
    [Header("Configura��es da Sala")]
    [SerializeField] private classSala tierSala;
    [SerializeField] private BoxCollider colliderSpawn;
    public GameObject terminalBuff;    
    [SerializeField] private CanosSpawners[] spawners;
    private List<CanosSpawners> spawnerSorteado;
    public Cover[] covers;
    public GameObject posTP;
    private List<GameObject> inimigosQueSpawnam = new List<GameObject>();  
    private int waves;
    private int wavesSpawnadas;
    private bool fimWaveAtual;

    private enum classSala
    {
        Tier0,
        Tier1,
        Tier2,
        Tier3,
        Tier4,
        Tier5,
    };

    [Header("Configura��es de Inimigos")]
    [SerializeField] private GameObject[] inimigos;
    public GameObject[] inimigosVivos;
    [SerializeField] private bool inimigoSimples;
    private int qntdInimigosSimples;
    [SerializeField] private bool inimigoSniper;
    private int qntdInimigosSniper;
    [SerializeField] private bool inimigoExplosivo;
    private int qntdInimigosExplosivo;
    [SerializeField] private bool inimigoTorreta;
    private int qntdInimigosTorreta;
    private int minInimigos;
    private int maxInimigos;
    private int inimigosTotais = 0;
    private int[] inimigosPorWave;
    private int inimigosInstanciados;

    private ObjectPool objectPool;  

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(waves > 0)
            {
                colliderSpawn.enabled = false;
                ColocandoMusica();
                AtivandoInimigos();
            }   
        }       
    }

    private void OnEnable()
    {
        EventBus.Instance.onGameOver += ReativarSpawn;
    }
    private void OnDisable()
    {
        EventBus.Instance.onGameOver -= ReativarSpawn;
    }

    private void Awake()
    {
        objectPool = FindObjectOfType<ObjectPool>();        
    }

    private void Update()
    {
        if (fimWaveAtual)
        {
            VerificaInimigosVivos();
        }
    }

    [ContextMenu("SpawnInimigos")]
    public void RanomizandoInimigos()
    {
        //Debug.Log("Randomizando Inimigos");

        switch (tierSala)
        {
            case classSala.Tier1:

                waves = 1;

                minInimigos = spawners.Length / 2;
                maxInimigos = spawners.Length;

                inimigosTotais = Random.Range(minInimigos, maxInimigos + 1);

                break;

            case classSala.Tier2:

                minInimigos = spawners.Length / 2;
                maxInimigos = spawners.Length + (spawners.Length / 2);

                inimigosTotais = Random.Range(minInimigos, maxInimigos + 1);

                if (inimigosTotais < spawners.Length ) waves = 1;
                else waves = 2;             

                break;

            case classSala.Tier3:

                minInimigos = spawners.Length;
                maxInimigos = spawners.Length * 2;

                inimigosTotais = Random.Range(minInimigos, maxInimigos + 1);

                if (inimigosTotais < spawners.Length + (spawners.Length / 2)) waves = 2 ;
                else waves = 3;

                break;

            case classSala.Tier4:

                minInimigos = spawners.Length + (spawners.Length / 2);
                maxInimigos = (spawners.Length * 2) + (spawners.Length / 2);

                inimigosTotais = Random.Range(minInimigos, maxInimigos + 1);

                if (inimigosTotais < spawners.Length * 2) waves = 3;
                else waves = 4;


                break;

            case classSala.Tier5:

                minInimigos = spawners.Length * 2;
                maxInimigos = spawners.Length * 3;

                inimigosTotais = Random.Range(minInimigos, maxInimigos + 1);

                if (inimigosTotais < (spawners.Length * 2) + (spawners.Length / 2)) waves = 4;
                else waves = 5;

                break;

            case classSala.Tier0:
                waves = 0;
                inimigosTotais = 0;
                break;
        }

        RandomizandoInimigosPorWave();

        //Debug.Log("Min de Inimigos ---> " + minInimigos);
        //Debug.Log("Max de Inimigos ---> " + maxInimigos);
        //Debug.Log("Waves de Inimigos ---> " + waves);
        Debug.Log("Total de Inimigos ---> " + inimigosTotais);
    }

    private void RandomizandoInimigosPorWave()
    {
        if (waves == 0) return;

        //Debug.Log("Randomizando Inimigos por Wave");

        inimigosPorWave = new int[waves];
        int inimigosRestantes = inimigosTotais;
        int wavesCalculo = waves;

        for (int i = 0; i < inimigosPorWave.Length; i++)
        {
            //Debug.Log("Waves: " + (i + 1) + "/" + waves);
            if (i + 1 < waves)
            {
                int inimigosMin = spawners.Length / 2;
                int inimigosMax = inimigosRestantes/wavesCalculo + 1;

                if (inimigosMax <= inimigosMin) inimigosPorWave[i] = inimigosMin;
                else inimigosPorWave[i] = Random.Range(inimigosMin, inimigosMax);

                //Debug.Log("Min e Max de inimigos na Wave " + (i + 1) + ": " + inimigosMin + "/" + (inimigosMax - 1));
                //Debug.Log("Inimigos na Wave " + (i + 1) + ": " + inimigosPorWave[i]);

                wavesCalculo--;

                if ((inimigosRestantes - inimigosPorWave[i]) / wavesCalculo > spawners.Length)
                {
                    while ((inimigosRestantes - inimigosPorWave[i]) / wavesCalculo > spawners.Length) inimigosPorWave[i]++;

                    Debug.LogWarning("N�mero de inimigos na Wave " + (i + 1) + " foram insuficientes");
                }

                inimigosRestantes -= inimigosPorWave[i];

                //Debug.Log("Inimigos Restantes: " + inimigosRestantes);
            }
            else
            {
                inimigosPorWave[i] = inimigosRestantes;
                //Debug.Log("Inimigos na Wave " + (i + 1) + ": " + inimigosPorWave[i]);
            }
        }

        objectPool.DeterminaPool(inimigosTotais, waves);
        //Debug.Log("Inimigos Totais: " + inimigosTotais);

        SpawnaInimigos();
    }

    private void SpawnaInimigos()
    {

        if (inimigoSimples)
        {
            inimigosQueSpawnam.Add(inimigos[0]);
            qntdInimigosSimples = 0;
        }

        if (inimigoSniper)
        {
            inimigosQueSpawnam.Add(inimigos[1]);
            qntdInimigosSniper = 0;
        }

        if (inimigoExplosivo)
        {
            inimigosQueSpawnam.Add(inimigos[2]);
            qntdInimigosExplosivo = 0;
        }

        if (inimigoTorreta)
        {
            inimigosQueSpawnam.Add(inimigos[3]);
            qntdInimigosTorreta = 0;
        }

        inimigosVivos = new GameObject[inimigosTotais];
        wavesSpawnadas = 0;
        inimigosInstanciados = 0;

        Vector3 posSpawn = new Vector3(transform.position.x, transform.position.y - 10, transform.position.z);

        for (int i = 0; i < inimigosTotais; i++)
        {
            inimigosVivos[i] = Instantiate(inimigosQueSpawnam[Random.Range(0, inimigosQueSpawnam.Count)], posSpawn, gameObject.transform.rotation);
            inimigosVivos[i].name = "Inimigo " + i;
            inimigosVivos[i].GetComponentInChildren<Inimigo>().spawnInimigos = this;
            inimigosVivos[i].SetActive(false);

            if (inimigosVivos[i].GetComponent<Inimigo>().inimigoSniper)
            {
                qntdInimigosSniper++;

                if (qntdInimigosSniper >= inimigosTotais / 4)
                {
                    inimigosQueSpawnam.Remove(inimigos[1]);
                    Debug.Log("Total de Inimigos Sniper ---> " + qntdInimigosSniper);
                }
            }
            else if (inimigosVivos[i].GetComponent<Inimigo>().inimigoExplosivo)
            {
                qntdInimigosExplosivo++;

                if (qntdInimigosExplosivo >= inimigosTotais / 3)
                {
                    inimigosQueSpawnam.Remove(inimigos[2]);
                    Debug.Log("Total de Inimigos Explosivo ---> " +  qntdInimigosExplosivo);
                }
            }
            else if (inimigosVivos[i].GetComponent<Inimigo>().inimigoTorreta)
            {
                qntdInimigosTorreta++;

                if (qntdInimigosTorreta >= inimigosTotais / 5)
                {
                    inimigosQueSpawnam.Remove(inimigos[3]);
                    Debug.Log("Total de Inimigos Torreta ---> " + qntdInimigosTorreta);
                }
            }
            else
            {
                qntdInimigosSimples++;
            }
        }

        Debug.Log("Total de Inimigos Simples ---> " + qntdInimigosSimples);
    }

    [ContextMenu("Ativa Inimigos")]
    private void AtivandoInimigos()
    {
        spawnerSorteado = new List<CanosSpawners>();
        for (int i = 0; i < spawners.Length; i++)
        {
            spawnerSorteado.Add(spawners[i]);
        }

        for (int i = 0; i < inimigosPorWave[wavesSpawnadas]; i++)
        {
            inimigosVivos[inimigosInstanciados].SetActive(true);

            int spawnerSorteadoAtual = Random.Range(0, spawnerSorteado.Count);
            //Debug.Log(spawnerSorteadoAtual);

            //Debug.Log(spawnerSorteado[spawnerSorteadoAtual].name + " foi sorteado e ser� removido do sorteio");
            spawnerSorteado[spawnerSorteadoAtual].AtivaInimigo(inimigosInstanciados);
            spawnerSorteado.Remove(spawnerSorteado[spawnerSorteadoAtual]);

            inimigosInstanciados++;
        }

        fimWaveAtual = true;
        wavesSpawnadas++;
        //Debug.Log("Wave: " + (wavesSpawnadas) + "/" + waves);
    }

    private void VerificaInimigosVivos()
    {
        int qntdInimigosMortos = 0;

        if (wavesSpawnadas < waves)
        {
            for (int i = 0; i < inimigosVivos.Length; i++)
            {
                if (inimigosVivos[i] == null)
                {
                    qntdInimigosMortos++;
                }
            }

            //Debug.Log("Mortos: " + qntdInimigosMortos + "/" + inimigosInstanciados);

            if (qntdInimigosMortos >= (inimigosInstanciados - 1))
            {                             
                fimWaveAtual = false;
                AtivandoInimigos();
            }
        }
        else
        {
            for (int i = 0; i < inimigosVivos.Length; i++)
            {
                if (inimigosVivos[i] == null)
                {
                    qntdInimigosMortos++;
                }
            }
           
            //Debug.Log("Mortos: " + qntdInimigosMortos + "/" + inimigosInstanciados);

            if (qntdInimigosMortos >= inimigosInstanciados)
            {
                fimWaveAtual = false;
                terminalBuff.SetActive(true);
                terminalBuff.GetComponent<BoxCollider>().enabled = true;
                AudioManager.instance.PlayOneShot(FMODEvents.instance.terminouSala, transform.position);

                AudioManager.instance.SetMusicParameter("area", 3);
            }
        }       
    }

    private void ReativarSpawn()
    {
        for(int i = 0; i < inimigosVivos.Length; i++)
        {
            Destroy(inimigosVivos[i].gameObject);
        }
    }

    private void ColocandoMusica()
    {
        int aux = Random.Range(1, 4);

        if (aux == 1)
        {
            AudioManager.instance.SetMusicParameter("area", 2);
        }

        if (aux == 2)
        {
            AudioManager.instance.SetMusicParameter("area", 4);
        }

        if (aux == 3)
        {
            AudioManager.instance.SetMusicParameter("area", 5);
        }
    }
}
