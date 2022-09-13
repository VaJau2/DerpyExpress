using UnityEngine;
using System.Collections.Generic;

namespace DerpyExpress.Player.Controllers
{
    class PlayerMovingController : BaseController
    {
        private bool isJumping;

        private float jumpCooldown = 0f;

        public PlayerMovingController(Player player): base(player) 
        {
            jumpCooldown = 1f;
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
            UpdateJumpCooldown();

            if (Input.GetButtonDown("Jump") && jumpCooldown < 0 && !isRunning())
            {
                player.SetMovementController(new PlayerFlyingController(player));
                return;
            }

            if (Input.GetButtonDown("Crouch"))
            {
                player.isCrouching = !player.isCrouching;
                UpdateCrouching(player.isCrouching);
            }

            float tempSpeed = GetSpeed();

            if (GetInput().magnitude > 0)
            {
                mesh.transform.localEulerAngles = GetRotation();
            }
            
            Vector3 dir = mesh.transform.forward * tempSpeed;
            anim.SetFloat("speed", tempSpeed);

            dir.y -= player.gravity;
            controller.Move(dir * Time.deltaTime);
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

        private void UpdateJumpCooldown()
        {
            if (jumpCooldown > 0)
            {
                jumpCooldown -= Time.deltaTime;
            }
        }
    }
}
