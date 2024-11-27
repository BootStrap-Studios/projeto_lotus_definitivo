using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Playables;

public class TP_Player : MonoBehaviour, IInteractable
{
    [SerializeField] private CinemachineVirtualCamera cameraPlayer;
    [SerializeField] private bool tpFinal;
    [SerializeField] private bool instituto;
    [SerializeField] private GameObject fimUI;
    [SerializeField] private GameObject sala;
    [SerializeField] private GameObject proxSala;
    [SerializeField] private Image fader;
    [SerializeField] private GameObject ruinasLoading;

    private PlayerMovement player;
    private StatusJogador statusJogador;
    private VidaPlayer vidaPlayer;
    private MenuSystem menuSystem;


    private void Start()
    {
        statusJogador = FindObjectOfType<StatusJogador>();
        vidaPlayer = FindObjectOfType<VidaPlayer>();
        menuSystem = FindObjectOfType<MenuSystem>();
    }
    public void Interagir()
    {
        if (tpFinal)
        {
            EventBus.Instance.PauseGame();
            Time.timeScale = 0;
            menuSystem.BTNTentarNovamente();
        }
        else
        {
            player = FindObjectOfType<PlayerMovement>();
            StartCoroutine(CO_TP());
        }
    }

    private IEnumerator CO_TP()
    {
        while (fader.color.a < 1)
        {
            fader.color = new Color(0, 0, 0, fader.color.a + (Time.deltaTime / 0.5f));
            yield return null;
        }
        fader.color = new Color(0, 0, 0, 1);

        EventBus.Instance.PauseGame();
        EventBus.Instance.TP(false, true);
        EventBus.Instance.PodePausar(false);

        CuraPorSala();

        if (!instituto)
        {
            proxSala.SetActive(true);

            player.transform.position = proxSala.GetComponentInChildren<SpawnInimigos>().posTP.transform.position;
            player.gameObject.transform.rotation = proxSala.GetComponentInChildren<SpawnInimigos>().posTP.transform.rotation;
            proxSala.GetComponentInChildren<SpawnInimigos>().RanomizandoInimigos();

            //Debug.Log(spawn.gameObject.transform.eulerAngles.y);
            cameraPlayer.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value = proxSala.GetComponentInChildren<SpawnInimigos>().gameObject.transform.eulerAngles.y;
            cameraPlayer.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value = 0;

            yield return new WaitForSeconds(1f);
        }
        else
        {
            menuSystem.podeSalvar = false;

            //sala.SetActive(false);
            proxSala.SetActive(true);

            player.transform.position = proxSala.GetComponentInChildren<SpawnInimigos>().posTP.transform.position;
            player.gameObject.transform.rotation = proxSala.GetComponentInChildren<SpawnInimigos>().posTP.transform.rotation;
            proxSala.GetComponentInChildren<SpawnInimigos>().RanomizandoInimigos();

            //Debug.Log(spawn.gameObject.transform.eulerAngles.y);
            cameraPlayer.GetCinemachineComponent<CinemachinePOV>().m_HorizontalAxis.Value = proxSala.GetComponentInChildren<SpawnInimigos>().gameObject.transform.eulerAngles.y;
            cameraPlayer.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.Value = 0;

            ruinasLoading.SetActive(true);

            while (fader.color.a > 0)
            {
                fader.color = new Color(0, 0, 0, fader.color.a - (Time.deltaTime / 0.5f));
                yield return null;
            }
            fader.color = new Color(0, 0, 0, 0);

            yield return new WaitForSeconds(3f);

            while (fader.color.a < 1)
            {
                fader.color = new Color(0, 0, 0, fader.color.a + (Time.deltaTime / 0.5f));
                yield return null;
            }
            fader.color = new Color(0, 0, 0, 1);

            yield return new WaitForSeconds(.5f);
            ruinasLoading.SetActive(false);
        }

        EventBus.Instance.PauseGame();
        EventBus.Instance.TP(true, false);
        EventBus.Instance.PodePausar(true);

        while (fader.color.a > 0)
        {
            fader.color = new Color(0, 0, 0, fader.color.a - (Time.deltaTime / 0.5f));
            yield return null;
        }
        fader.color = new Color(0, 0, 0, 0);

        sala.SetActive(false);
    }

    private void CuraPorSala()
    {
        vidaPlayer.CurarVida(statusJogador.curaPorSala);
    }
}
