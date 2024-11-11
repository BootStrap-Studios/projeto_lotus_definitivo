using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnInimigos : MonoBehaviour
{
    [Header("Configurações do Spawn")]
    [SerializeField] private int minInimigos;
    [SerializeField] private int maxInimigos;
    [SerializeField] private BoxCollider colliderSpawn;
    [SerializeField] private GameObject terminalBuff;
    [SerializeField] private GameObject[] inimigos;    
    public GameObject[] inimigosVivos;    
    [SerializeField] private CanosSpawners[] spawners;
    [SerializeField] private int tierSala;
    [SerializeField] private float intervaloSpawn;
    private List<GameObject> inimigosQueSpawnam = new List<GameObject>();
    private int waves;
    private int wavesSpawnadas;
    private int[] inimigosPorWave;   
    private int inimigosInstanciados;
    private bool fimWaveAtual;
    private bool pauseWave;
    private float intervaloSpawnAux;


    [Header("Tipos de Inimigos")]
    [SerializeField] private bool inimigoSimples;
    private int inimigosSimples;
    [SerializeField] private bool inimigoSniper;
    private int inimigosSniper;
    [SerializeField] private bool inimigoExplosivo;
    private int inimigosExplosivo;
    [SerializeField] private bool inimigoTorreta;
    private int inimigosTorreta;
    private int inimigosTotais = 0;

    private ObjectPool objectPool;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(waves > 0)
            {
                colliderSpawn.enabled = false;
                AtivandoInimigos();
            }   
        }       
    }

    private void Start()
    {
        objectPool = FindObjectOfType<ObjectPool>();        
    }

    private void Update()
    {
        /*if (pauseWave)
        {
            intervaloSpawnAux -= Time.deltaTime;

            if(intervaloSpawnAux <= 0)
            {               
                pauseWave = false;
                AtivandoInimigos();
            }
        }*/

        if (fimWaveAtual)
        {
            VerificaInimigosVivos();
        }
    }

    [ContextMenu("SpawnInimigos")]
    public void RanomizandoInimigos()
    {
        //Debug.Log("Randomizando Inimigos");        


        inimigosTotais = Random.Range(minInimigos, maxInimigos + 1 );

        waves = inimigosTotais / spawners.Length;

        if(inimigosTotais - spawners.Length >=   spawners.Length / 2)
        {
            waves++;
        }

        //Debug.Log(inimigosTotais);
        //Debug.Log(waves);

        RandomizandoInimigosPorWave();

        //sorteando quantidade de waves, de inimigos e de inimigos por wave, com base no tier e quantidade de spawners da sala
        /*switch (tierSala)
        {
            case 1:

                waves = Random.Range(1, 4);

                if (waves == 1)
                {
                    inimigosTotais = Random.Range((spawners.Length * waves) - (spawners.Length / 4), (spawners.Length * waves) + (spawners.Length / 4) + 1);
                    DebugQuantidadeInimigos();

                    inimigosPorWave = new int[waves];
                    inimigosPorWave[0] = inimigosTotais;

                    objectPool.DeterminaPool(inimigosTotais, waves);

                    SpawnaInimigos();
                }
                else
                {
                    int maxInimigos = spawners.Length * waves;
                    Debug.Log("maximos inimigos: " + maxInimigos);
                    inimigosTotais = Random.Range((spawners.Length * waves) - (spawners.Length / 2), maxInimigos + 1);
                    DebugQuantidadeInimigos();

                    RandomizandoInimigosPorWave();
                }

                break;

            case 2:

                waves = Random.Range(3, 5);

                if (waves == 3)
                {
                    inimigosTotais = Random.Range((spawners.Length * waves) - (spawners.Length / 4), (spawners.Length * waves) + (spawners.Length / 4) + 1);
                    DebugQuantidadeInimigos();

                    RandomizandoInimigosPorWave();
                }
                else
                {
                    int maxInimigos = spawners.Length * waves;
                    Debug.Log("maximos inimigos: " + maxInimigos);
                    inimigosTotais = Random.Range((spawners.Length * waves) - (spawners.Length / 2), maxInimigos + 1);
                    DebugQuantidadeInimigos();

                    RandomizandoInimigosPorWave();
                }

                break;

            case 3:

                waves = 5;

                inimigosTotais = Random.Range((spawners.Length * waves - spawners.Length), (spawners.Length * waves) + spawners.Length + 1);
                DebugQuantidadeInimigos();

                RandomizandoInimigosPorWave();

                break;

            case 0:
                Debug.Log("SEM INIMIGOS");
                waves = -5;
                break;
       
        }*/
    }

    private void DebugQuantidadeInimigos()
    {
        if(waves == 1 || waves == 3)
        {
            Debug.Log("Waves de Inimigos: " + waves);
            Debug.Log("Inimigos Min: " + ((spawners.Length * waves) - (spawners.Length / 4)));
            Debug.Log("Inimigos Max: " + ((spawners.Length * waves) + (spawners.Length / 4)));
            Debug.Log("Inimigos Sorteados: " + inimigosTotais);
        }
        else if (waves == 2 || waves == 4)
        {
            inimigosTotais = Random.Range(spawners.Length * waves, ((spawners.Length * waves) + spawners.Length));

            Debug.Log("Waves de Inimigos: " + waves);
            Debug.Log("Inimigos Min: " + ((spawners.Length * waves) - (spawners.Length / 2)));
            Debug.Log("Inimigos Max: " + (spawners.Length * waves));
            Debug.Log("Inimigos Sorteados: " + inimigosTotais);
        }
        else
        {
            Debug.Log("Waves de Inimigos: " + waves);
            Debug.Log("Inimigos Min: " + (spawners.Length * waves - (spawners.Length / 2)));
            Debug.Log("Inimigos Max: " + ((spawners.Length * waves) + spawners.Length));
            Debug.Log("Inimigos Sorteados: " + inimigosTotais);
        }
    }

    private void RandomizandoInimigosPorWave()
    {
        //Debug.Log("Randomizando Inimigos por Wave");

        inimigosPorWave = new int[waves];
        int inimigosRestantes = inimigosTotais;
        int wavesCalculo = waves;

        for (int i = 0; i < inimigosPorWave.Length; i++)
        {
            //Debug.Log("Waves: " + (i + 1) + "/" + waves);
            if (i + 1 < waves)
            {
                inimigosPorWave[i] = Random.Range(spawners.Length - 1, (inimigosRestantes / wavesCalculo) + 2);
                //Debug.Log("Min e Max de inimigos na Wave: " + (spawners.Length - 1) + "/" + ((inimigosRestantes / wavesCalculo) + 1));
                Debug.Log("Inimigos na Wave: " + inimigosPorWave[i]);

                inimigosRestantes -= inimigosPorWave[i];
                //Debug.Log("Inimigos Restantes: " + inimigosRestantes);

                wavesCalculo--;
            }
            else
            {
                inimigosPorWave[i] = inimigosRestantes;
                Debug.Log("Inimigos na Wave final: " + inimigosPorWave[i]);
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
        }

        if (inimigoSniper)
        {
            inimigosQueSpawnam.Add(inimigos[1]);
        }

        if (inimigoExplosivo)
        {
            inimigosQueSpawnam.Add(inimigos[2]);
        }

        if (inimigoTorreta)
        {
            inimigosQueSpawnam.Add(inimigos[3]);
        }

        inimigosVivos = new GameObject[inimigosTotais];
        wavesSpawnadas = 0;
        inimigosInstanciados = 0;


        Vector3 posSpawn = new Vector3(transform.position.x, transform.position.y - 10, transform.position.z);

        for (int i = 0; i < inimigosTotais; i++)
        {
            inimigosVivos[i] = Instantiate(inimigosQueSpawnam[Random.Range(0, inimigosQueSpawnam.Count)], posSpawn, gameObject.transform.rotation);
            inimigosVivos[i].SetActive(false);
        }
    }

    [ContextMenu("Ativa Inimigos")]
    private void AtivandoInimigos()
    {
        for (int i = 0; i < spawners.Length; i++)
        {
            //Debug.Log((i + 1) + "/" + inimigosPorWave[wavesSpawnadas]);

            inimigosVivos[inimigosInstanciados].SetActive(true);           

            spawners[i].AtivaInimigo(inimigosInstanciados);

            inimigosInstanciados++;
            inimigosPorWave[wavesSpawnadas]--;

            if (inimigosPorWave[wavesSpawnadas] == 0)
            {
                break;
            }
          
        }
      
        if (inimigosPorWave[wavesSpawnadas] > 0)
        {
            //intervaloSpawnAux = intervaloSpawn;
            //pauseWave = true;

            AtivandoInimigos();
        }
        else
        {           
            wavesSpawnadas++;
            Debug.Log("Wave: " + (wavesSpawnadas) + "/" + waves);
            fimWaveAtual = true;
        }
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
            }
        }       
    }
}
