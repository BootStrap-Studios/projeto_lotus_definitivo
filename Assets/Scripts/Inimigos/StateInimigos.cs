using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static StateInimigos;

public class StateInimigos
{
    public enum STATE
    {
        IDLE, CHASE, ATIRAR, RELOAD, TOMARDANO
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
        if (ativarChase)
        {
            ativarChase = false;

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
        if (!inimigo.GetComponent<Inimigo>().inimigoExplosivo)
        {
            inimigo.GetComponent<Inimigo>().animator.SetBool("Andando", true);
        }        
        base.Enter();
    }

    public override void Update()
    {
        agent.speed = inimigo.GetComponent<Inimigo>().velocidadeAndar;

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
        if (!inimigo.GetComponent<Inimigo>().inimigoExplosivo)
        {
            inimigo.GetComponent<Inimigo>().animator.SetBool("Andando", false);
        }
            
        base.Exit();
    }
}

public class Atirar : StateInimigos
{
    public Atirar(GameObject _inimigo, NavMeshAgent _agent, Transform _player, int _municao, float _alcanceMaxArma, float _alcanceMinArma, float _cooldownTiro) : base(_inimigo, _agent, _player, _municao, _alcanceMaxArma, _alcanceMinArma, _cooldownTiro)
    {
        stateName = STATE.ATIRAR;
        agent.angularSpeed = 800f;
        
        agent.isStopped = false;
        cooldownTiroAux = cooldownTiro;
    }

    public override void Enter()
    {
        //mudar animação
        //inimigo.GetComponent<Inimigo>().AtualizaStatus("STATUS: Atirando");

        if (!inimigo.GetComponent<Inimigo>().inimigoExplosivo)
        {
            inimigo.GetComponent<Inimigo>().animator.SetBool("Atirando", true);
        }

        base.Enter();
    }

    public override void Update()
    {
        agent.speed = inimigo.GetComponent<Inimigo>().velocidadeAndar;

        if (newReload)
        {
            municaoAux = municao;
            newReload = false;
        }

        if (inimigo.GetComponent<Inimigo>().inimigoSniper || inimigo.GetComponent<Inimigo>().inimigoTorreta)
        {
            pararAndar = true;
        }

        if (inimigo.GetComponent<Inimigo>().inimigoExplosivo)
        {
            agent.acceleration = 30f;
        }

        //mirando no player e verificando se continua no alcançe do tiro
        inimigo.transform.LookAt(player.transform);

        if (pararAndar)
        {
            agent.SetDestination(inimigo.transform.position);
        }
        else if (VejoPlayer())
        {
            Vector3 distanciaTiro = player.position - inimigo.transform.position;

            if (distanciaTiro.magnitude < alcanceMinArma)
            {
                agent.SetDestination(inimigo.transform.position);

                if (inimigo.GetComponent<Inimigo>().inimigoExplosivo)
                {
                    pararAndar = true;
                }
            }
            else if(distanciaTiro.magnitude <= alcanceMaxArma && distanciaTiro.magnitude >= alcanceMinArma)
            {
                agent.SetDestination(player.position);
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
                if (inimigo.GetComponent<Inimigo>().Atirar())
                {
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
                    else
                    {
                        municaoAux--;
                        cooldownTiroAux = cooldownTiro;
                    }
                }
                else if(!pararAndar)
                {
                    agent.SetDestination(player.position);
                    base.Update();
                }
            }
            else
            {
                nextState = new Reload(inimigo, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro);
                stage = EVENT.EXIT;

                return;
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
        if (!inimigo.GetComponent<Inimigo>().inimigoExplosivo)
        {
            inimigo.GetComponent<Inimigo>().animator.SetBool("Atirando", false);
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
        if (!inimigo.GetComponent<Inimigo>().inimigoExplosivo)
        {
            inimigo.GetComponent<Inimigo>().animator.SetBool("Atirando", false);
            inimigo.GetComponent<Inimigo>().animator.SetBool("Andando", false);
        }
        base.Enter();
    }

    public override void Update()
    {
        tempoAux -= Time.deltaTime;

        if (tempoAux <= 0)
        {
            nextState = new Atirar(inimigo, agent, player, municao, alcanceMaxArma, alcanceMinArma, cooldownTiro);
            stage = EVENT.EXIT;
            newReload = true;
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



