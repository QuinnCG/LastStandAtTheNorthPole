using Quinn;
using Quinn.PlayerSystem;
using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Player Is Near", story: "Player is Within [Radius]", category: "Conditions", id: "0103e5cf96da425bfc8565e70ef6f820")]
public partial class PlayerIsNearCondition : Condition
{
    [SerializeReference] 
    public BlackboardVariable<float> Radius;

    public override bool IsTrue()
    {
        return Player.Instance.transform.position.DistanceTo(GameObject.transform.position) <= Radius.Value;
    }
}
