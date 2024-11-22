using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TP_Player : MonoBehaviour, IInteractable
{
    [SerializeField] private Vector3 posTP;
    [SerializeField] private CinemachineVirtualCamera cameraPlayer;
    [SerializeField] private bool tpFinal;
    [SerializeField] private bool instituto;
    [SerializeField] private GameObject fimUI;
    [SerializeField] private GameObject sala;
    [SerializeField] private GameObject proxSala;
    [SerializeField] private Image fader;
    private PlayerMovement player;

    private StatusJogador statusJogador;
    VidaPlayer vidaPlayer;


    private void Start()
    {
        statusJogador = FindObjectOfType<StatusJogador>();
        vidaPlayer = FindObjectOfType<VidaPlayer>();
    }
    public void Interagir()
    {
        if (tpFinal)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            fimUI.SetActive(true);
        }
        else
        {
            player = FindObjectOfType<PlayerMovement>();
            StartCoroutine(CO_TP());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (tpFinal)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                fimUI.SetActive(true);
            }
            else
            {
                player = other.GetComponent<PlayerMovement>();
                StartCoroutine(CO_TP());
            }
        }
    }

    private IEnumerator CO_TP()
    {
        EventBus.Instance.TP(true, true);

        while (fader.color.a < 1)
        {
            fader.color = new Color(0, 0, 0, fader.color.a + (Time.deltaTime / 0.5f));
            yield return null;
        }
        fader.color = new Color(0, 0, 0, 1);

        CuraPorSala();

        if (proxSala != null)
        {
            proxSala.SetActive(true);

            EventBus.Instance.TP(false, true);

            player.transform.position = proxSala.GetComponentInChildren<SpawnInimigos>().posTP.transform.position;
            player.gameObject.transform.rotation = proxSala.GetComponentInChildren<SpawnInimigos>().posTP.transform.rotation;

            EventBus.Instance.TP(true, true);
            proxSala.GetComponentInChildren<SpawnInimigos>().RanomizandoInimigos();

            //Debug.Log(spawn.gameObject.transform.eulerAngles.y);
            cameraPlayer.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value = proxSala.GetComponentInChildren<SpawnInimigos>().gameObject.transform.eulerAngles.y;
            cameraPlayer.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value = 0;
        }
        else
        {
            EventBus.Instance.TP(false, true);
            player.gameObject.transform.position = posTP;
            EventBus.Instance.TP(true, true);
        }

        yield return new WaitForSeconds(2);

        while (fader.color.a > 0)
        {
            fader.color = new Color(0, 0, 0, fader.color.a - (Time.deltaTime / 0.5f));
            yield return null;
        }
        fader.color = new Color(0, 0, 0, 0);

        EventBus.Instance.TP(true, false);

        if (instituto)
        {
            //para sair do instituto
        }
        else
        {
            sala.SetActive(false);
        }
    }

    public void JogarNovamente()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        fimUI.SetActive(false);
        SceneManager.LoadScene("Implemenetacao");
    }

    private void CuraPorSala()
    {
        vidaPlayer.CurarVida(statusJogador.curaPorSala);
    }
}
