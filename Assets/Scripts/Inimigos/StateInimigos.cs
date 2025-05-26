using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static StateInimigos;

public class StateInimigos
{
    #region Construtor dos Inimigos

    public enum STATE
    {
        DESATIVADO, ATIVADO, ATIRAR, RELOAD, COVER
    }

    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    }

    public STATE stateName;
    public EVENT stage;
    protected GameObject inimigoObj;
    protected Inimigo inimigoScript;
    protected NavMeshAgent agent;
    protected Transform player;
    protected StateInimigos nextState;
    protected int municao;
    protected float alcanceMaxArma;
    protected float alcanceMinArma;
    protected float cooldownTiro;
    protected Animator anim;

    public StateInimigos(GameObject _inimigoObj, Inimigo _inimigoScript, NavMeshAgent _agent, Transform _player, int _municao, float _alcanceMaxArma, float _alcanceMinArma, float _cooldownTiro, Animator _anim)
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

    public float tempoAux = 0;
    public int municaoAux = 0;
    public bool ativarRobo = false;
    public Cover coverSelecionado;

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
    public StateInimigos Process()
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

        if (direcao.magnitude < alcanceMaxArma + 10 && angulo < 80)
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
        else if (direcao.magnitude < 4f)
        {
            return true;
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
                        if (coverAtual != null)
                        {
                            if (distCoverAtualInimigo.magnitude > distCoverInimigo.magnitude)
                            {
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
                if(coverSelecionado.inimigoAtual == inimigoScript)
                {
                    return true;
                }
                else
                {
                    coverSelecionado = null;
                    return false;
                } 
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

public class SimplesDesativado : StateInimigos
{
    public SimplesDesativado(GameObject _inimigoObj, Inimigo _inimigoScript, NavMeshAgent _agent, Transform _player, int _municao, float _alcanceMaxArma, float _alcanceMinArma, float _cooldownTiro, Animator _anim) : base(_inimigoObj, _inimigoScript, _agent, _player, _municao, _alcanceMaxArma, _alcanceMinArma, _cooldownTiro, _anim)
    {
        stateName = STATE.DESATIVADO;
        //inimigo.GetComponent<Inimigo>().AtualizaStatus("STATUS: Idle");
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
            nextState = new SimplesAtivado(inimigoObj, inimigoScript, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro, anim);
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

public class SimplesAtivado : StateInimigos
{
    public SimplesAtivado(GameObject _inimigoObj, Inimigo _inimigoScript, NavMeshAgent _agent, Transform _player, int _municao, float _alcanceMaxArma, float _alcanceMinArma, float _cooldownTiro, Animator _anim) : base(_inimigoObj, _inimigoScript, _agent, _player, _municao, _alcanceMaxArma, _alcanceMinArma, _cooldownTiro, _anim)
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
        agent.speed = inimigoScript.velocidadeAndar;
        anim.SetFloat("Velocidade", agent.desiredVelocity.sqrMagnitude);
        Vector3 distanciaTiro = player.position - inimigoObj.transform.position;

        if (MiraPlayer() && coverSelecionado == null)
        {
            agent.SetDestination(player.position);
            RotacionaRobo();

            if (distanciaTiro.magnitude <= alcanceMaxArma)
            {
                nextState = new SimplesAtirar(inimigoObj, inimigoScript, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro, anim);  
                stage = EVENT.EXIT;
            }
            else
            {
                base.Update();
                return;
            }
        }
        else if (distanciaTiro.magnitude <= alcanceMinArma)
        {
            agent.SetDestination(inimigoObj.transform.position);
            RotacionaRobo();

            nextState = new SimplesAtirar(inimigoObj, inimigoScript, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro, anim);
            stage = EVENT.EXIT;
        }
        else
        {
            if (coverSelecionado != null)
            {
                agent.SetDestination(coverSelecionado.transform.position);
                coverSelecionado.VerificaCover2();

                if (coverSelecionado.coverEscondido && coverSelecionado.inimigoAtual == inimigoScript)
                {
                    Vector3 distanciaCover = inimigoObj.transform.position - coverSelecionado.transform.position;

                    if (distanciaCover.magnitude <= 1f)
                    {
                        nextState = new SimplesCover(inimigoObj, inimigoScript, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro, anim);
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
            else if (TemCover())
            {
                agent.SetDestination(coverSelecionado.transform.position);
                base.Update();
                return;
            }
            else
            {
                //PATRULHA??
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

public class SimplesAtirar : StateInimigos
{
    public SimplesAtirar(GameObject _inimigoObj, Inimigo _inimigoScript, NavMeshAgent _agent, Transform _player, int _municao, float _alcanceMaxArma, float _alcanceMinArma, float _cooldownTiro, Animator _anim) : base(_inimigoObj, _inimigoScript, _agent, _player, _municao, _alcanceMaxArma, _alcanceMinArma, _cooldownTiro, _anim)
    {
        stateName = STATE.ATIRAR;
        agent.isStopped = false;
        agent.speed = inimigoScript.velocidadeAndar;
        tempoAux = cooldownTiro;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        anim.SetFloat("Velocidade", agent.desiredVelocity.sqrMagnitude);

        if (MiraPlayer())
        {
            Vector3 distanciaTiro = player.position - inimigoObj.transform.position;
            RotacionaRobo();

            if (distanciaTiro.magnitude <= alcanceMaxArma)
            {
                anim.SetBool("Atirando", true);

                agent.speed = inimigoScript.velocidadeAndar - (inimigoScript.velocidadeAndar/5);

                if (distanciaTiro.magnitude < alcanceMinArma)
                {
                    agent.SetDestination(inimigoObj.transform.position);
                }
                else
                {
                    agent.SetDestination(player.position);
                }

                //verificando cooldown do tiro para disparar
                tempoAux -= Time.deltaTime;

                if (tempoAux <= 0)
                {
                    if (inimigoScript.Atirar(false))
                    {
                        municaoAux++;
                        tempoAux = cooldownTiro;
                        if (municaoAux >= municao)
                        {
                            nextState = new SimplesReload(inimigoObj, inimigoScript, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro, anim);
                            stage = EVENT.EXIT;
                        }
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
                anim.SetBool("Atirando", false);
                tempoAux = cooldownTiro;
                agent.SetDestination(player.position);
                agent.speed = inimigoScript.velocidadeAndar;
            }
        }
        else
        {
            nextState = new SimplesAtivado(inimigoObj, inimigoScript, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro, anim);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class SimplesReload : StateInimigos
{
    public SimplesReload(GameObject _inimigoObj, Inimigo _inimigoScript, NavMeshAgent _agent, Transform _player, int _municao, float _alcanceMaxArma, float _alcanceMinArma, float _cooldownTiro, Animator _anim) : base(_inimigoObj, _inimigoScript, _agent, _player, _municao, _alcanceMaxArma, _alcanceMinArma, _cooldownTiro, _anim)
    {
        stateName = STATE.RELOAD;
        tempoAux = 2.5f;
    }

    public override void Enter()
    {
        agent.SetDestination(inimigoObj.transform.position);
        anim.SetFloat("Velocidade", agent.desiredVelocity.sqrMagnitude);
        anim.SetTrigger("Reload");
        base.Enter();
    }

    public override void Update()
    {
        tempoAux -= Time.deltaTime;

        if (tempoAux <= 0)
        {
            if(coverSelecionado != null)
            {
                nextState = new SimplesCover(inimigoObj, inimigoScript, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro, anim);
                stage = EVENT.EXIT;
            }
            else
            {
                nextState = new SimplesAtirar(inimigoObj, inimigoScript, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro, anim);
                stage = EVENT.EXIT;
            }
        }
        else
        {
            base.Update();
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class SimplesCover : StateInimigos
{
    public SimplesCover(GameObject _inimigoObj, Inimigo _inimigoScript, NavMeshAgent _agent, Transform _player, int _municao, float _alcanceMaxArma, float _alcanceMinArma, float _cooldownTiro, Animator _anim) : base(_inimigoObj, _inimigoScript, _agent, _player, _municao, _alcanceMaxArma, _alcanceMinArma, _cooldownTiro, _anim)
    {
        stateName = STATE.COVER;
        tempoAux = cooldownTiro + 3.5f;
    }

    public override void Enter()
    {     
        anim.SetBool("Cover", true);
        anim.SetTrigger("EspiandoCover");
        anim.SetBool("AtirandoCover", true);
        coverSelecionado.VerificaCover2();
        RotacionaRobo();

        base.Enter();
    }

    public override void Update()
    {
        anim.SetFloat("Velocidade", agent.desiredVelocity.sqrMagnitude);

        if (coverSelecionado != null)
        {
            tempoAux -= Time.deltaTime;

            if (MiraPlayer())
            {
                coverSelecionado.VerificaCover2();
                RotacionaRobo();

                if (coverSelecionado.coverEscondido)
                {
                    if (tempoAux <= 0)
                    {
                        if (inimigoScript.Atirar(false))
                        {
                            municaoAux++;
                            tempoAux = cooldownTiro;

                            if (municaoAux >= municao)
                            {
                                nextState = new SimplesReload(inimigoObj, inimigoScript, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro, anim);
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

                    nextState = new SimplesAtivado(inimigoObj, inimigoScript, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro, anim);
                    stage = EVENT.EXIT;
                }
            }
            else if (tempoAux <= -12f)
            {
                coverSelecionado.inimigoAtual = null;
                coverSelecionado = null;

                anim.SetBool("Cover", false);

                nextState = new SimplesAtivado(inimigoObj, inimigoScript, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro, anim);
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

            nextState = new SimplesAtivado(inimigoObj, inimigoScript, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro, anim);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        anim.SetBool("AtirandoCover", false);
        base.Exit();
    }
}