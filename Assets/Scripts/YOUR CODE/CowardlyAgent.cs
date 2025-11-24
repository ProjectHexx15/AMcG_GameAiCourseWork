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

    private bool EnemyTooClose(float range)
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

}
