using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;

public class BuffManager : MonoBehaviour
{
    [SerializeField] private GameObject[] botoesDefesa;
    [SerializeField] private GameObject[] botoesCorrosao;
    [SerializeField] private GameObject[] botoesCritico;
    [SerializeField] private GameObject[] botoesMovimentacao;
    [SerializeField] private GameObject[] botoesBurst;

    private List<GameObject> botoesDefesaList;
    private List<GameObject> botoesCorrosaoList;
    private List<GameObject> botoesCriticoList;
    private List<GameObject> botoesMovimentacaoList;
    private List<GameObject> botoesBurstList;

    private StatusJogador statusJogador;

    [SerializeField] private GameObject menuBuffsUI;

    private void Start()
    {
        statusJogador = FindObjectOfType<StatusJogador>();

        PassandoArrayPraList();
    }

    private void PassandoArrayPraList()
    {
        botoesDefesaList = botoesDefesa.ToList();
        botoesCorrosaoList = botoesCorrosao.ToList();
        botoesCriticoList = botoesCritico.ToList();
        botoesMovimentacaoList = botoesMovimentacao.ToList();
        botoesBurstList = botoesBurst.ToList();
    }

    public void SorteandoQualArvore()
    {
        int i = Random.Range(1, 5);

        //Debug.Log(i);

        switch(i)
        {
            case 1:
                SorteandoQuaisBuffs(botoesDefesaList);
                //Debug.Log("Defesa");
                break;

            case 2:
                SorteandoQuaisBuffs(botoesCorrosaoList);
                //Debug.Log("Corrosao");
                break;

            case 3:
                SorteandoQuaisBuffs(botoesCriticoList);
                break;

            case 4:
                SorteandoQuaisBuffs(botoesMovimentacaoList);
                //Debug.Log("Movimentacao");
                break;

            case 5:
                SorteandoQuaisBuffs(botoesBurstList);
                //Debug.Log("Burst");
                break;
        }
        
    }

    private void SorteandoQuaisBuffs(List<GameObject> buffs)
    {
        List<int> numbers = new List<int>();

        for(int i = 0; i < buffs.Count; i++)
        {
            numbers.Add(i);
            Debug.Log(i);
        }
        

        List<int> numeroUsados = new List<int>();

        if(buffs.Count > 3)
        {
            int primeiroBuff = Random.Range(0, buffs.Count - 1);
            numeroUsados.Add(primeiroBuff);

            buffs[primeiroBuff].gameObject.SetActive(true);


            int segundoBuff = Random.Range(0, buffs.Count - 1);



            while (numeroUsados.Contains(segundoBuff))
            {
                segundoBuff = Random.Range(0, buffs.Count - 1);
            }


            numeroUsados.Add(segundoBuff);

            buffs[segundoBuff].gameObject.SetActive(true);


            int terceiroBuff = Random.Range(0, buffs.Count - 1);

            while (numeroUsados.Contains(terceiroBuff))
            {
                terceiroBuff = Random.Range(0, buffs.Count - 1);
            }

            buffs[terceiroBuff].gameObject.SetActive(true);
        } else
        {
            foreach(GameObject botao in buffs)
            {
                botao.SetActive(true);
            }
        }
        

        menuBuffsUI.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

    public void DesativandoTodosOsBotoes()
    {
        for(int i = 0; i < botoesDefesa.Length; i++)
        {
            botoesDefesa[i].SetActive(false);
        }
        for (int i = 0; i < botoesCorrosao.Length; i++)
        {
            botoesCorrosao[i].SetActive(false);
        }
        for (int i = 0; i < botoesCritico.Length; i++)
        {
            botoesCritico[i].SetActive(false);
        }
        for (int i = 0; i < botoesMovimentacao.Length; i++)
        {
            botoesMovimentacao[i].SetActive(false);
        }
        for (int i = 0; i < botoesBurst.Length; i++)
        {
            botoesBurst[i].SetActive(false);
        }

        menuBuffsUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RetirandoBuffDaLista(int qualArvore, GameObject botaoBuff) 
    {
        switch(qualArvore)
        {
            case 1:
                botoesDefesaList.Remove(botaoBuff);
                Debug.Log(botaoBuff);
                break;

            case 2:
                botoesCorrosaoList.Remove(botaoBuff);
                Debug.Log(botaoBuff);
                break;

            case 3:
                botoesCriticoList.Remove(botaoBuff);
                break;

            case 4:
                botoesMovimentacaoList.Remove(botaoBuff);
                Debug.Log(botaoBuff);
                break;

            case 5:
                botoesBurstList.Remove(botaoBuff);
                Debug.Log(botaoBuff);
                break;
        }
    }
}
