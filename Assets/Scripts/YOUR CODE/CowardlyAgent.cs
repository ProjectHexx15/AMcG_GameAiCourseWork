using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CowardlyAgent : SteeringAgent
{
    protected enum State
    {
        FollowLeader,
        SeenEnemy,
        AttackEnemy,
        Hide
    }

    private State currentState;
    public SteeringAgent closestAlly;
    private float sightRadius = 15.0f;
    private float attackRadius = 10.0f;
    private float hideEnterRange = 5f;
    private float hideExitRange = 6f;


    protected override void InitialiseFromAwake()
    {
        currentState = State.FollowLeader;
        gameObject.AddComponent<OP_Cowardly>().enabled = true;
        gameObject.AddComponent<EnemyInSight>().enabled = false;
        gameObject.AddComponent<HideBehindAllies>().enabled = false;
        gameObject.AddComponent<Cow_Attack>().enabled = false;

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

                if (EnemyTooClose(hideEnterRange))
                {
                    SwitchState(State.Hide);
                }

                if (EnemyInAttackRange())
                {
                    SwitchState(State.AttackEnemy);
                }

                if (!EnemyInSight())
                {
                    SwitchState(State.FollowLeader);
                }
                break;

            case State.AttackEnemy:

                if (EnemyTooClose(hideEnterRange))
                {
                    SwitchState(State.Hide);
                }

                if (!EnemyInAttackRange() && EnemyInSight())
                {
                    SwitchState(State.SeenEnemy);
                }

                else if (!EnemyInAttackRange() && !EnemyInSight())
                {
                    SwitchState(State.FollowLeader);
                }
                break;

            case State.Hide:

                if (!EnemyInSight())
                {
                    SwitchState(State.FollowLeader);
                }

                if (!EnemyTooClose(hideExitRange))
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

    private bool EnemyTooClose(float range)
    {


        foreach (var enemy in GetValidEnemies())
        {
            // to ensure there are no null enemies being considered 
            if (enemy == null) continue;

            // calculate distance
            float distance = Vector3.Distance(transform.position, enemy.transform.position);   

            if (distance <= range)
                return true;
        }
        return false;
    }

    // helper function
    private void SwitchState(State newState)
    {
        // Disable all behaviours
        gameObject.GetComponent<OP_Cowardly>().enabled = false;
        gameObject.GetComponent<EnemyInSight>().enabled = false;
        gameObject.GetComponent<Cow_Attack>().enabled = false;
        gameObject.GetComponent<HideBehindAllies>().enabled = false;

        // enable the relevant state only

        switch (newState)
        {
            case State.FollowLeader:
                gameObject.GetComponent<OP_Cowardly>().enabled = true;
                break;

            case State.SeenEnemy:
                gameObject.GetComponent<EnemyInSight>().enabled = true;
                break;

            case State.AttackEnemy:
                gameObject.GetComponent<Cow_Attack>().enabled = true;
                break;

            case State.Hide:
                gameObject.GetComponent<HideBehindAllies>().enabled = true;
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


