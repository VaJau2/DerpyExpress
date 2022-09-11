using UnityEngine;

namespace DerpyExpress.Player
{
    //Передвижения игрока
    class PlayerMovingController
    {
        const string CAMERA_NAME = "rotationHelper";
        const string MESH_NAME = "derpy";

        const float ACCELERATION = 4f;
        const float DEACCELERATION = 8f;

        public Animator anim;
        private GameObject cameraHelper;
        private CharacterController controller;
        private GameObject mesh;
        private Player player;
        private float gravity = 6f;
        private float tempSpeed = 0f;

        private float smoothRotVelocity;
        private float smoothRotTime = 0.16f;

        public PlayerMovingController(Player player)
        {
            this.player = player;

            cameraHelper = GetChildObject(player.transform, CAMERA_NAME);
            mesh = GetChildObject(player.transform, MESH_NAME);
            controller = player.GetComponent<CharacterController>();
            anim = mesh.GetComponent<Animator>();
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

        public void UpdateMovement()
        {
            Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            float tempSpeed = GetSpeed(input);
            if (input.magnitude > 0)
            {
                mesh.transform.localEulerAngles = new Vector3(
                    0,
                    Mathf.SmoothDampAngle(
                        mesh.transform.localEulerAngles.y, 
                        GetYDirection(input),
                        ref smoothRotVelocity,
                        smoothRotTime),
                    0
                );
            }
            
            Vector3 dir = mesh.transform.forward * tempSpeed;
            anim.SetFloat("speed", tempSpeed);

            dir.y -= gravity;
            controller.Move(dir * Time.deltaTime);
        }

        private GameObject GetChildObject(Transform transform, string name)
        {
            return transform.Find(name).gameObject;
        }

        private float GetYDirection(Vector2 input)
        {
            float forwardRotation = cameraHelper.transform.localEulerAngles.y;

            if (input.x > 0)
            {
                forwardRotation += GetVerticalRotation(input.y);
            }
            else if (input.x < 0)
            {
                forwardRotation -= GetVerticalRotation(input.y);
            }
            else if (input.y < 0)
            {
                forwardRotation += 180f;
            }

            return forwardRotation;
        }

        private float GetVerticalRotation(float vertical)
        {
            if (vertical == 0)
            {
                return 90f;
            }
            else
            {
                return vertical > 0 ? 45f : 135f;
            }
        }

        private float GetSpeed(Vector2 input)
        {
            float target = GetSpeedTarget(input);

            if (tempSpeed > target + 0.2f)
            {
                tempSpeed -= DEACCELERATION * Time.deltaTime;
            }
            else if(tempSpeed < target - 0.2f)
            {
                tempSpeed += ACCELERATION * Time.deltaTime;
            }
            else 
            {
                tempSpeed = target;
            }

            return tempSpeed;
        }

        private float GetSpeedTarget(Vector2 input)
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
            else
            {
                return player.speed;
            }
        }

        private bool isRunning()
        {
            return !player.isCrouching && Input.GetButton("Run");
        }
    }
}
