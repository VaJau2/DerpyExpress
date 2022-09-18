using UnityEngine;
using System.Collections.Generic;

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
        private float jumpSpeed;
        private float jumpCooldownTime;
        private float groundedCheckStartCooldown = 0.5f;

        public PlayerMovingController(Player player): base(player) 
        {
            anim.SetBool("jump", false);
            mesh.transform.localEulerAngles = GetRotation();
        }

        public void UpdateCrouching(bool on)
        {
            anim.SetBool("crouch", on);
            if (on)
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

        public override void UpdateMovement()
        {
            if (jumpCooldownTime > 0)
            {
                jumpCooldownTime -= Time.deltaTime;
                return;
            }

            if (Input.GetButtonDown("Jump") && !isRunning())
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
                if (Input.GetButtonDown("Crouch"))
                {
                    player.isCrouching = !player.isCrouching;
                    UpdateCrouching(player.isCrouching);
                }

                dir.y -= player.gravity;
            }

            if (GetInput().magnitude > 0)
            {
                mesh.transform.localEulerAngles = GetRotation();
            }

            bool isGrounded = GetIsGrounded();
           
            if (!isGrounded && controller.isGrounded)
            {
                jumpCooldownTime = LAND_TIME;
            }

            anim.SetBool("on_floor", isGrounded);
            anim.SetFloat("speed", isGrounded ? tempSpeed : 0);

            controller.Move(dir * Time.deltaTime);
        }

        private bool GetIsGrounded()
        {
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
            }
        }
    }
}
