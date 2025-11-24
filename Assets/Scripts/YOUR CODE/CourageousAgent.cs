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
    }

    private bool EnemyInSight()
    {
        for(int i = 0; i < GameData.Instance.enemies.Count; i++)
        {
            // calculate distance between player and each enemy
            float distance = Vector3.Distance(this.transform.position, GameData.Instance.enemies[i].transform.position);

            // if the enemy is within the sight range
            if(distance <= sightRadius)
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

            // if the enemy is within the attack range
            if (distance <= attackRadius)
            {
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
    }

}
