using UnityEngine;

public class CowardlyAgent : SteeringAgent
{
    protected enum state
    {
        FollowLeader,
        SeenEnemy,
        AttackEnemy,
        Hide
    }

    private state currentState;
    public SteeringAgent closestAlly;
    private float sightRadius = 15.0f;
    private float attackRadius = 10.0f;
    private float hideRange = 5f;


    protected override void InitialiseFromAwake()
    {
        gameObject.AddComponent<OP_Cowardly>();

        gameObject.AddComponent<EnemyInSight>().enabled = false;
        gameObject.AddComponent<HideBehindAllies>().enabled = false;
        gameObject.AddComponent<Cow_Attack>().enabled = false;

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
                    gameObject.GetComponent<OP_Cowardly>().enabled = false;
                    gameObject.GetComponent<EnemyInSight>().enabled = true;
                    break;
                }

                break;

            case state.SeenEnemy:

                if (!EnemyInAttackRange() && !EnemyInSight())
                {
                    currentState = state.FollowLeader;
                    gameObject.GetComponent<EnemyInSight>().enabled = false;
                    gameObject.GetComponent<OP_Cowardly>().enabled = true;
                    break;
                }

                if (EnemyTooClose())
                {
                    currentState = state.Hide;
                    gameObject.GetComponent<EnemyInSight>().enabled = false;
                    gameObject.GetComponent<HideBehindAllies>().enabled = true;
                    break;
                }


                if (EnemyInAttackRange())
                {
                    currentState = state.AttackEnemy;
                    gameObject.GetComponent<EnemyInSight>().enabled = false;
                    gameObject.GetComponent<Cow_Attack>().enabled = true;
                    break;
                }





                break;

            case state.AttackEnemy:

                if(!EnemyInAttackRange())
                {
                    currentState = state.SeenEnemy;
                    gameObject.GetComponent<Cow_Attack>().enabled = false;
                    gameObject.GetComponent<OP_Cowardly>().enabled = true;
                    break;
                }

                if(EnemyTooClose())
                {
                    currentState = state.Hide;
                    gameObject.GetComponent<Cow_Attack>().enabled = false;
                    gameObject.GetComponent<HideBehindAllies>().enabled = true;
                }


                break;

            case state.Hide:

                if(!EnemyTooClose())
                {
                    currentState = state.SeenEnemy;
                    gameObject.GetComponent<HideBehindAllies>().enabled = false;
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

    private bool EnemyTooClose()
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

            if (distance <= hideRange)
                return true;
        }
        return false;
    }




}
