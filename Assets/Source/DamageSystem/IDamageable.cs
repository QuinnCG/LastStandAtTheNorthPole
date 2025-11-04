namespace Quinn.DamageSystem
{
	public interface IDamageable
	{
		public bool CanDamage(DamageInfo info);
		public bool TryTakeDamage(DamageInfo info, bool force = false);
		public void Kill();
	}
}
