using UnityEngine;

public class SeekEnemy : SteeringBehaviour
{
    public override Vector3 UpdateBehaviour(SteeringAgent steeringAgent)
    {
        Debug.Log("i see you");

        return steeringVelocity;


        throw new System.NotImplementedException();
    }
}
