using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Quinn.MovementSystem;
using Quinn;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Patrol", story: "Patrol in Radius [R] of Pos [P]", category: "Action", id: "7e4ddd986ee4e3c7f536480b78f9541b")]
public partial class PatrolAction : Action
{
    [SerializeReference] 
    public BlackboardVariable<float> R;
    [SerializeReference] 
    public BlackboardVariable<Vector2> P;

    [SerializeReference]
    public BlackboardVariable<Vector2> IdleDuration = new(new(2f, 3f));
    [SerializeReference]
    public BlackboardVariable<float> StoppingDst = new(0.1f);

	private CharacterMovement _movement;
    private Vector2 _target;
    private float _nextAllowedMoveTime;

    protected override Status OnStart()
    {
        _movement = GameObject.GetComponent<CharacterMovement>();
        _target = GameObject.transform.position;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Time.time > _nextAllowedMoveTime)
        {
			if (_movement.MoveTo(_target, stoppingDst: StoppingDst.Value))
            {
				_target = GetRandomPos();
				_nextAllowedMoveTime = Time.time + IdleDuration.Value.GetRandom();
			}
		}

        return Status.Running;
    }

    private Vector2 GetRandomPos()
    {
        return P.Value + (UnityEngine.Random.insideUnitCircle * R.Value);
    }
}
