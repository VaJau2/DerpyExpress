using UnityEngine;

namespace DerpyExpress.Player.Controllers
{
    class PlayerMovingController : BaseController
    {
        private const float JUMP_START_TIME = 0.5f;
        private const float LAND_TIME = 0.5f;
        private const float JUMP_SPEED = 4f;
        private const float MIN_JUMP_SPEED = 1f;
        private const float DEACCELERATION = 5f;
        private bool isJumping;
        private bool isLanding;
        private float jumpSpeed;
        private float jumpCooldownTime;
        private float groundedCheckStartCooldown = 0.5f;

        RaycastHit hit;
        public PlayerMovingController(Player player): base(player) 
        {
            mesh.transform.localEulerAngles = GetRotation();
        }

        public void UpdateCrouching()
        {
            if (Input.GetButtonDown("Crouch"))
            {
                if (player.isCrouching)
                {
                    Vector3 raycastFrom = mesh.transform.position;
                    raycastFrom.y += 0.3f;
                    bool isCollidingTop = Physics.Raycast(raycastFrom, Vector3.up, out hit, 4f);
                    if (isCollidingTop)
                    {
                        return;
                    }
                }

                bool makeCrouching = !player.isCrouching;
                player.isCrouching = makeCrouching;
                
                anim.SetBool("crouch", makeCrouching);
                if (makeCrouching)
                {
                    controller.height = 0;
                    controller.center = new Vector3(0, 0.15f, 0);
                }
                else
                {
                    controller.height = 0.47f;
                    controller.center = new Vector3(0, 0.25f, 0);
                }
            }
        }

        public override void UpdateMovement()
        {
            if (jumpCooldownTime > 0)
            {
                jumpCooldownTime -= Time.deltaTime;
                return;
            }

            if (Input.GetButtonDown("Jump") && !player.isCrouching && !isRunning())
            {
                anim.SetBool("jump", true);

                if (isJumping && !controller.isGrounded)
                {
                    player.SetMovementController(new PlayerFlyingController(player));
                }
                else
                {
                    isJumping = true;
                    jumpSpeed = JUMP_SPEED;
                    jumpCooldownTime = JUMP_START_TIME;
                }
                
                return;
            }

            float tempSpeed = GetSpeed();
            Vector3 dir = mesh.transform.forward * tempSpeed;

            if (isJumping)
            {
                UpdateJumpSpeed();
                dir.y = jumpSpeed;
            }
            else
            {
                UpdateCrouching();

                dir.y -= player.gravity;
            }

            if (GetInput().magnitude > 0)
            {
                mesh.transform.localEulerAngles = GetRotation();
            }

            bool isGrounded = GetIsGrounded();

            if (isLanding && isGrounded)
            {
                isLanding = false;
                jumpCooldownTime = LAND_TIME;
            }

            anim.SetBool("on_floor", isGrounded);
            anim.SetBool("jump", false);
            anim.SetFloat("speed", isGrounded ? tempSpeed : 0);

            controller.Move(dir * Time.deltaTime);
        }

        private bool GetIsGrounded()
        {
            if (Time.timeScale == 0)
            {
                groundedCheckStartCooldown = 1;
                return true;
            }

            if (groundedCheckStartCooldown > 0)
            {
                groundedCheckStartCooldown -= Time.deltaTime;
                return true;
            }

            return controller.isGrounded;
        }

        protected override float GetSpeedTarget(Vector2 input)
        {
            if (input.magnitude == 0) 
            {
                return 0;
            }
            else if (player.isCrouching)
            {
                return player.crouchSpeed;
            }
            else if (isRunning())
            {
                return player.runSpeed;
            }

            return base.GetSpeedTarget(input);
        }

        private bool isRunning()
        {
            return !player.isCrouching && Input.GetButton("Run");
        }

        private void UpdateJumpSpeed()
        {
            if (jumpSpeed > MIN_JUMP_SPEED)
            {
                jumpSpeed -= DEACCELERATION * Time.deltaTime;
            }
            else
            {
                anim.SetBool("jump", false);
                isJumping = false;
                isLanding = true;
            }
        }
    }
}
