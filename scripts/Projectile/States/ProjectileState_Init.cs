using Godot;

public class ProjectileState_Init : ProjectileState
{

	public ProjectileState_Init(Projectile projectile) : base(projectile) {  }


	public override void OnEnter()
	{
		m_projectile.PlaySound_Instantiate();
		m_projectile.SetState(Projectile.EStateName.Moving);
	}

	public override void OnExit() {  }

	public override void OnPhysicsProcess(float delta) {  }

	public override void OnProcess(float delta) {  }

	public override void OnAreaEntered(Area2D area) {  }

	public override void OnBodyEntered(PhysicsBody2D body) {  }

}

