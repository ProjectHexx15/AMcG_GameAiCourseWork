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
        gameObject.AddComponent<OP_Courageous>();

        // initially following - therefore false
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
                    currentState = State.SeenEnemy;
                    gameObject.GetComponent<OP_Courageous>().enabled = false;
                    gameObject.GetComponent<SeekEnemy>().enabled = true;
                    break;
                }


                break;

            case State.SeenEnemy:

                if(EnemyInAttackRange())
                {
                    currentState = State.AttackEnemy;
                    gameObject.GetComponent<SeekEnemy>().enabled = false;
                    gameObject.GetComponent<Cour_AttackEnemy>().enabled = true;
                    break;

                }

                if(!EnemyInAttackRange() || !EnemyInSight())
                {
                    currentState = State.FollowLeader;
                    gameObject.GetComponent<SeekEnemy>().enabled = false;
                    gameObject.GetComponent<OP_Courageous>().enabled = true;
                    break;
                }


                break;

            case State.AttackEnemy:

                if(!EnemyInAttackRange())
                {
                    currentState = State.SeenEnemy;
                    gameObject.GetComponent<SeekEnemy>().enabled = true;
                    gameObject.GetComponent<Cour_AttackEnemy>().enabled = false;
                    break;
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

            // if the enemy is within the sight range
            if (distance <= attackRadius)
            {
                return true;
            }

        }
        return false;
    }
    

}
