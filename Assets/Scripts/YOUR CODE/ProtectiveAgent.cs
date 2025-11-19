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

                if(!EnemyAttackAlly() && EnemyInAttackRange())
                {
                    currentState = state.AttackEnemy;
                    gameObject.GetComponent<InterposeAlly>().enabled = false;
                    gameObject.GetComponent<Prot_Attack>().enabled = true;
                    break;
                }

                if(!EnemyAttackAlly() && !EnemyInAttackRange())
                {
                    currentState = state.AttackEnemy;
                    gameObject.GetComponent<InterposeAlly>().enabled = false;
                    gameObject.GetComponent<EnemyInSight>().enabled = true;
                    break;
                }



                break;

        
        }

    }

    private bool EnemyInSight()
    {
        // ensuring no null values are in the list
        if (GameData.Instance == null || GameData.Instance.enemies == null)
            return false;

        foreach (var enemy in GameData.Instance.enemies)
        {
            // to ensure there are no null enemies being considered 
            if (enemy == null) continue;

            // calculate distance
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance <= sightRadius)
                return true;
        }
        return false;
    }

    private bool EnemyInAttackRange()
    {
        // ensure no null values are considered
        if (GameData.Instance == null || GameData.Instance.enemies == null)
            return false;

        // for each enemy
        foreach (var enemy in GameData.Instance.enemies)
        {
            if (enemy == null) continue;
            // calculate distance to enemy
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance <= attackRadius)
                return true;
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
                Debug.Log($"Dot product vs ally: {dot}");

                if (dot > 0.7f) // traveling towards ally
                    return true;

                if(dot > 0.3f && dot < 0.7f) // kinda close to ally
                {
                    // only go if allies health is low
                    if(closestAlly.Health < 0.25) // quater left
                    {
                        return true;
                    }

                }


                if(dot < 0.3f) // not on target at all
                    return false;
            }
        }
        return false;
    }

    public static Attack GetNearestAttack(Vector3 position, List<Attack> attacks)
    {
        // stores the nearest attack - set to null so attack is not created
        Attack nearestAttack = new Attack(Attack.AttackType.None, null, null);
        float nearestSquareDistance = float.MaxValue;

        foreach (var attack in attacks)
        {
            // more defensive guards
            if (attacks == null || !attack.IsEnemy)
                continue;
            // calculate squared distance
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
