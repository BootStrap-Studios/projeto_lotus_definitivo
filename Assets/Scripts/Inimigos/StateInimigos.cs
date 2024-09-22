using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static StateInimigos;

public class StateInimigos
{
    public enum STATE
    {
        IDLE, CHASE, ATIRAR, RELOAD, HIT
    }

    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    }

    public STATE stateName;
    protected EVENT stage;
    protected GameObject inimigo;
    protected NavMeshAgent agent;
    protected Transform player;
    protected StateInimigos nextState;
    protected int municao;
    protected float alcanceArma;
    protected int vida;
    protected float cooldownTiro;
    //protected Animator anim;

    
    float visDistancia = 20f;
    float visAngulo = 45.0f;
    public float cooldownTiroAux;
    public float municaoAux;
    public int vidaAtual;

    public StateInimigos(GameObject _inimigo, NavMeshAgent _agent, Transform _player, int _municao, float _alcanceArma, int _vida, float _cooldownTiro)
    {
        inimigo = _inimigo;
        agent = _agent;
        player = _player;
        municao = _municao;
        alcanceArma = _alcanceArma;
        vida = _vida;
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

        RaycastHit hit;
        
        if(direcao.magnitude < visDistancia && angulo < visAngulo)
        {
            if(Physics.Linecast(inimigo.transform.position, player.position, out hit))
            {
                if (hit.transform.tag == "Player")
                {
                    return true;
                }
                else
                { return false;}
            }
            else
            { return false; }
        }

        return false;
    }
}

public class Idle : StateInimigos
{
    public Idle(GameObject _inimigo, NavMeshAgent _agent, Transform _player, int _municao, float _alcanceArma, int _vida, float _cooldownTiro) : base(_inimigo, _agent, _player, _municao, _alcanceArma, _vida, _cooldownTiro)
    {
        stateName = STATE.IDLE;
        agent.isStopped = true; 
    }
    
    public override void Enter()
    {
        //mudar animação
        //Debug.Log("entrou em idle");
        base.Enter();
    }

    public override void Update()
    {
        //verificando se player está em seu campo de visão
        //Debug.Log("checkando");
        if (VejoPlayer())
        {
            nextState = new Chase(inimigo, agent, player, municao, alcanceArma, vida, cooldownTiro);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        //reset da animação
        //Debug.Log("saiu do idle");
        base.Exit();
    }
}

public class Chase : StateInimigos
{
    public Chase(GameObject _inimigo, NavMeshAgent _agent, Transform _player, int _municao, float _alcanceArma, int _vida, float _cooldownTiro) : base(_inimigo, _agent, _player, _municao, _alcanceArma, _vida, _cooldownTiro)
    {
        stateName = STATE.CHASE;
        agent.speed = 6f;
        agent.angularSpeed = 250f;
        agent.isStopped = false;
    }

    public override void Enter()
    {
        //mudar animação
        //Debug.Log("entrou na chase");
        base.Enter();
    }

    public override void Update()
    {
        //persiguindo player caso ainda não esteja perto o suficiente pra atirar
        agent.SetDestination(player.position);

        if (VejoPlayer())
        {
            Vector3 distanciaTiro = player.position - inimigo.transform.position;

            if (distanciaTiro.magnitude <= alcanceArma + 2)
            {
                nextState = new Atirar(inimigo, agent, player, municao, alcanceArma, vida, cooldownTiro);
                stage = EVENT.EXIT;
            }
        }

    }

    public override void Exit()
    {
        //reset da animação
        //Debug.Log("saiu da chase");
        base.Exit();
    }
}

public class Atirar : StateInimigos
{
    public Atirar(GameObject _inimigo, NavMeshAgent _agent, Transform _player, int _municao, float _alcanceArma, int _vida, float _cooldownTiro) : base(_inimigo, _agent, _player, _municao, _alcanceArma, _vida, _cooldownTiro)
    {
        stateName = STATE.ATIRAR;
        agent.speed = 3;
        agent.angularSpeed = 500f;
        agent.isStopped = false;
        cooldownTiroAux = cooldownTiro;
        municaoAux = municao;
    }

    public override void Enter()
    {
        //mudar animação
        //Debug.Log("entrando no modo de atirar");
        base.Enter();
    }

    public override void Update()
    {   

        agent.SetDestination(player.position);
        if (VejoPlayer())
        {
            Vector3 distanciaTiro = player.position - inimigo.transform.position;

            if (distanciaTiro.magnitude <= alcanceArma - 2)
            {
                agent.speed = 0f;
            }
            else if(distanciaTiro.magnitude <= alcanceArma + 2 && distanciaTiro.magnitude >= alcanceArma - 3)
            {
                agent.speed = 3f;  
            }
            else
            {
                nextState = new Chase(inimigo, agent, player, municao, alcanceArma, vida, cooldownTiro);
                stage = EVENT.EXIT;
            }
        }
        else
        {
            nextState = new Chase(inimigo, agent, player, municao, alcanceArma, vida, cooldownTiro);
            stage = EVENT.EXIT;
        }

        
        cooldownTiroAux-= Time.deltaTime;
        if (cooldownTiroAux <= 0)
        {
            Atirando();
        }
        else
        {
            base.Update();
        }
    }

    

    private void Atirando()
    {
        if(municaoAux > 0)
        {
            municaoAux--;
            //Debug.Log("POWWW!!! munição: " + municaoAux);
            cooldownTiroAux = cooldownTiro;
        }
        else
        {
            nextState = new Reload(inimigo, agent, player, municao, alcanceArma, vida, cooldownTiro);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        //reset da animação
        //Debug.Log("saindo do modo de Atirar");
        base.Exit();
    }
}

public class Reload : StateInimigos
{
    public Reload(GameObject _inimigo, NavMeshAgent _agent, Transform _player, int _municao, float _alcanceArma, int _vida, float _cooldownTiro) : base(_inimigo, _agent, _player, _municao, _alcanceArma, _vida, _cooldownTiro)
    {
        stateName = STATE.RELOAD;
        agent.isStopped = true;
    }

    public override void Enter()
    {
        //trocar animação
        Debug.Log("reload");
        base.Enter();
    }

    public override void Update()
    {
        municaoAux = municao;
        nextState = new Atirar(inimigo, agent, player, municao, alcanceArma, vida, cooldownTiro);
        stage = EVENT.EXIT;
    }

    public override void Exit()
    {
        //reset da animação
        base.Exit();
    }
}

public class TomarDano : StateInimigos
{
    public TomarDano(GameObject _inimigo, NavMeshAgent _agent, Transform _player, int _municao, float _alcanceArma, int _vida, float _cooldownTiro) : base(_inimigo, _agent, _player, _municao, _alcanceArma, _vida, _cooldownTiro)
    {
        stateName = STATE.HIT;
        vidaAtual = 3;
    }

    public override void Enter()
    {
        //possível animação de levar dano
        Debug.Log("Estado de levar dano");
        base.Enter();
    }

    public override void Update()
    {
        vidaAtual--;
        inimigo.GetComponent<BarraDeVida>().AlterarBarraDeVida(vidaAtual, vida);

        if(vidaAtual <= 0)
        {
            GameObject.Destroy(inimigo.gameObject);
        }
        else
        {
            nextState = new Chase(inimigo, agent, player, municao, alcanceArma, vida, cooldownTiro);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        //Reset da Animação
        base.Exit();
    }
}




