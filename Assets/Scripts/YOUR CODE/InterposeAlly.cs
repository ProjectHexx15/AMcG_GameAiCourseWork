using UnityEngine;

public class InterposeAlly : SteeringBehaviour
{
    private SteeringAgent allyToProtect;
    private SteeringAgent enemyAttacking;


    public override Vector3 UpdateBehaviour(SteeringAgent steeringAgent)
    {
        return steeringVelocity;

    }

}
