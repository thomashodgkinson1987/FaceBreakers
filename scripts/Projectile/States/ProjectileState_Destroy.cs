using Godot;

public class ProjectileState_Destroy : ProjectileState
{

	#region Fields

	private bool m_wasDestroyConditionMet = false;
	private bool m_isDestroyConditionMet = false;

	#endregion // Fields



	#region Constructors

	public ProjectileState_Destroy(Projectile projectile) : base(projectile) { }

	#endregion // Constructors



	#region Public methods

	public override void OnEnter()
	{
		m_projectile.Play_Sound(Projectile.ESound.Free);
		m_projectile.Set_SpriteVisible(false);
		m_projectile.Set_CollisionEnabled(false);
	}

	public override void OnExit() { }

	public override void OnPhysicsProcess(float delta) { }

	public override void OnProcess(float delta)
	{
		if (!m_isDestroyConditionMet)
		{
			if (!m_projectile.Get_SoundPlaying(Projectile.ESound.Init) && !m_projectile.Get_SoundPlaying(Projectile.ESound.Free))
			{
				m_isDestroyConditionMet = true;
			}
		}

		if (m_isDestroyConditionMet && !m_wasDestroyConditionMet)
		{
			m_wasDestroyConditionMet = true;

			m_projectile.Destroy();
		}
	}

	public override void OnAreaEntered(Area2D area) { }

	public override void OnBodyEntered(PhysicsBody2D body) { }

	#endregion // Public methods

}

