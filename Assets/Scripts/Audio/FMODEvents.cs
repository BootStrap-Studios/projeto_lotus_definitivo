using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Passos")]
    [field: SerializeField] public EventReference passos { get; private set; }

    [field: Header("Tiro escopeta")]
    [field: SerializeField] public EventReference manoEsco { get; private set; }

    [field: Header("Tiro pistola")]
    [field: SerializeField] public EventReference manoPistol { get; private set; }

    [field: Header("Tiro shuriken")]
    [field: SerializeField] public EventReference manoShuri { get; private set; }

    [field: Header("Morte")]
    [field: SerializeField] public EventReference morte { get; private set; }

    [field: Header("SpawnPoca")]
    [field: SerializeField] public EventReference spawnPoca { get; private set; }
    
    [field: Header("BurstProc")]
    [field: SerializeField] public EventReference burstProc { get; private set; }
    
    [field: Header("TerminouSala")]
    [field: SerializeField] public EventReference terminouSala { get; private set; }

    [field: Header("PauseGame")]
    [field: SerializeField] public EventReference pauseGame { get; private set; }

    [field: Header("AbrirTerminalBuffs")]
    [field: SerializeField] public EventReference abrirTerminalBuffs { get; private set; }

    [field: Header("AmbienteFabrica")]
    [field: SerializeField] public EventReference ambienteFabrica { get; private set; }

    [field: Header("MusicaFabrica")]
    [field: SerializeField] public EventReference musicaGame { get; private set; }

    [field: Header("TiroInimigoBase")]
    [field: SerializeField] public EventReference tiroInimigoBase { get; private set; }

    [field: Header("TiroInimigoSniper")]
    [field: SerializeField] public EventReference tiroInimigoSniper { get; private set; }

    [field: Header("TiroInimigoTorreta")]
    [field: SerializeField] public EventReference tiroInimigoTorreta { get; private set; }

    [field: Header("AvisoBomba")]
    [field: SerializeField] public EventReference avisoBomba { get; private set; }

    [field: Header("BombaExplodiu")]
    [field: SerializeField] public EventReference bombaExplodiu { get; private set; }

    [field: Header("PassosInimigoBase")]
    [field: SerializeField] public EventReference inimigoBasePassos { get; private set; }

    [field: Header("PassosInimigoBomba")]
    [field: SerializeField] public EventReference inimigoBombaPassos { get; private set; }

    [field: Header("PassosInimigoSniper")]
    [field: SerializeField] public EventReference inimigoSniperPassos { get; private set; }

    [field: Header("MenuSnapshot")]
    [field: SerializeField] public EventReference menuSnapshot { get; private set; }

    [field: Header("Recarga")]
    [field: SerializeField] public EventReference acertoRecarga { get; private set; }
    [field: SerializeField] public EventReference duranteRecarga { get; private set; }
    [field: SerializeField] public EventReference erroRecarga { get; private set; }
    [field: SerializeField] public EventReference recargaUnidade { get; private set; }
    [field: SerializeField] public EventReference recargaCompleta { get; private set; }

    [field: Header("Dash")]
    [field: SerializeField] public EventReference dashMavie { get; private set; }


    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }

        instance = this;
    }
}
