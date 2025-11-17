using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtectiveAgent : SteeringAgent
{
    protected enum state
    {
        FollowLeader,
        SeenEnemy,
        AttackEnemy,
        DefendAlly
    
    }

    private state currentState;
   // private List<Attack> attacks;
    public SteeringAgent closestAlly;
    private float sightRadius = 15.0f;
    private float attackRadius = 10.0f;


    protected override void InitialiseFromAwake()
    {
        gameObject.AddComponent<OP_Protective>();

        // initially following leader
        gameObject.AddComponent<Prot_Attack>().enabled = false;
        gameObject.AddComponent<InterposeAlly>().enabled = false;
        gameObject.AddComponent<EnemyInSight>().enabled = false;

    }

    protected override void CooperativeArbitration()
    {
        base.CooperativeArbitration();

        switch (currentState)
        {
        
            case state.FollowLeader:

                if(EnemyInSight())
                {
                    currentState = state.SeenEnemy;
                    gameObject.GetComponent<OP_Protective>().enabled = false;
                    gameObject.GetComponent<EnemyInSight>().enabled = true;
                    break;

                }

                break;

            case state.SeenEnemy:

                if(EnemyInAttackRange())
                {
                    currentState = state.AttackEnemy;
                    gameObject.GetComponent<EnemyInSight>().enabled = false;
                    gameObject.GetComponent<Prot_Attack>().enabled = true;
                    break;
                }

                if(!EnemyInSight() && !EnemyInAttackRange())
                {
                    currentState = state.FollowLeader;
                    gameObject.GetComponent<EnemyInSight>().enabled = false;
                    gameObject.GetComponent<OP_Protective>().enabled = true;
                    break;
                }

                if(EnemyAttackAlly())
                {
                    currentState = state.DefendAlly;
                    gameObject.GetComponent<EnemyInSight>().enabled = false;
                    gameObject.GetComponent<InterposeAlly>().enabled = true;
                    break;
                }
               
                break;

            case state.AttackEnemy:

                if(!EnemyInAttackRange())
                {
                    currentState = state.SeenEnemy;
                    gameObject.GetComponent<EnemyInSight>().enabled = true;
                    gameObject.GetComponent<Prot_Attack>().enabled = false;
                    break;
                }

                break;


            case state.DefendAlly:


                break;

        
        }

    }

    private bool EnemyInSight()
    {
        for (int i = 0; i < GameData.Instance.enemies.Count; i++)
        {
            // calculate distance between player and each enemy
            float distance = Vector3.Distance(this.transform.position, GameData.Instance.enemies[i].transform.position);

            // if the enemy is within the sight range
            if (distance <= sightRadius)
            {
                return true;
            }

        }
        return false;

    }



    private bool EnemyInAttackRange()
    {
        for (int i = 0; i < GameData.Instance.enemies.Count; i++)
        {
            // calculate distance between player and each enemy
            float distance = Vector3.Distance(this.transform.position, GameData.Instance.enemies[i].transform.position);

            // if the enemy is within the sight range
            if (distance <= attackRadius)
            {
                return true;
            }

        }
        return false;
    }

    private bool EnemyAttackAlly()
    {
        // find closest ally so it can be protected
        closestAlly = GetNearestAgent(transform.position, GameData.Instance.allies);

        if(closestAlly = null)
        {
            return false;
        }

        var attacks = GameData.Instance.attacks;
        var closestAttack = GetNearestAttack(closestAlly.transform.position, attacks);
        //if(closestAttack.Type != Attack.AttackType.None)
        //{
          //  var attackVector = closestAttack.currentPosition - closestAlly.transform.position;
           // var targetPosition = Vector3.Normalize(attackVector) * 2.0f + closestAlly.transform.position;
       // }

        foreach (var attack in attacks)
        {
            if(attack.IsEnemy && attack.Type == Attack.AttackType.EnemyGun)
            {

                var attackDirection = attack.Direction;
                var attackToAllyDirection = Vector3.Normalize(closestAlly.transform.position - attack.currentPosition);

                // check if the attack is cominig to nearest ally
                float dot = Vector3.Dot(attackToAllyDirection, attackDirection);
                Debug.Log(dot);

                if(dot < 0.5)
                {
                    return true;
                }

            }
        }
        return false;


    }

    public static Attack GetNearestAttack(Vector3 position, List<Attack> attacks)
    {
        Attack nearestAttack = new Attack(Attack.AttackType.None, null, null);
        float nearestSquareDistance = float.MaxValue;
        foreach (var attack in attacks)
        {
            if(!attack.IsEnemy)
            {
                continue;
            }

            var squareDistance = (attack.currentPosition - position).sqrMagnitude;
            if (squareDistance < nearestSquareDistance)
            {
                nearestSquareDistance = squareDistance;
                nearestAttack = attack;
            }
        }
        return nearestAttack;
    }

}
