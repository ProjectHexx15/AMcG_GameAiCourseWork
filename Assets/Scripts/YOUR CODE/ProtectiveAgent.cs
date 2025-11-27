using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtectiveAgent : SteeringAgent
{
    protected enum State
    {
        FollowLeader,
        SeenEnemy,
        AttackEnemy,
        DefendAlly
    
    }

    private State currentState;
    public SteeringAgent closestAlly;
    public Attack possibleAttack;
    private float sightRadius = 15.0f;
    private float attackRadius = 10.0f;

    protected override void InitialiseFromAwake()
    {
        currentState = State.FollowLeader;
        gameObject.AddComponent<OP_Protective>().enabled = true;
        gameObject.AddComponent<Prot_Attack>().enabled = false;
        gameObject.AddComponent<InterposeAlly>().enabled = false;
        gameObject.AddComponent<EnemyInSight>().enabled = false;

    }

    protected override void CooperativeArbitration()
    {
        base.CooperativeArbitration();

        switch (currentState)
        {
        
            case State.FollowLeader:

                if(EnemyInSight())
                {
                    SwitchState(State.SeenEnemy);
                }

                break;


            case State.SeenEnemy:

                if(EnemyInAttackRange())
                {
                    SwitchState(State.AttackEnemy);
                }

                if(!EnemyInSight() && !EnemyInAttackRange())
                {

                    SwitchState(State.FollowLeader);
                }

                if(EnemyAttackAlly())
                {
                    SwitchState(State.DefendAlly);
                }
                break;

            case State.AttackEnemy:

                if(!EnemyInAttackRange() && EnemyInSight())
                {
                    SwitchState(State.SeenEnemy);
                }

                else if(!EnemyInAttackRange() && !EnemyInSight())
                {
                    SwitchState(State.FollowLeader);
                }
                break;


            case State.DefendAlly:

                if(!EnemyAttackAlly() && EnemyInAttackRange())
                {
                    SwitchState(State.AttackEnemy);
                }

                if(!EnemyAttackAlly() && !EnemyInAttackRange())
                {
                    SwitchState(State.SeenEnemy);
                }
                break;        
        }

    }

    private bool EnemyInSight()
    {
        foreach (var enemy in GetValidEnemies())
        {
            // calculate distance between this agent and the valid enemy
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= sightRadius)
            {
                // return true if in sight
                return true;
            }

        }
        return false;

    }

    private bool EnemyInAttackRange()
    {
        foreach (var enemy in GetValidEnemies())
        {
            // calculate distance between this agent and enemy
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= attackRadius)
            {
                // return true if within sight
                return true;
            }

        }

        return false;
    }

    private bool EnemyAttackAlly()
    {
        // ensure no null enemies considered - causes issues with defending allies
        if (GameData.Instance == null || GameData.Instance.allies == null)
            return false;

        // calculate the nearest aggent to this agent
        closestAlly = GetNearestAgent(transform.position, GameData.Instance.allies);

        if (closestAlly == null)
            return false;

        var attacks = GameData.Instance.attacks;
        if (attacks == null || attacks.Count == 0)
            return false;

        // loop through attacks
        foreach (var attack in attacks)
        {
            // ensure there are no null attacks - cuases erorrs with defendig allies
            if (attacks == null || attack.AttackerAgent == null) continue;

            if (attack.IsEnemy && attack.Type == Attack.AttackType.EnemyGun)
            {
                // calculate attack direction
                var attackDirection = attack.Direction;

                // calculate direction from attack to ally
                var attackToAllyDirection = Vector3.Normalize(
                    closestAlly.transform.position - attack.currentPosition);

                // calculate dot product to determine if attack will hit ally
                float dot = Vector3.Dot(attackToAllyDirection, attackDirection);

                if (dot > 0.7f) // traveling towards ally
                {
                    possibleAttack = attack;
                    return true;
                }
                    

                if(dot > 0.3f && dot < 0.7f) // kinda close to ally
                {
                    // only go if allies health is low
                    if(closestAlly.Health < 0.25) // quater left
                    {
                        possibleAttack = attack;
                        return true;
                    }

                }


                if(dot < 0.3f) // not on target at all
                    return false;
            }
        }
        return false;
    }

    // helper function
    private void SwitchState(State newState)
    {
        // Disable all behaviours
        gameObject.GetComponent<OP_Protective>().enabled = false;
        gameObject.GetComponent<EnemyInSight>().enabled = false;
        gameObject.GetComponent<Prot_Attack>().enabled = false;
        gameObject.GetComponent<InterposeAlly>().enabled = false;

        // enable the relevant state only

        switch (newState)
        {
            case State.FollowLeader:
                gameObject.GetComponent<OP_Protective>().enabled = true;
                break;

            case State.SeenEnemy:
                gameObject.GetComponent<EnemyInSight>().enabled = true;
                break;

            case State.AttackEnemy:
                gameObject.GetComponent<Prot_Attack>().enabled = true;
                break;

            case State.DefendAlly:
                gameObject.GetComponent<InterposeAlly>().enabled = true;
                break;


        }

        currentState = newState;
    }

    private List<SteeringAgent> GetValidEnemies()
    {
        // create a new list of only valid enemies
        List<SteeringAgent> validEnemies = new List<SteeringAgent>();

        foreach (var enemy in GameData.Instance.enemies)
        {
            if (enemy == null)
            {
                continue;
            }

            if (!enemy.gameObject.activeInHierarchy)
            {
                continue;
            }

            validEnemies.Add(enemy);
        }

        return validEnemies;
    }


}




