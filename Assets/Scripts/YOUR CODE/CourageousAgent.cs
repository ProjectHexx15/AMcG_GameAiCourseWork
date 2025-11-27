using System.Collections.Generic;
using UnityEngine;

public class CourageousAgent : SteeringAgent
{
    protected enum State
    {
        FollowLeader,
        SeenEnemy,
        AttackEnemy
    
    }

    private State currentState;
    private float sightRadius = 10.0f;
    private float attackRadius = 2.0f;

    protected override void InitialiseFromAwake()
    {
        currentState = State.FollowLeader;
        gameObject.AddComponent<OP_Courageous>().enabled = true;
        gameObject.AddComponent<SeekEnemy>().enabled = false;
        gameObject.AddComponent<Cour_AttackEnemy>().enabled = false;
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
                else if(!EnemyInSight())
                {
                    SwitchState(State.FollowLeader);
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
        }

        Debug.Log($"Enemy count: {GameData.Instance.enemies.Count}");
        Debug.Log($"EnemyInSight: {EnemyInSight()}");
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

    // helper function
    private void SwitchState(State newState)
    {
        // Disable all behaviours
        gameObject.GetComponent<OP_Courageous>().enabled = false;
        gameObject.GetComponent<SeekEnemy>().enabled = false;
        gameObject.GetComponent<Cour_AttackEnemy>().enabled = false;

        // enable the relevant state only

        switch (newState)
        {
            case State.FollowLeader:
                gameObject.GetComponent<OP_Courageous>().enabled = true;
                break;

            case State.SeenEnemy:
                gameObject.GetComponent<SeekEnemy>().enabled = true;
                break;

            case State.AttackEnemy:
                gameObject.GetComponent<Cour_AttackEnemy>().enabled = true;
                break;
        
        }

        currentState = newState;
        Debug.Log($"{name} switched to{newState}");
    }

    private List <SteeringAgent> GetValidEnemies()
    {
        // create a new list of only valid enemies
        List<SteeringAgent> validEnemies = new List<SteeringAgent>();

        foreach(var enemy in GameData.Instance.enemies)
        {
            if(enemy ==  null)
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
