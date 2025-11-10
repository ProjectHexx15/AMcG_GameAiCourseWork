using UnityEngine;

public class ProtectiveAgent : SteeringAgent
{
    protected override void InitialiseFromAwake()
    {
        gameObject.AddComponent<OP_Protective>();
    }


}
