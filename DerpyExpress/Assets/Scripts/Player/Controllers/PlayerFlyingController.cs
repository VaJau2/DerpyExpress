using UnityEngine;

namespace DerpyExpress.Player.Controllers
{
    class PlayerFlyingController : BaseController
    {
        private const float HORIZONTAL_FLY_SPEED = 3f;
        private const float FLY_SPEED = 2f;
        private const float MAX_FLY_SPEED = 8f;
        
        private float flyForwardSpeed = 1;
        private float animSpeed;
        private float smoothRotX, smoothRotZ;

        private Vector3 dir;

        public PlayerFlyingController(Player player, bool jump = true): base(player) 
        {
            deacceleration = 2f;
            dir = Vector3.zero;
        }

        public override void UpdateMovement()
        {
            mesh.transform.localEulerAngles = GetRotation();
           
            UpdateFlyForwardSpeed();
            UpdateDirRotation();

            if (GetInput().magnitude > 0)
            {
                dir = GetFlyDirection();
            }
            else
            {
                dir.y = 0;
            }

            if (Input.GetButton("Jump"))
            {
                dir.y = FLY_SPEED;
            }
            else if (Input.GetButton("Run"))
            {
                dir.y = -FLY_SPEED;
            }

            if (controller.isGrounded)
            {
                OnLandOnFloor();
            }

            anim.SetBool("on_floor", controller.isGrounded);
            anim.SetBool("jump", !Input.GetButton("Run"));
            anim.SetFloat("side", GetInput().x);
            anim.SetFloat("speed", GetAnimSpeed());
            
            controller.Move(dir * Time.deltaTime);
        }

        private void UpdateFlyForwardSpeed()
        {
            if (GetInput().y > 0)
            {
                if (flyForwardSpeed < MAX_FLY_SPEED)
                {
                    flyForwardSpeed += Time.deltaTime;
                }
            }
            else
            {
                flyForwardSpeed = 1;
            }
        }

        private Vector3 GetFlyDirection()
        {
            Vector2 input = GetInput();
            Vector3 target = new Vector3();

            if (input.x > 0)
            {
                target += mesh.transform.right * HORIZONTAL_FLY_SPEED;
            }
            else if (input.x < 0)
            {
                target -= mesh.transform.right * HORIZONTAL_FLY_SPEED;
            }

            if (input.y > 0)
            {
                target += mesh.transform.forward * HORIZONTAL_FLY_SPEED * flyForwardSpeed;
            }
            else if (input.y < 0)
            {
                target -= mesh.transform.forward * HORIZONTAL_FLY_SPEED;
            }

            if (target.magnitude > 0)
            {
                return target;
            }

            return Vector3.Lerp(dir, Vector3.zero, deacceleration * Time.deltaTime);
        }

        protected override Vector3 GetRotation()
        {
            Vector2 input = GetInput();
            Vector3 rotation = base.GetRotation();
            
            if (input.y > 0 && input.x == 0)
            {
                rotation.x = cameraHelper.transform.localEulerAngles.x;
                rotation.z = Mathf.Clamp((-smoothRotVelocity / 9f), -60, 60);
            }
            else
            {
                Vector3 currentRotation = mesh.transform.localEulerAngles;
                
                rotation = new Vector3(
                    Mathf.SmoothDampAngle(currentRotation.x, 0, ref smoothRotX, smoothRotTime),
                    rotation.y,
                    Mathf.SmoothDampAngle(currentRotation.z, 0, ref smoothRotZ, smoothRotTime)
                );
            }

            return rotation;
        }

        protected override float GetYDirection(Vector2 input)
        {
            if (GetInput().y <= 0)
            {
                input.x = 0;
                input.y = 1;
            }

            return base.GetYDirection(input);
        }

        private void UpdateDirRotation()
        {
            if (dir.x != 0 || dir.z != 0)
            {
                Vector3 target = GetFlyDirection();
                dir = new Vector3(target.x, dir.y, target.z);
            }
        }

        private void OnLandOnFloor()
        {
            player.isFlying = false;
            player.SetMovementController(new PlayerMovingController(player));
        }

        private float GetAnimSpeed()
        {
            if (GetInput().y > 0)
            {
                animSpeed = flyForwardSpeed - 1;
            }
            else
            {
                animSpeed = Mathf.Lerp(animSpeed, 0, deacceleration * Time.deltaTime);
            }

            return animSpeed;
        }
    }
}
