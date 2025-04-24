using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static StateInimigoSniper;

public class StateInimigoSniper
{
    #region Construtor dos Inimigos

    public enum STATE
    {
        DESATIVADO, ATIVADO, ATIRANDO, RELOAD, COVER
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
    protected StateInimigoSniper nextState;
    protected int municao;
    protected float alcanceMaxArma;
    protected float alcanceMinArma;
    protected float cooldownTiro;
    protected Animator anim;
    protected Inimigo inimigoScript;

    public StateInimigoSniper(GameObject _inimigoObj, Inimigo _inimigoScript, NavMeshAgent _agent, Transform _player, int _municao, float _alcanceMaxArma, float _alcanceMinArma, float _cooldownTiro, Animator _anim)
    {
        inimigoObj = _inimigoObj;
        inimigoScript = _inimigoScript;
        agent = _agent;
        player = _player;
        municao = _municao;
        alcanceMaxArma = _alcanceMaxArma;
        alcanceMinArma = _alcanceMinArma;
        cooldownTiro = _cooldownTiro;
        anim = _anim;
    }

    #endregion

    public Cover coverSelecionado = null;
    public bool ativarRobo = false;
    public float tempoAUX = 0f;
    public int tirosDisparados = 0;

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
    public StateInimigoSniper Process()
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
        Vector3 direcao = player.position - inimigoScript.olhosRobo.transform.position;
        float angulo = Vector3.Angle(direcao, inimigoScript.olhosRobo.transform.forward);

        RaycastHit hit;

