using UnityEngine;

namespace DerpyExpress.Player.Controllers
{
    class PlayerFlyingController : BaseController
    {
        private const float JUMP_START_TIME = 0.5f;
        private const float JUMP_SPEED = 5f;
        private const float MIN_JUMP_SPEED = 4f;
        private const float FLY_DELTA = 5f;
        private const float MAX_FLY_SPEED = 9f;
        private const float DEACCELERATION = 6f;
        private const float FLY_COOLDOWN = 0.5f;
        private bool isJumping;
        private float jumpSpeed;
        private float jumpStartingTime;

        private float flyCooldown;
        private float tempDeacceleration;

        private Vector3 dir;
        private FlyDirection flyDirection = FlyDirection.back;

        private float flySpeed;

        public PlayerFlyingController(Player player, bool jump = true): base(player) 
        {
            acceleration = 5f * Time.deltaTime;
            deacceleration = 5f * Time.deltaTime;
            dir = Vector3.zero;

            if (jump)
            {
                anim.SetTrigger("jump");
                isJumping = true;
                jumpSpeed = JUMP_SPEED;
                jumpStartingTime = JUMP_START_TIME;
            }
        }

        public override void UpdateMovement()
        {
            if (jumpStartingTime > 0)
            {
                jumpStartingTime -= Time.deltaTime;
                return;
            }

            anim.SetBool("on_floor", controller.isGrounded);
            mesh.transform.localEulerAngles = GetRotation();

            if (isJumping)
            {
                UpdateJumpSpeed();
                dir.y = jumpSpeed;
            }
            else
            {
                UpdateDirRotation();
                UpdateDynamicVariables();
                
                if (Input.GetButton("Jump") && flyCooldown <= 0)
                {
                    tempDeacceleration = 0;
                    flyCooldown = FLY_COOLDOWN;
                    anim.SetTrigger("jump");

                    if (flySpeed < MAX_FLY_SPEED)
                    {
                        flySpeed += FLY_DELTA;
                    }

                    if (GetInput().magnitude > 0)
                    {
                        UpdateFlyDirectionFromInput();
                        dir = GetFlyDirection();
                    }
                    else
                    {
                        dir = mesh.transform.up * (flySpeed / 2);
                    }
                }
                
                if (flySpeed < 5 && dir.y > -player.gravity)
                {
                    dir.y -= player.gravity * Time.deltaTime;
                }

                if (controller.isGrounded)
                {
                    OnLandOnFloor();
                }
            }
       
            anim.SetFloat("side", GetInput().x);
            anim.SetFloat(
                "speed", 
                flyDirection == FlyDirection.forward ? GetSpeed() * flySpeed * 2 : 0
            );
            
            controller.Move(dir * Time.deltaTime);
        }

        private void UpdateDynamicVariables()
        {
            if (flyCooldown > 0)
            {
                flyCooldown -= Time.deltaTime;
            }

            if (tempDeacceleration < DEACCELERATION)
            {
                tempDeacceleration += 2f * Time.deltaTime;
            }

            if (flySpeed > 0)
            {
                flySpeed -= tempDeacceleration * Time.deltaTime;
            }
        }

        private void UpdateFlyDirectionFromInput()
        {
            Vector2 input = GetInput();

            if (input.y > 0)
            {
                flyDirection = FlyDirection.forward;
            }
            else
            {
                flyDirection = FlyDirection.back;

                if (input.x < 0)
                {
                    flyDirection = FlyDirection.left;
                }
                else if (input.x > 0)
                {
                    flyDirection = FlyDirection.right;
                }
            }
        }

        private Vector3 GetFlyDirection()
        {
            switch (flyDirection)
            {
                case FlyDirection.forward:
                    return mesh.transform.forward * flySpeed;
                case FlyDirection.left:
                    return -mesh.transform.right * (flySpeed / 2);
                case FlyDirection.right:
                    return mesh.transform.right * (flySpeed / 2);
                case FlyDirection.back:
                    return -mesh.transform.forward * (flySpeed / 2);
            }

            return Vector3.zero;
        }

        protected override Vector3 GetRotation()
        {
            if (isJumping)
            {
                return mesh.transform.localEulerAngles;
            }

            Vector3 rotation = base.GetRotation();

            if (flyDirection == FlyDirection.forward)
            {
                if (GetSpeed() * flySpeed > 0.1f)
                {
                    rotation.x = cameraHelper.transform.localEulerAngles.x;
                    rotation.z = Mathf.Clamp((-smoothRotVelocity / 9f), -80, 80);
                }
            }

            return rotation;
        }

        protected override float GetYDirection(Vector2 input)
        {
            if (flyDirection != FlyDirection.forward)
            {
                input.x = 0;
                input.y = 1;
            }

            return base.GetYDirection(input);
        }

        private void UpdateJumpSpeed()
        {
            if (jumpSpeed > MIN_JUMP_SPEED)
            {
                jumpSpeed -= DEACCELERATION * Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
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
    }

    public enum FlyDirection
    {
        forward, left, right, back
    }
}
