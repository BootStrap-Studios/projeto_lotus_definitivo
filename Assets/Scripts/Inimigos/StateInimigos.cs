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
    public EVENT stage;
    protected GameObject inimigo;
    protected NavMeshAgent agent;
    protected Transform player;
    protected StateInimigos nextState;
    protected int municao;
    protected float alcanceArma;
    protected float cooldownTiro;
    protected float vida;
    protected float danoRecebido;
    //protected Animator anim;

    
    float visDistancia = 20f;
    float visAngulo = 45.0f;
    public float cooldownTiroAux;
    public float cooldownReload;
    //public float municaoAux;
    //public float vidaAtual;

    public StateInimigos(GameObject _inimigo, NavMeshAgent _agent, Transform _player, int _municao, float _alcanceArma, float _vida, float _cooldownTiro)
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
    public Idle(GameObject _inimigo, NavMeshAgent _agent, Transform _player, int _municao, float _alcanceArma, float _vida, float _cooldownTiro) : base(_inimigo, _agent, _player, _municao, _alcanceArma, _vida, _cooldownTiro)
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
    public Chase(GameObject _inimigo, NavMeshAgent _agent, Transform _player, int _municao, float _alcanceArma, float _vida, float _cooldownTiro) : base(_inimigo, _agent, _player, _municao, _alcanceArma, _vida, _cooldownTiro)
    {
        stateName = STATE.CHASE;
        agent.speed = 6f;
        agent.angularSpeed = 250f;
        agent.isStopped = false;
    }

    public override void Enter()
    {
        //mudar animação
        Debug.Log("entrou na chase");
        base.Enter();
    }

    public override void Update()
    {
        //persiguindo player caso ainda não esteja perto o suficiente pra atirar
        agent.SetDestination(player.position);
        //Debug.Log("Passei");

        if (VejoPlayer())
        {
            Vector3 distanciaTiro = player.position - inimigo.transform.position;

            if (distanciaTiro.magnitude <= alcanceArma + 3)
            {
                nextState = new Atirar(inimigo, agent, player, municao, alcanceArma, vida, cooldownTiro);
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
    public Atirar(GameObject _inimigo, NavMeshAgent _agent, Transform _player, int _municao, float _alcanceArma, float _vida, float _cooldownTiro) : base(_inimigo, _agent, _player, _municao, _alcanceArma, _vida, _cooldownTiro)
    {
        stateName = STATE.ATIRAR;
        agent.speed = 3;
        agent.angularSpeed = 800f;
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
        agent.SetDestination(player.position);

        if (VejoPlayer())
        {
            Vector3 distanciaTiro = player.position - inimigo.transform.position;

            if (distanciaTiro.magnitude <= alcanceArma - 2)
            {
                agent.speed = 0f;
            }
            else if(distanciaTiro.magnitude <= alcanceArma + 3 && distanciaTiro.magnitude >= alcanceArma - 2)
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
            inimigo.GetComponent<Inimigo>().Atirar();
            cooldownTiroAux = cooldownTiro;            
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

public class Reload : StateInimigos
{
    public Reload(GameObject _inimigo, NavMeshAgent _agent, Transform _player, int _municao, float _alcanceArma, float _vida, float _cooldownTiro) : base(_inimigo, _agent, _player, _municao, _alcanceArma, _vida, _cooldownTiro)
    {
        stateName = STATE.RELOAD;
        agent.speed = 0f;
        cooldownReload = 2f;
        
    }

    public override void Enter()
    {
        //trocar animação
        cooldownReload = 2f;
        base.Enter();
    }

    public override void Update()
    {
        cooldownReload -= Time.deltaTime;
        Debug.Log("cooldown reload: " + cooldownReload);

        if (cooldownReload <= 0)
        {
            nextState = new Chase(inimigo, agent, player, municao, alcanceArma, vida, cooldownTiro);
            stage = EVENT.EXIT;
        }

        base.Update();
    }

    public override void Exit()
    {
        //reset da animação
        base.Exit();
    }
}

/* ============ ESTADO DE TOMAR DANO ==================
 * 
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
*/