        if (direcao.magnitude < alcanceMaxArma && angulo < 80)
        {
            if (Physics.Linecast(inimigoScript.olhosRobo.transform.position, player.position, out hit))
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

    public bool TemCover()
    {
        Cover coverAtual = null;
        Vector3 distCoverAtualInimigo = Vector3.zero;

        if (inimigoScript.spawnInimigos.covers.Length > 0)
        {
            for (int i = 0; i < inimigoScript.spawnInimigos.covers.Length; i++)
            {
                if (!inimigoScript.spawnInimigos.covers[i].coverCheio && inimigoScript.spawnInimigos.covers[i].coverEscondido)
                {
                    Vector3 distCoverPlayer = player.transform.position - inimigoScript.spawnInimigos.covers[i].transform.position;
                    Vector3 distCoverInimigo = inimigoScript.transform.position - inimigoScript.spawnInimigos.covers[i].transform.position;

                    if (distCoverPlayer.magnitude <= alcanceMaxArma && distCoverInimigo.magnitude < alcanceMaxArma)
                    {
                        //Debug.Log("Distância Cover1: " + distCoverAtualInimigo.magnitude);
                        //Debug.Log("Distância Cover2: " + distCoverInimigo.magnitude);

                        if (coverAtual != null)
                        {
                            if(distCoverAtualInimigo.magnitude > distCoverInimigo.magnitude)
                            {
                                //Debug.Log("CoverAlterado");

                                inimigoScript.spawnInimigos.covers[i].inimigoAtual = inimigoScript;
                                coverSelecionado = inimigoScript.spawnInimigos.covers[i];

                                coverAtual = coverSelecionado;
                                distCoverAtualInimigo = distCoverInimigo;
                            }
                        }
                        else
                        {
                            inimigoScript.spawnInimigos.covers[i].inimigoAtual = inimigoScript;
                            coverSelecionado = inimigoScript.spawnInimigos.covers[i];

                            coverAtual = coverSelecionado;
                            distCoverAtualInimigo = distCoverInimigo;
                        }
                    }
                }
            }
            if (coverSelecionado == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
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

public class SniperDesativado : StateInimigoSniper
{
    public SniperDesativado(GameObject _inimigoObj, Inimigo _inimigoScript, NavMeshAgent _agent, Transform _player, int _municao, float _alcanceMaxArma, float _alcanceMinArma, float _cooldownTiro, Animator _anim) : base(_inimigoObj, _inimigoScript, _agent, _player, _municao, _alcanceMaxArma, _alcanceMinArma, _cooldownTiro, _anim)
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
            nextState = new SniperAtivado(inimigoObj, inimigoScript, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro, anim);
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

public class SniperAtivado : StateInimigoSniper
{
    public SniperAtivado(GameObject _inimigoObj, Inimigo _inimigoScript, NavMeshAgent _agent, Transform _player, int _municao, float _alcanceMaxArma, float _alcanceMinArma, float _cooldownTiro, Animator _anim) : base(_inimigoObj, _inimigoScript, _agent, _player, _municao, _alcanceMaxArma, _alcanceMinArma, _cooldownTiro, _anim)
    {
        stateName = STATE.ATIVADO;
        //inimigo.GetComponent<Inimigo>().AtualizaStatus("STATUS: Idle");
        agent.isStopped = false;
    }

    public override void Enter()
    {
        //tocar animação idle    
        base.Enter();
    }

    public override void Update()
    {
        anim.SetFloat("Velocidade", agent.desiredVelocity.sqrMagnitude);
        Vector3 distanciaPlayer = inimigoObj.transform.position - player.transform.position;

        if (coverSelecionado == null)
        {
            //verificando se tem cover
            if (TemCover() && distanciaPlayer.magnitude > alcanceMinArma)
            {
                agent.SetDestination(coverSelecionado.transform.position);
                base.Update();
                return;
            }
            else
            {
                //verificando se player está em seu campo de visão
                if (MiraPlayer())
                {
                    //SOM DE DETECTAR O PLAYER
                    nextState = new SniperAtirando(inimigoObj, inimigoScript, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro, anim);
                    stage = EVENT.EXIT;
                }
                else
                {
                    //PATRULHAR ÁREA??
                    base.Update();
                    return;
                }
            }
        }
        else
        {
            agent.SetDestination(coverSelecionado.transform.position);
            coverSelecionado.VerificaCover2();

            if (coverSelecionado.coverEscondido)
            {
                Vector3 distanciaCover = inimigoObj.transform.position - coverSelecionado.transform.position;

                if (distanciaCover.magnitude <= 1f)
                {
                    nextState = new SniperCover(inimigoObj, inimigoScript, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro, anim);
                    stage = EVENT.EXIT;
                }
                else
                {
                    base.Update();
                    return;
                }
            }
            else
            {
                coverSelecionado.inimigoAtual = null;
                coverSelecionado = null;

                agent.SetDestination(inimigoObj.transform.position);

                base.Update();
                return;
            }
        }
    }

    public override void Exit()
    {
        //reset da animação
        base.Exit();
    }
}

public class SniperAtirando : StateInimigoSniper
{
    public SniperAtirando(GameObject _inimigoObj, Inimigo _inimigoScript, NavMeshAgent _agent, Transform _player, int _municao, float _alcanceMaxArma, float _alcanceMinArma, float _cooldownTiro, Animator _anim) : base(_inimigoObj, _inimigoScript, _agent, _player, _municao, _alcanceMaxArma, _alcanceMinArma, _cooldownTiro, _anim)
    {
        stateName = STATE.ATIRANDO;
        agent.isStopped = false; 
    }

    public override void Enter()
    {
        anim.SetBool("Mirando", true);
        tempoAUX = cooldownTiro;
        RotacionaRobo();
        base.Enter();
    }

    public override void Update()
    {
        anim.SetFloat("Velocidade", agent.desiredVelocity.sqrMagnitude);

        tempoAUX -= Time.deltaTime;

        if (MiraPlayer())
        {
            RotacionaRobo();

            if (tempoAUX <= 0)
            {
                if (inimigoScript.Atirar(false))
                {
                    anim.SetTrigger("Atirar");
                    tirosDisparados++;
                    tempoAUX = cooldownTiro;

                    if (tirosDisparados >= municao)
                    {
                        nextState = new SniperReload(inimigoObj, inimigoScript, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro, anim);
                        stage = EVENT.EXIT;
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
                base.Update();
                return;
            }
        }
        else if (tempoAUX <= -12f)
        {
            anim.SetBool("Mirando", false);

            Debug.Log("Não vejo o player");
            nextState = new SniperAtivado(inimigoObj, inimigoScript, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro, anim);
            stage = EVENT.EXIT;
        }
        else
        {
            if (TemCover())
            {
                anim.SetBool("Mirando", false);

                Debug.Log("Não vejo o player, mas achei um cover");
                agent.SetDestination(coverSelecionado.transform.position);
                nextState = new SniperAtivado(inimigoObj, inimigoScript, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro, anim);
                stage = EVENT.EXIT;
            }
            else
            {
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

public class SniperCover : StateInimigoSniper
{
    public SniperCover(GameObject _inimigoObj, Inimigo _inimigoScript, NavMeshAgent _agent, Transform _player, int _municao, float _alcanceMaxArma, float _alcanceMinArma, float _cooldownTiro, Animator _anim) : base(_inimigoObj, _inimigoScript, _agent, _player, _municao, _alcanceMaxArma, _alcanceMinArma, _cooldownTiro, _anim)
    {
        stateName = STATE.COVER;       
        agent.isStopped = true;    
    }

    public override void Enter()
    {
        anim.SetBool("Cover", true);
        tempoAUX = 3f;
        coverSelecionado.VerificaCover2();
        RotacionaRobo();
        base.Enter();
    }

    public override void Update()
    {
        anim.SetFloat("Velocidade", agent.desiredVelocity.sqrMagnitude);

        if (coverSelecionado != null)
        {
            tempoAUX -= Time.deltaTime;

            if (MiraPlayer())
            {
                coverSelecionado.VerificaCover2();
                RotacionaRobo();

                if (coverSelecionado.coverEscondido)
                {                    
                    if (tempoAUX <= 0)
                    {
                        if (inimigoScript.Atirar(true))
                        {
                            anim.SetTrigger("Atirar");
                            tirosDisparados++;
                            tempoAUX = cooldownTiro;

                            if (tirosDisparados >= municao)
                            {
                                nextState = new SniperReload(inimigoObj, inimigoScript, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro, anim);
                                stage = EVENT.EXIT;
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
                        base.Update();
                        return;
                    }
                }
                else
                {
                    coverSelecionado.inimigoAtual = null;
                    coverSelecionado = null;

                    anim.SetBool("Cover", false);

                    nextState = new SniperAtivado(inimigoObj, inimigoScript, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro, anim);
                    stage = EVENT.EXIT;
                }
            }
            else if (tempoAUX <= -12f)
            {
                coverSelecionado.inimigoAtual = null;
                coverSelecionado = null;

                anim.SetBool("Cover", false);

                nextState = new SniperAtivado(inimigoObj, inimigoScript, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro, anim);
                stage = EVENT.EXIT;
            }
            else
            {
                base.Update();
                return;
            }
        }
        else
        {
            anim.SetBool("Cover", false);

            nextState = new SniperAtivado(inimigoObj, inimigoScript, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro, anim);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class SniperReload : StateInimigoSniper
{
    public SniperReload(GameObject _inimigoObj, Inimigo _inimigoScript, NavMeshAgent _agent, Transform _player, int _municao, float _alcanceMaxArma, float _alcanceMinArma, float _cooldownTiro, Animator _anim) : base(_inimigoObj, _inimigoScript, _agent, _player, _municao, _alcanceMaxArma, _alcanceMinArma, _cooldownTiro, _anim)
    {
        stateName = STATE.RELOAD;
        agent.isStopped = true;       
    }

    public override void Enter()
    {
        anim.SetTrigger("Reload");
        tempoAUX = 3f;
        base.Enter();
    }

    public override void Update()
    {
        anim.SetFloat("Velocidade", agent.desiredVelocity.sqrMagnitude);

        tempoAUX -= Time.deltaTime;

        if (tempoAUX < 0)
        {
            if (coverSelecionado != null)
            {
                nextState = new SniperCover(inimigoObj, inimigoScript, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro, anim);
                stage = EVENT.EXIT;
            }
            else
            {
                nextState = new SniperAtirando(inimigoObj, inimigoScript, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro, anim);
                stage = EVENT.EXIT;
            }
        }
    }

    public override void Exit()
    {
        //reset da animação
        base.Exit();
    }
}
