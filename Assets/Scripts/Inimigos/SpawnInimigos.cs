using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnInimigos : MonoBehaviour
{
    [Header("Configurações do Spawn")]
    [SerializeField] private GameObject[] inimigos;
    [SerializeField] private CanosSpawners[] spawners;
    [SerializeField] private int tierSala;
    private int waves;
    private int wavesSpawnadas;
    private int[] inimigosPorWave;
    private float intervaloSpawn;


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

    private void Start()
    {
        objectPool = FindObjectOfType<ObjectPool>(); 
        //RanomizandoInimigos();
    }

    private void Update()
    {
        if(intervaloSpawn > 0)
        {
            intervaloSpawn = Time.deltaTime;
        }
    }

    public void RanomizandoInimigos()
    {
        //Debug.Log("Randomizando Inimigos");
        wavesSpawnadas = 0;

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
                    Debug.Log("Inimigos Totais: " + inimigosTotais);

                    SpawnandoInimigos();
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
        Debug.Log("Inimigos Totais: " + inimigosTotais);

        SpawnandoInimigos();
    }

    private void SpawnandoInimigos()
    {     
        Debug.Log("Wave: " + (wavesSpawnadas + 1) + "/" + waves);

        for (int i = 0; i < spawners.Length; i++)
        {
            Debug.Log("Spawner: " + (i + 1) + "/" + spawners.Length);
            spawners[i].SpawnandoInimigo(inimigos[Random.Range(0, 3)]);

            inimigosPorWave[wavesSpawnadas] --;
            Debug.Log("InimigosRestantes: " + inimigosPorWave[wavesSpawnadas]);

            if (inimigosPorWave[wavesSpawnadas] == 0)
            {
                break;
            }
        }
        if(inimigosPorWave[wavesSpawnadas] > 0)
        {
            SpawnandoInimigos();
        }
        else if((wavesSpawnadas + 1) < waves)
        {
            wavesSpawnadas++;
            SpawnandoInimigos();
        }
    }
}
