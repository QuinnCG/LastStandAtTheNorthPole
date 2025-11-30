using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Quinn.MissileSystem.CustomBehaviors
{
    public class RainbowLight : CustomMissileBehavior
	{
        [SerializeField]
        private Gradient Gradient = new();
        [SerializeField]
        private float Speed = 1f;

        private Light2D? _light;

        public override void OnSpawn(Missile self)
        {
            _light = self.gameObject.GetComponent<Light2D>();
        }

        public override void OnUpdate()
        {
            _light!.color = Gradient.Evaluate(Time.time * Speed);
        }
	}
}
