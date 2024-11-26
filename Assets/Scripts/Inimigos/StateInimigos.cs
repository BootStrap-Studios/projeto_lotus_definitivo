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
    public enum STATE
    {
        IDLE, CHASE, ATIRAR, RELOAD, COVER,TOMARDANO
    }

    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    }

    public STATE stateName;
    public EVENT stage;
    protected GameObject inimigo;
    protected NavMeshAgent agent;
    protected Transform player;
    protected StateInimigos nextState;
    protected StateInimigos previousState;
    protected int municao;
    protected float alcanceMaxArma;
    protected float alcanceMinArma;
    protected float cooldownTiro;
    protected float danoRecebido;
    protected Animator anim;

    
    public float visDistancia = 25f;
    float visAngulo = 80f;
    public float cooldownTiroAux;
    public float tempoAux;
    public float municaoAux;
    public bool newReload = true;
    public bool ativarChase = false;
    public bool pararAndar = false;
    public int contagemTiros;
    public Cover coverSelecionado;

    public StateInimigos(GameObject _inimigo, NavMeshAgent _agent, Transform _player, int _municao, float _alcanceMaxArma, float _alcanceMinArma, float _cooldownTiro)
    {
        inimigo = _inimigo;
        agent = _agent;
        player = _player;
        municao = _municao;
        alcanceMaxArma = _alcanceMaxArma;
        alcanceMinArma = _alcanceMinArma;
        cooldownTiro = _cooldownTiro;
    }

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

    //função que verifica se o player está no campo de visão do inimigo
    public bool VejoPlayer()
    {      
        Vector3 direcao = player.position - inimigo.transform.position;
        float angulo = Vector3.Angle(direcao, inimigo.transform.forward);

        if (inimigo.GetComponent<Inimigo>().inimigoTorreta)
        {
            visDistancia = alcanceMaxArma;
        }

        RaycastHit hit;

        if (direcao.magnitude < visDistancia && angulo < visAngulo)
        {
            if(Physics.Linecast(inimigo.transform.position, player.position, out hit))
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
        if(inimigo.GetComponent<Inimigo>().spawnInimigos.covers.Length > 0)
        {
            for (int i = 0; i < inimigo.GetComponent<Inimigo>().spawnInimigos.covers.Length; i++)
            {
                if (!inimigo.GetComponent<Inimigo>().spawnInimigos.covers[i].coverCheio && inimigo.GetComponent<Inimigo>().spawnInimigos.covers[i].coverEscondido)
                {
                    Vector3 distCoverPlayer = player.transform.position - inimigo.GetComponent<Inimigo>().spawnInimigos.covers[i].transform.position;
                    Vector3 distCoverInimigo = inimigo.transform.position - inimigo.GetComponent<Inimigo>().spawnInimigos.covers[i].transform.position;

                    if (distCoverPlayer.magnitude <= alcanceMaxArma && distCoverInimigo.magnitude < 8f)
                    {
                        //Debug.Log("cover escolhido");
                        inimigo.GetComponent<Inimigo>().spawnInimigos.covers[i].inimigoAtual = inimigo.GetComponent<Inimigo>();
                        coverSelecionado = inimigo.GetComponent<Inimigo>().spawnInimigos.covers[i];
                        break;
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
}

public class Idle : StateInimigos
{
    public Idle(GameObject _inimigo, NavMeshAgent _agent, Transform _player, int _municao, float _alcanceMaxArma, float _alcanceMinArma, float _cooldownTiro) : base(_inimigo, _agent, _player, _municao, _alcanceMaxArma, _alcanceMinArma, _cooldownTiro)
    {
        stateName = STATE.IDLE;
        //inimigo.GetComponent<Inimigo>().AtualizaStatus("STATUS: Idle");
        agent.isStopped = true; 
    }
    
    public override void Enter()
    {
        //tocar animação idle    
        base.Enter();
    }

    public override void Update()
    {
        if(!inimigo.GetComponent<Inimigo>().inimigoTorreta) inimigo.GetComponent<Inimigo>().animator.SetFloat("velocidade", agent.desiredVelocity.sqrMagnitude); 

        if (ativarChase)
        {
            if (inimigo.GetComponent<Inimigo>().inimigoSniper || inimigo.GetComponent<Inimigo>().inimigoTorreta)
            {
                nextState = new Atirar(inimigo, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro);
                stage = EVENT.EXIT;
            }
            else
            {
                nextState = new Chase(inimigo, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro);
                stage = EVENT.EXIT;
            }

            return;
        }

        //verificando se player está em seu campo de visão
        if (VejoPlayer())
        {
            //SOM PERCEBE INIMIGO
            nextState = new Chase(inimigo, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        //reset da animação
        base.Exit();
    }
}

public class Chase : StateInimigos
{
    public Chase(GameObject _inimigo, NavMeshAgent _agent, Transform _player, int _municao, float _alcanceMaxArma, float _alcanceMinArma, float _cooldownTiro) : base(_inimigo, _agent, _player, _municao, _alcanceMaxArma, _alcanceMinArma, _cooldownTiro)
    {
        stateName = STATE.CHASE;
        agent.angularSpeed = 250f;
        agent.isStopped = false;
        tempoAux = 2f;
    }

    public override void Enter()
    {
        //tocar animação de persiguição
        //inimigo.GetComponent<Inimigo>().AtualizaStatus("STATUS: Persiguindo");

        base.Enter();
    }

    public override void Update()
    {
        agent.speed = inimigo.GetComponent<Inimigo>().velocidadeAndar;
        if (!inimigo.GetComponent<Inimigo>().inimigoTorreta) inimigo.GetComponent<Inimigo>().animator.SetFloat("velocidade", agent.desiredVelocity.sqrMagnitude);

        //persiguindo player caso ainda não esteja perto o suficiente pra atirar
        agent.SetDestination(player.position);

        if (inimigo.GetComponent<Inimigo>().inimigoExplosivo)
        {
            agent.acceleration = 80f;
        }

        if (VejoPlayer())
        {
            Vector3 distanciaTiro = player.position - inimigo.transform.position;

            if (distanciaTiro.magnitude <= alcanceMaxArma)
            {
                nextState = new Atirar(inimigo, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro);  
                stage = EVENT.EXIT;
            }
            else
            {
                base.Update();
            }
        }
        else
        {
            base.Update();
        }

    }

    public override void Exit()
    {
        //reset da animação
            
        base.Exit();
    }
}

public class Atirar : StateInimigos
{
    public Atirar(GameObject _inimigo, NavMeshAgent _agent, Transform _player, int _municao, float _alcanceMaxArma, float _alcanceMinArma, float _cooldownTiro) : base(_inimigo, _agent, _player, _municao, _alcanceMaxArma, _alcanceMinArma, _cooldownTiro)
    {
        stateName = STATE.ATIRAR;
        agent.angularSpeed = 250f;
        
        agent.isStopped = false;
        cooldownTiroAux = cooldownTiro;
    }

    public override void Enter()
    {
        //mudar animação
        base.Enter();
    }

    public override void Update()
    {
        agent.speed = inimigo.GetComponent<Inimigo>().velocidadeAndar;
        if (!inimigo.GetComponent<Inimigo>().inimigoTorreta) inimigo.GetComponent<Inimigo>().animator.SetFloat("velocidade", agent.desiredVelocity.sqrMagnitude);

        tempoAux -= Time.deltaTime;

        if(coverSelecionado != null)
        {
            if (inimigo.GetComponent<Inimigo>().inimigoSniper)
            {
                inimigo.GetComponent<Inimigo>().animator.SetBool("Mirando", false);
            }
            else if (inimigo.GetComponent<Inimigo>().inimigoNormal)
            {
                inimigo.GetComponent<Inimigo>().animator.SetBool("Atirando", false);
            }

            if (coverSelecionado.coverEscondido)
            {
                agent.SetDestination(coverSelecionado.transform.position);
                Vector3 distanciaCover = inimigo.transform.position - coverSelecionado.transform.position;

                if (distanciaCover.magnitude <= 1f)
                {
                    //Debug.Log(inimigo.name + "entrando no cover " + coverSelecionado.name + "a partir da chase");
                    nextState = new AtirandoCover(inimigo, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro);
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

                agent.SetDestination(player.transform.position);
            }

        }
        else
        {
            if (inimigo.GetComponent<Inimigo>().inimigoSniper)
            {
                inimigo.GetComponent<Inimigo>().animator.SetBool("Mirando", true);
            }
            else if (inimigo.GetComponent<Inimigo>().inimigoNormal)
            {
                inimigo.GetComponent<Inimigo>().animator.SetBool("Atirando", true);
            }

            if (newReload)
            {
                cooldownTiroAux = cooldownTiro + 1;
                municaoAux = municao;
                newReload = false;
            }

            if (inimigo.GetComponent<Inimigo>().inimigoSniper || inimigo.GetComponent<Inimigo>().inimigoTorreta)
            {
                pararAndar = true;
            }
            else if (inimigo.GetComponent<Inimigo>().inimigoExplosivo)
            {
                agent.acceleration = 30f;
            }

            if (pararAndar)
            {
                if (TemCover() && !inimigo.GetComponent<Inimigo>().inimigoExplosivo && !inimigo.GetComponent<Inimigo>().inimigoTorreta)
                {
                    agent.SetDestination(coverSelecionado.transform.position);
                    pararAndar = false;
                }
                else
                {
                    agent.SetDestination(inimigo.transform.position);

                    if (inimigo.GetComponent<Inimigo>().inimigoTorreta)
                    {
                        Vector3 lookPos = player.transform.position - inimigo.transform.position;
                        float angulo = Vector3.Angle(lookPos, inimigo.transform.forward);
                        lookPos.y = lookPos.y - 2;
                        Quaternion rotation = Quaternion.LookRotation(lookPos);

                        if (angulo > 22)
                        {
                            inimigo.GetComponent<Inimigo>().objInimigo.transform.rotation = Quaternion.Slerp(inimigo.transform.rotation, rotation, .8f);
                        }
                        else
                        {
                            inimigo.GetComponent<Inimigo>().objInimigo.transform.rotation = Quaternion.Slerp(inimigo.transform.rotation, rotation, .2f);
                        }
                    }
                    else if (VejoPlayer())
                    {
                        Vector3 lookPos = player.transform.position - inimigo.transform.position;
                        float angulo = Vector3.Angle(lookPos, inimigo.transform.forward);
                        lookPos.y = 0;
                        Quaternion rotation = Quaternion.LookRotation(lookPos);

                        if (angulo > 22)
                        {
                            inimigo.transform.rotation = Quaternion.Slerp(inimigo.transform.rotation, rotation, .8f);
                        }
                        else
                        {
                            inimigo.transform.rotation = Quaternion.Slerp(inimigo.transform.rotation, rotation, .2f);
                        }

                    }
                }
            }
            else if (VejoPlayer())
            {
                Vector3 distanciaTiro = player.position - inimigo.transform.position;

                Vector3 lookPos = player.transform.position - inimigo.transform.position;
                float angulo = Vector3.Angle(lookPos, inimigo.transform.forward);
                lookPos.y = 0;
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                if (angulo > 22)
                {
                    inimigo.transform.rotation = Quaternion.Slerp(inimigo.transform.rotation, rotation, .8f);
                }
                else
                {
                    inimigo.transform.rotation = Quaternion.Slerp(inimigo.transform.rotation, rotation, .2f);
                }

                //SOM TORRETA SE MEXENDO

                if (distanciaTiro.magnitude < alcanceMinArma)
                {
                    agent.SetDestination(inimigo.transform.position);

                }
                else if (distanciaTiro.magnitude <= alcanceMaxArma && distanciaTiro.magnitude >= alcanceMinArma)
                {
                    if (!inimigo.GetComponent<Inimigo>().inimigoExplosivo && !inimigo.GetComponent<Inimigo>().inimigoTorreta)
                    {
                        if (TemCover())
                        {
                            agent.SetDestination(coverSelecionado.transform.position);
                        }
                        else
                        {
                            agent.SetDestination(player.position);
                        }
                    }
                    else
                    {
                        agent.SetDestination(player.position);
                    }
                }
                else
                {
                    nextState = new Chase(inimigo, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro);
                    stage = EVENT.EXIT;
                }
            }
            else
            {
                nextState = new Chase(inimigo, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro);
                stage = EVENT.EXIT;
            }

            //verificando cooldown do tiro para disparar
            cooldownTiroAux -= Time.deltaTime;

            if (cooldownTiroAux <= 0)
            {
                if (municaoAux > 0 || inimigo.GetComponent<Inimigo>().inimigoTorreta)
                {
                    
                    if (inimigo.GetComponent<Inimigo>().Atirar(false))
                    {
                        if (inimigo.GetComponent<Inimigo>().inimigoSniper)
                        {
                            inimigo.GetComponent<Inimigo>().animator.SetTrigger("Atirar");
                        }

                        if (inimigo.GetComponent<Inimigo>().inimigoTorreta)
                        {
                            contagemTiros++;
                            cooldownTiroAux = 0.3f;

                            if (contagemTiros >= 3)
                            {
                                cooldownTiroAux = cooldownTiro;
                                contagemTiros = 0;
                            }
                        }
                        else if (inimigo.GetComponent<Inimigo>().inimigoExplosivo)
                        {
                            municaoAux--;
                            pararAndar = true;
                            cooldownTiroAux = 1000;
                        }
                        else
                        {
                            municaoAux--;
                            cooldownTiroAux = cooldownTiro;

                           
                        }
                    }
                    else if (!pararAndar)
                    {
                        agent.SetDestination(player.position);
                        base.Update();
                    }
                }
                else
                {
                    //if (TemCover())
                    //{
                    //    previousState = new Reload(inimigo, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro);
                    //    agent.SetDestination(coverSelecionado.transform.position);
                    //}
                    //else
                    //{
                    nextState = new Reload(inimigo, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro);
                    stage = EVENT.EXIT;
                    //}

                    return;
                }
            }
            else
            {
                base.Update();
            }
        }
    }

    public override void Exit()
    {
        //reset das animações

        if(nextState == new Reload(inimigo, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro))
        {
            if (inimigo.GetComponent<Inimigo>().inimigoSniper)
            {
                inimigo.GetComponent<Inimigo>().animator.SetBool("Mirando", false);
            }
            else if (inimigo.GetComponent<Inimigo>().inimigoNormal)
            {
                inimigo.GetComponent<Inimigo>().animator.SetBool("Atirando", false);
            }
        }      
        base.Exit();
    } 
}

public class Reload : StateInimigos
{
    public Reload(GameObject _inimigo, NavMeshAgent _agent, Transform _player, int _municao, float _alcanceMaxArma, float _alcanceMinArma, float _cooldownTiro) : base(_inimigo, _agent, _player, _municao, _alcanceMaxArma, _alcanceMinArma, _cooldownTiro)
    {
        stateName = STATE.RELOAD;
        agent.speed = 0f;
        tempoAux = 2f;
    }

    public override void Enter()
    {
        //tocar a animação de reloading
        //inimigo.GetComponent<Inimigo>().AtualizaStatus("STATUS: Recarregando");

        inimigo.GetComponent<Inimigo>().animator.SetFloat("velocidade", agent.desiredVelocity.sqrMagnitude);
        inimigo.GetComponent<Inimigo>().animator.SetTrigger("Reload");

        base.Enter();
    }

    public override void Update()
    {
        inimigo.transform.rotation = inimigo.transform.rotation;

        tempoAux -= Time.deltaTime;

        if (tempoAux <= 0)
        {
            if(coverSelecionado != null)
            {
                nextState = new AtirandoCover(inimigo, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro);
                stage = EVENT.EXIT;
                newReload = true;
            }
            else
            {
                nextState = new Atirar(inimigo, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro);
                stage = EVENT.EXIT;
                newReload = true;

                inimigo.GetComponent<Inimigo>().animator.SetBool("Cover", false);
            }
        }
        else
        {
            base.Update();
        }
    }

    public override void Exit()
    {
        //reset da animação
        base.Exit();
    }
}

public class AtirandoCover : StateInimigos
{
    public AtirandoCover(GameObject _inimigo, NavMeshAgent _agent, Transform _player, int _municao, float _alcanceMaxArma, float _alcanceMinArma, float _cooldownTiro) : base(_inimigo, _agent, _player, _municao, _alcanceMaxArma, _alcanceMinArma, _cooldownTiro)
    {
        stateName = STATE.COVER;
    }

    public override void Enter()
    {
        base.Enter();

        tempoAux = 0f;

        inimigo.GetComponent<Inimigo>().animator.SetBool("Cover", true);
    }

    public override void Update()
    {
        agent.speed = inimigo.GetComponent<Inimigo>().velocidadeAndar;
        inimigo.GetComponent<Inimigo>().animator.SetFloat("velocidade", agent.desiredVelocity.sqrMagnitude);

        Vector3 lookPos = player.transform.position - inimigo.transform.position;
        float angulo = Vector3.Angle(lookPos, inimigo.transform.forward);
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        if (angulo > 22)
        {
            inimigo.transform.rotation = Quaternion.Slerp(inimigo.transform.rotation, rotation, .8f);
        }
        else
        {
            inimigo.transform.rotation = Quaternion.Slerp(inimigo.transform.rotation, rotation, .2f);
        }

        if (coverSelecionado == null)
        {
            nextState = new Atirar(inimigo, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro);
            stage = EVENT.EXIT;

            inimigo.GetComponent<Inimigo>().animator.SetBool("Cover", false);

            if (inimigo.GetComponent<Inimigo>().inimigoNormal)
            {
                inimigo.GetComponent<Inimigo>().animator.SetBool("AtirandoCover", false);
            }
        }
        else
        {
            agent.SetDestination(coverSelecionado.transform.position);


            if (newReload)
            {
                if (inimigo.GetComponent<Inimigo>().inimigoNormal)
                {
                    inimigo.GetComponent<Inimigo>().animator.SetTrigger("EspiandoCover");
                    inimigo.GetComponent<Inimigo>().animator.SetBool("AtirandoCover", true);
                    cooldownTiroAux = cooldownTiro + 3.2f;

                }
                else
                {
                    cooldownTiroAux = cooldownTiro + 1;
                }
                
                newReload = false;
                municaoAux = municao;
            }

            cooldownTiroAux -= Time.deltaTime;

            if (cooldownTiroAux <= 0)
            {
                if (municaoAux > 0)
                {
                    if (inimigo.GetComponent<Inimigo>().Atirar(true))
                    {
                        if (inimigo.GetComponent<Inimigo>().inimigoSniper)
                        {
                            inimigo.GetComponent<Inimigo>().animator.SetTrigger("Atirar");
                        }

                        municaoAux--;
                        cooldownTiroAux = cooldownTiro;
                        tempoAux = 0;
                    }
                    else
                    {
                        tempoAux += Time.deltaTime;
                        if (tempoAux >= 10)
                        {
                            nextState = new Chase(inimigo, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro);
                            stage = EVENT.EXIT;

                            inimigo.GetComponent<Inimigo>().animator.SetBool("Cover", false);

                            if (inimigo.GetComponent<Inimigo>().inimigoNormal)
                            {
                                inimigo.GetComponent<Inimigo>().animator.SetBool("AtirandoCover", false);
                            }

                            coverSelecionado.inimigoAtual = null;
                            coverSelecionado = null;
                        }
                    }
                }
                else
                {
                    nextState = new Reload(inimigo, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro);
                    stage = EVENT.EXIT;

                    if (inimigo.GetComponent<Inimigo>().inimigoNormal)
                    {
                        inimigo.GetComponent<Inimigo>().animator.SetBool("AtirandoCover", false);
                    }
                }
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class TomarDano : StateInimigos
{
    public TomarDano(GameObject _inimigo, NavMeshAgent _agent, Transform _player, int _municao, float _alcanceMaxArma, float _alcanceMinArma, float _cooldownTiro) : base(_inimigo, _agent, _player, _municao, _alcanceMaxArma, _alcanceMinArma, _cooldownTiro)
    {
        stateName = STATE.TOMARDANO;
        //agent.speed = 0f;
        tempoAux = 0.5f;
    }

    public override void Enter()
    {
        //tocar a animação de levar dano
        //inimigo.GetComponent<Inimigo>().AtualizaStatus("STATUS: TomandoDano");
        base.Enter();
    }

    public override void Update()
    {
        tempoAux -= Time.deltaTime;

        if(tempoAux <= 0)
        {
            nextState = new Chase(inimigo, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro);
            stage = EVENT.EXIT;
        }
        else
        {
            base.Update();
        }

    }

    public override void Exit()
    {
        //reset da animação
        base.Exit();
    }
}



