using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnInimigos : MonoBehaviour
{
    [Header("Configurações da Sala")]
    [SerializeField] private int tierSala;  
    [SerializeField] private BoxCollider colliderSpawn;
    [SerializeField] private GameObject terminalBuff;
    [SerializeField] private GameObject[] inimigos;    
    public GameObject[] inimigosVivos;    
    [SerializeField] private CanosSpawners[] spawners;
    public Cover[] covers;
    public GameObject posTP;
    private List<GameObject> inimigosQueSpawnam = new List<GameObject>();
    private int minInimigos;
    private int maxInimigos;
    private int waves;
    private int wavesSpawnadas;
    private int[] inimigosPorWave;   
    private int inimigosInstanciados;
    private bool fimWaveAtual;


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
            case 1:

                minInimigos = spawners.Length - (spawners.Length / 3);
                maxInimigos = spawners.Length + 1;

                waves = Random.Range(1, 3);

                inimigosTotais = Random.Range(minInimigos * waves, maxInimigos * waves);

                //waves = 1;
                //inimigosTotais = 3;

                break;

            case 2:

                minInimigos = spawners.Length - (spawners.Length / 3);
                maxInimigos = spawners.Length + 1;

                waves = Random.Range(3, 5);

                inimigosTotais = Random.Range(minInimigos * waves, maxInimigos * waves);

                break;

            case 3:

                minInimigos = spawners.Length - (spawners.Length / 3);
                maxInimigos = spawners.Length + 1;

                waves = 5;

                inimigosTotais = Random.Range(minInimigos * waves, maxInimigos * waves);

                break;

            case 0:
                waves = 0;
                inimigosTotais = 0;
                break;
        }

        /*
        inimigosTotais = Random.Range(minInimigos, maxInimigos + 1 );

        waves = inimigosTotais / spawners.Length;

        if(inimigosTotais - (spawners.Length * waves) >=   spawners.Length / 2)
        {
            waves++;
        }
        */

        //Debug.Log("Inimigos no total: " + inimigosTotais);
        //Debug.Log("Total de waves: " + waves);

        RandomizandoInimigosPorWave();
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
                inimigosPorWave[i] = Random.Range(spawners.Length - 1, (inimigosRestantes / wavesCalculo) + 2);
                //Debug.Log("Min e Max de inimigos na Wave: " + (spawners.Length - 1) + "/" + ((inimigosRestantes / wavesCalculo) + 1));
                //Debug.Log("Inimigos na Wave " + (i+1) +": " + inimigosPorWave[i]);

                inimigosRestantes -= inimigosPorWave[i];
                //Debug.Log("Inimigos Restantes: " + inimigosRestantes);

                wavesCalculo--;
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
            inimigosVivos[i].name = "Inimigo " + i;
            inimigosVivos[i].GetComponentInChildren<Inimigo>().spawnInimigos = this;
            inimigosVivos[i].SetActive(false);
        }
    }

    [ContextMenu("Ativa Inimigos")]
    private void AtivandoInimigos()
    {
        ColocandoMusica();

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

            AtivandoInimigos();
        }
        else
        {           
            wavesSpawnadas++;
            //Debug.Log("Wave: " + (wavesSpawnadas) + "/" + waves);
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
                terminalBuff.GetComponent<BoxCollider>().enabled = true;
                AudioManager.instance.PlayOneShot(FMODEvents.instance.terminouSala, transform.position);

                AudioManager.instance.SetMusicParameter("area", 3);
            }
        }       
    }

    private void ColocandoMusica()
    {
        int aux = Random.Range(1, 3);

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
