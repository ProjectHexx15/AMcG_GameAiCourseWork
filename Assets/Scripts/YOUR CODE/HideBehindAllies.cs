using UnityEngine;

public class HideBehindAllies : SteeringBehaviour
{
    public override Vector3 UpdateBehaviour(SteeringAgent steeringAgent)
    {




        Debug.Log("AAAAAAH!!!!!!!!!");
        return steeringVelocity;
    }

}
