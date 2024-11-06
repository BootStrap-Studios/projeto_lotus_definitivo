using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnInimigos : MonoBehaviour
{
    [Header("Configurações do Spawn")]
    [SerializeField] private BoxCollider colliderSpawn;
    [SerializeField] private GameObject[] inimigos;    
    public GameObject[] inimigosVivos;    
    [SerializeField] private CanosSpawners[] spawners;
    [SerializeField] private int tierSala;
    private List<GameObject> inimigosQueSpawnam = new List<GameObject>();
    private int waves;
    private int wavesSpawnadas;
    private int[] inimigosPorWave;
    private float intervaloSpawn;
    private int inimigosInstanciados;
    private bool fimWaveAtual;
    private bool pauseWaveAtual;


    [Header("Tipos de Inimigos")]
    [SerializeField] private bool inimigoSimples;
    private int inimigosSimples;
    [SerializeField] private bool inimigoSniper;
    private int inimigosSniper;
    [SerializeField] private bool inimigoExplosivo;
    private int inimigosExplosivo;
    [SerializeField] private bool inimigoTorreta;
    private int inimigosTorreta;
    private int inimigosTotais;

    private ObjectPool objectPool;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            colliderSpawn.enabled = false;
            RanomizandoInimigos();
        }       
    }

    private void Start()
    {
        objectPool = FindObjectOfType<ObjectPool>();
        
    }

    private void Update()
    {
        if(pauseWaveAtual)
        {
            intervaloSpawn -= Time.deltaTime;
            if(intervaloSpawn <= 0)
            {
                pauseWaveAtual = false;
                SpawnandoInimigos();
            }
        }

        if (fimWaveAtual)
        {
            VerificaInimigosVivos();
        }
    }

    [ContextMenu("SpawnInimigos")]
    private void RanomizandoInimigos()
    {
        //Debug.Log("Randomizando Inimigos");        

        //sorteando quantidade de waves, de inimigos e de inimigos por wave, com base no tier e quantidade de spawners da sala
        switch (tierSala)
        {
            case 1:

                waves = Random.Range(1, 3);

                if (waves == 1)
                {
                    inimigosTotais = Random.Range(spawners.Length * waves, ((spawners.Length * waves) + (spawners.Length / 2)) + 2);
                    //DebugQuantidadeInimigos();

                    inimigosPorWave = new int[waves];
                    inimigosPorWave[0] = inimigosTotais;

                    objectPool.DeterminaPool(inimigosTotais, waves);
                    //Debug.Log("Inimigos Totais: " + inimigosTotais);

                    VerificaTipoDeInimigos();
                }
                else
                {
                    inimigosTotais = Random.Range(spawners.Length * waves, ((spawners.Length * waves) + spawners.Length) + 1);
                    //DebugQuantidadeInimigos();

                    RandomizandoInimigosPorWave();
                }

                break;

            case 2:

                waves = Random.Range(3, 5);

                if (waves == 3)
                {
                    inimigosTotais = Random.Range(spawners.Length * waves, ((spawners.Length * waves) + (spawners.Length / 2)) + 2);
                    //DebugQuantidadeInimigos();

                    RandomizandoInimigosPorWave();
                }
                else
                {
                    inimigosTotais = Random.Range(spawners.Length * waves, ((spawners.Length * waves) + spawners.Length) + 1);
                    //DebugQuantidadeInimigos();

                    RandomizandoInimigosPorWave();
                }

                break;

            case 3:

                waves = 5;

                inimigosTotais = Random.Range(spawners.Length * waves, ((spawners.Length * waves) + spawners.Length + (spawners.Length / 2)) + 1);
                //DebugQuantidadeInimigos();

                RandomizandoInimigosPorWave();

                break;
       
        }
    }

    private void DebugQuantidadeInimigos()
    {
        if(waves == 1 || waves == 3)
        {
            Debug.Log("Waves de Inimigos: " + waves);
            Debug.Log("Inimigos Min: " + (spawners.Length * waves));
            Debug.Log("Inimigos Max: " + ((spawners.Length * waves) + (spawners.Length / 2) + 1));
            Debug.Log("Inimigos Sorteados: " + inimigosTotais);
        }
        else if (waves == 2 || waves == 3)
        {
            inimigosTotais = Random.Range(spawners.Length * waves, ((spawners.Length * waves) + spawners.Length));

            Debug.Log("Waves de Inimigos: " + waves);
            Debug.Log("Inimigos Min: " + (spawners.Length * waves));
            Debug.Log("Inimigos Max: " + ((spawners.Length * waves) + spawners.Length));
            Debug.Log("Inimigos Sorteados: " + inimigosTotais);
        }
        else
        {
            Debug.Log("Waves de Inimigos: " + waves);
            Debug.Log("Inimigos Min: " + (spawners.Length * waves));
            Debug.Log("Inimigos Max: " + ((spawners.Length * waves) + (spawners.Length / 2) + spawners.Length));
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
                //Debug.Log("Inimigos na Wave: " + inimigosPorWave[i]);

                inimigosRestantes -= inimigosPorWave[i];
                //Debug.Log("Inimigos Restantes: " + inimigosRestantes);

                wavesCalculo--;
            }
            else
            {
                inimigosPorWave[i] = inimigosRestantes;
                //Debug.Log("Inimigos na Wave final: " + inimigosPorWave[i]);
            }
        }

        objectPool.DeterminaPool(inimigosTotais, waves);
        //Debug.Log("Inimigos Totais: " + inimigosTotais);

        VerificaTipoDeInimigos();
    }

    private void VerificaTipoDeInimigos()
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

        SpawnandoInimigos();
    }
 
    private void SpawnandoInimigos()
    {           
        /*for(int i = 0; i < spawners.Length; i++)
        {
            spawners[i].SpawnandoInimigo(inimigosQueSpawnam[Random.Range(0, inimigosQueSpawnam.Count)], inimigosInstanciados);
            inimigosInstanciados++;
        }

        if (inimigosInstanciados + 1 < inimigosTotais)
        {
            SpawnandoInimigos();
        }
        else
        {
            inimigosInstanciados = 0;
            AtivandoInimigos();
        }*/

        for (int i = 0; i < spawners.Length; i++)
        {
            //Debug.Log("Spawner: " + (i + 1) + "/" + spawners.Length);
            spawners[i].SpawnandoInimigo(inimigosQueSpawnam[Random.Range(0, inimigosQueSpawnam.Count)], inimigosInstanciados);
            inimigosInstanciados++; 

            inimigosPorWave[wavesSpawnadas]--;
            //Debug.Log("InimigosRestantes: " + inimigosPorWave[wavesSpawnadas]);

            if (inimigosPorWave[wavesSpawnadas] == 0)
            {
                break;
            }
        }
        if (inimigosPorWave[wavesSpawnadas] > 0)
        {            
            intervaloSpawn = 2f;
            pauseWaveAtual = true;
        }
        else if ((wavesSpawnadas + 1) < waves)
        {
            wavesSpawnadas++;

            //Debug.Log("Wave: " + (wavesSpawnadas) + "/" + waves);
            fimWaveAtual = true;
        }
    }

    private void AtivandoInimigos()
    {
        for (int i = 0; i < inimigosPorWave[wavesSpawnadas]; i++)
        {
            intervaloSpawn = 1f;

            while(intervaloSpawn > 0)
            {
                intervaloSpawn -= Time.deltaTime;
            }

            inimigosVivos[inimigosInstanciados].SetActive(true);
            inimigosInstanciados++;          
        }
        if (inimigosPorWave[wavesSpawnadas] > 0)
        {
            AtivandoInimigos();
        }
        else if((wavesSpawnadas + 1) < waves)
        {
            //Debug.Log("Wave: " + (wavesSpawnadas) + "/" + waves);
            fimWaveAtual = true;
        }
    }

    private void VerificaInimigosVivos()
    {
        int qntdInimigosVivos = 0;

        for (int i = 0; i < inimigosVivos.Length; i++)
        {
            if (inimigosVivos != null)
            {
                qntdInimigosVivos++;
            }
        }

        //Debug.Log("Inimigos Vivos: " + qntdInimigosVivos);

        if (qntdInimigosVivos <= 1)
        {
            fimWaveAtual = false;
            SpawnandoInimigos();
        }
    }
}
