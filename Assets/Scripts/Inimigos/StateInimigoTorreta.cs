using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static StateInimigoTorreta;

public class StateInimigoTorreta
{
    #region Construtor dos Inimigos

    public enum STATE
    {
        DESATIVADA, ATIVADA, ATIRANDO
    }

    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    }

    public STATE stateName;
    public EVENT stage;
    protected NavMeshAgent agent;
    protected GameObject inimigoObj;
    protected Transform player;
    protected StateInimigoTorreta nextState;
    protected int municao;
    protected float alcanceMaxArma;
    protected float alcanceMinArma;
    protected float cooldownTiro;
    protected Inimigo inimigoScript;

    public StateInimigoTorreta(GameObject _inimigoObj, Inimigo _inimigoScript, NavMeshAgent _agent, Transform _player, float _alcanceMaxArma, float _alcanceMinArma, float _cooldownTiro, int _municao)
    {
        inimigoObj = _inimigoObj;
        inimigoScript = _inimigoScript;
        agent = _agent;
        player = _player;
        alcanceMaxArma = _alcanceMaxArma;
        alcanceMinArma = _alcanceMinArma;
        cooldownTiro = _cooldownTiro;
        municao = _municao;
    }

    #endregion

    public bool ativarRobo = false;
    public float tempoAUX = 0f;
    public float tempoAUX2 = 0f;
    public float cooldownTiroAux = 1f;
    public int municaoAUX = 0;

    #region Eventos
    public virtual void Enter()
    {
        stage = EVENT.UPDATE;
    }

    public virtual void Update()
    {
        stage = EVENT.UPDATE;
    }

    public virtual void Exit()
    {
        stage = EVENT.EXIT;
    }
    #endregion

    #region Processos
    public StateInimigoTorreta Process()
    {
        if (stage == EVENT.ENTER)
        {
            Enter();
        }
        else if (stage == EVENT.UPDATE)
        {
            Update();
        }
        else
        {
            Exit();
            return nextState;
        }

        return this;
    }
    #endregion

    public bool MiraPlayer()
    {
        Vector3 direcao = player.position - inimigoScript.posPontaArma.transform.position;
        float angulo = Vector3.Angle(direcao, inimigoScript.posPontaArma.transform.forward);

        RaycastHit hit;

        if (direcao.magnitude < alcanceMaxArma && angulo < 180)
        {
            if (Physics.Linecast(inimigoScript.posPontaArma.transform.position, player.position, out hit))
            {
                if (hit.transform.tag == "Player")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    public void RotacionaRobo()
    {
        Vector3 lookPos = player.transform.position - inimigoObj.transform.position;
        float angulo = Vector3.Angle(lookPos, inimigoObj.transform.forward);
        lookPos.y = player.transform.position.y - 2.08f;
        Quaternion rotation = Quaternion.LookRotation(lookPos);

        if (angulo > 25)
        {
            inimigoObj.transform.rotation = Quaternion.Slerp(inimigoObj.transform.rotation, rotation, .8f);
        }
        else
        {
            inimigoObj.transform.rotation = Quaternion.Slerp(inimigoObj.transform.rotation, rotation, .2f);
        }
    }
}

public class TorretaDesativada : StateInimigoTorreta
{
    public TorretaDesativada(GameObject _inimigoObj, Inimigo _inimigoScript, NavMeshAgent _agent, Transform _player, float _alcanceMaxArma, float _alcanceMinArma, float _cooldownTiro, int municao) : base(_inimigoObj, _inimigoScript, _agent, _player, _alcanceMaxArma, _alcanceMinArma, _cooldownTiro, municao)
    {
        stateName = STATE.DESATIVADA;
        agent.isStopped = true;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        if (ativarRobo)
        {
            nextState = new TorretaAtivada(inimigoObj, inimigoScript, agent, player, alcanceMaxArma, alcanceMinArma, cooldownTiro, municao);
            stage = EVENT.EXIT;
        }
        else
        {
            base.Update();
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class TorretaAtivada : StateInimigoTorreta
{
    public TorretaAtivada(GameObject _inimigoObj, Inimigo _inimigoScript, NavMeshAgent _agent, Transform _player, float _alcanceMaxArma, float _alcanceMinArma, float _cooldownTiro, int municao) : base(_inimigoObj, _inimigoScript, _agent, _player, _alcanceMaxArma, _alcanceMinArma, _cooldownTiro, municao)
    {
        stateName = STATE.ATIVADA;
        agent.isStopped = true;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        if (MiraPlayer())
        {
            nextState = new TorretaAtirando(inimigoObj, inimigoScript, agent, player, alcanceMaxArma, alcanceMinArma, cooldownTiro, municao);
            stage = EVENT.EXIT;
        }
        else
        {
            base.Update();
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class TorretaAtirando : StateInimigoTorreta
{
    public TorretaAtirando(GameObject _inimigoObj, Inimigo _inimigoScript, NavMeshAgent _agent, Transform _player, float _alcanceMaxArma, float _alcanceMinArma, float _cooldownTiro, int municao) : base(_inimigoObj, _inimigoScript, _agent, _player, _alcanceMaxArma, _alcanceMinArma, _cooldownTiro, municao)
    {
        stateName = STATE.ATIRANDO;
        agent.isStopped = true;
    }

    public override void Enter()
    {
        tempoAUX = cooldownTiro;
        tempoAUX2 = 8f;
        municaoAUX = 0;
        cooldownTiroAux = .35f;
        base.Enter();
    }

    public override void Update()
    {
        if (MiraPlayer() || municaoAUX > 0)
        {
            RotacionaRobo();
            tempoAUX2 = 8f;
            tempoAUX -= Time.deltaTime;

            if (tempoAUX <= 0)
            {
                if (inimigoScript.Atirar(true))
                {
                    tempoAUX = cooldownTiroAux;
                    municaoAUX++;

                    if (municaoAUX >= municao)
                    {
                        tempoAUX = cooldownTiro;
                        municaoAUX = 0;
                    }
                    else
                    {
                        base.Update();
                        return;
                    }
                }
                else
                {
                    base.Update();
                    return;
                }
            }
            else
            {
                //Debug.Log("Tempo para o tiro: " + tempoAUX);
                base.Update();
                return;
            }
        }
        else if(!MiraPlayer() && municaoAUX <= 0)
        {
            tempoAUX2 -= Time.deltaTime;
            tempoAUX = cooldownTiro;

            if (tempoAUX2 <= 0)
            {
                nextState = new TorretaAtivada(inimigoObj, inimigoScript, agent, player, alcanceMaxArma, alcanceMinArma, cooldownTiro, municao);
                stage = EVENT.EXIT;
            }
            else
            {
                tempoAUX = cooldownTiro;
                base.Update();
                return;
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
