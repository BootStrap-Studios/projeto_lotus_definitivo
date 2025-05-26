using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static StateInimigoExplosivo;

public class StateInimigoExplosivo
{
    #region Construtor dos Inimigos

    public enum STATE
    {
        DESATIVADO, ATIVADO, PERSEGUINDO
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
    protected StateInimigoExplosivo nextState;
    protected float alcanceMaxArma;
    protected float alcanceMinArma;
    protected float cooldownTiro;
    protected Animator anim;
    protected Inimigo inimigoScript;

    public StateInimigoExplosivo(GameObject _inimigoObj, Inimigo _inimigoScript, NavMeshAgent _agent, Transform _player, float _alcanceMaxArma, float _alcanceMinArma, float _cooldownTiro, Animator _anim)
    {
        inimigoObj = _inimigoObj;
        inimigoScript = _inimigoScript;
        agent = _agent;
        player = _player;
        alcanceMaxArma = _alcanceMaxArma;
        alcanceMinArma = _alcanceMinArma;
        cooldownTiro = _cooldownTiro;
        anim = _anim;
    }

    #endregion

    public bool ativarRobo = false;
    public float tempoAUX = 0f;

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
    public StateInimigoExplosivo Process()
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
        Vector3 direcao = player.position - inimigoObj.transform.position;
        float angulo = Vector3.Angle(direcao, inimigoObj.transform.forward);

        RaycastHit hit;

        if (direcao.magnitude < 45 && angulo < 80)
        {
            if (Physics.Linecast(inimigoObj.transform.position, new Vector3(player.position.x, inimigoObj.transform.position.y, player.position.z), out hit))
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
        lookPos.y = lookPos.y - 2;
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

public class ExplosivoDesativado : StateInimigoExplosivo
{
    public ExplosivoDesativado(GameObject _inimigoObj, Inimigo _inimigoScript, NavMeshAgent _agent, Transform _player, float _alcanceMaxArma, float _alcanceMinArma, float _cooldownTiro, Animator _anim) : base(_inimigoObj, _inimigoScript, _agent, _player, _alcanceMaxArma, _alcanceMinArma, _cooldownTiro, _anim)
    {
        stateName = STATE.DESATIVADO;
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
            nextState = new ExplosivoAtivado(inimigoObj, inimigoScript, agent, player, alcanceMaxArma, alcanceMinArma, cooldownTiro, anim);
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

public class ExplosivoAtivado : StateInimigoExplosivo
{
    public ExplosivoAtivado(GameObject _inimigoObj, Inimigo _inimigoScript, NavMeshAgent _agent, Transform _player, float _alcanceMaxArma, float _alcanceMinArma, float _cooldownTiro, Animator _anim) : base(_inimigoObj, _inimigoScript, _agent, _player, _alcanceMaxArma, _alcanceMinArma, _cooldownTiro, _anim)
    {
        stateName = STATE.ATIVADO;
        agent.isStopped = false;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        if (MiraPlayer())
        {
            RotacionaRobo();
            nextState = new ExplosivoExplodindo(inimigoObj, inimigoScript, agent, player, alcanceMaxArma, alcanceMinArma, cooldownTiro, anim);
            stage = EVENT.EXIT;
        }
        else
        {
            //PATRULHA??
            base.Update();
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class ExplosivoExplodindo : StateInimigoExplosivo
{
    public ExplosivoExplodindo(GameObject _inimigoObj, Inimigo _inimigoScript, NavMeshAgent _agent, Transform _player, float _alcanceMaxArma, float _alcanceMinArma, float _cooldownTiro, Animator _anim) : base(_inimigoObj, _inimigoScript, _agent, _player, _alcanceMaxArma, _alcanceMinArma, _cooldownTiro, _anim)
    {
        stateName = STATE.PERSEGUINDO;
        agent.isStopped = false;
        tempoAUX = cooldownTiro;
    }

    public override void Enter()
    {
        anim.SetBool("Andar", true);
        base.Enter();
    }

    public override void Update()
    {
        Vector3 distanciaPlayer = player.transform.position - inimigoObj.transform.position;

        if (tempoAUX <= 0)
        {
            anim.SetBool("Andar", false);
            agent.isStopped = true;
            inimigoScript.Explodir();
            Debug.Log("Acabou o tempo, EXPLODINDO!!!");
        }
        else if(distanciaPlayer.magnitude <= alcanceMinArma)
        {
            anim.SetBool("Andar", false);
            agent.isStopped = true;
            inimigoScript.Explodir();
            Debug.Log("Te peguei, EXPLODINDO!!!");
        }
        else
        {
            tempoAUX -= Time.deltaTime;
            agent.SetDestination(player.transform.position);
            RotacionaRobo();
            base.Update();
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

