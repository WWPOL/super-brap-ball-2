using Godot;

public class PlayerCollision : RigidBody
{
	// How much user inputs should be multiplied before being added as forces.
    const float MOVEMENT_MULT = 300;

    // Maximum lift coefficient. Characterization of wing efficiency, size, drag, ect.
	// See ../assets/lift-coefficient-suggestions.pdf
    const float MAX_LIFT_COEFFICIENT = 1.4f;

    // Wing area in meters squared.
    const float WING_AREA = 1f;

	// Density of air in kg/m^3.
	// https://en.m.wikipedia.org/wiki/Density_of_air
    const float AIR_DENSITY = 1.204f;

    // The collision normal of the player on the ground. Or null if the player is not touching the ground.
    public Vector3? floorCollisionNormal = null;

    // The ball 3D model.
    private Ball ball;

    public override void _Ready()
    {
        this.ball = GetNode<Ball>("ball");
    }

    public override void _PhysicsProcess(float delta)
    {
        this.Sleeping = false;

		var forwardStrength = Input.GetActionStrength("player_forward") - Input.GetActionStrength("player_backward");
		var leftStrength = Input.GetActionStrength("player_left") - Input.GetActionStrength("player_right");

        // Move player
        if (this.ball.isOpen)
        {
            // Air movement
            //this.Mode = ModeEnum.Kinematic;
            var lift_coefficient = this.Rotation.x * 0.06f + 0.4f;
            // MAX_LIFT_COEFFICIENT * this.Rotation.x;
            var lift = lift_coefficient * WING_AREA * 0.5f * AIR_DENSITY * Mathf.Pow(this.LinearVelocity.z, 2);

            GD.Print(lift);

            this.AddCentralForce(new Vector3(0, lift, 0));
        } else
        {
            // Ground movement
            //this.Mode = ModeEnum.Rigid;
			
            forwardStrength *= delta;
            leftStrength *= delta;

            forwardStrength *= MOVEMENT_MULT * this.Mass;
            leftStrength *= MOVEMENT_MULT * this.Mass;

            this.AddCentralForce(new Vector3(leftStrength, 0, forwardStrength));
        }
    }

    public override void _IntegrateForces(PhysicsDirectBodyState state)
    {
        // Detect collision with floor
        if (state.GetContactCount() > 0)
        {
            this.floorCollisionNormal = state.GetContactLocalNormal(0);
        } else {
			this.floorCollisionNormal = null;
		}
    }
}
