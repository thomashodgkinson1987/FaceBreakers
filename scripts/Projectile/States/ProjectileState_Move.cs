using Godot;

public class ProjectileState_Move : ProjectileState
{
    public ProjectileState_Move() { }

    public override void OnEnter(Projectile projectile)
    {
        projectile.PlaySound_Instantiate();
    }
    public override void OnExit(Projectile projectile) { }

    public override void OnPhysicsProcess(Projectile projectile, float delta)
    {
        if (projectile.LifetimeTimer < projectile.Lifetime)
        {
            projectile.LifetimeTimer += delta;
            if (projectile.LifetimeTimer < projectile.Lifetime)
            {
                Vector2 translation = (Vector2.Up).Rotated(projectile.View.Rotation);
                translation *= projectile.MoveSpeed * delta;
                projectile.View.Translate(translation);
            }
            else
            {
                projectile.SetState(new ProjectileState_Destroy());
            }
        }
    }
    public override void OnProcess(Projectile projectile, float delta) { }

    public override void OnAreaEntered(Projectile projectile, Area2D area)
    {
        //TODO Find better way of managing states
        projectile.SetState(new ProjectileState_Destroy());
    }
    public override void OnBodyEntered(Projectile projectile, PhysicsBody2D body)
    {
        //TODO Find better way of managing states
        projectile.SetState(new ProjectileState_Destroy());
    }
}

