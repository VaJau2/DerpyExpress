using UnityEngine;

namespace DerpyExpress.Player
{
    //Передвижения игрока
    class PlayerMovingController
    {
        const string CAMERA_NAME = "rotationHelper";
        const string MESH_NAME = "derpy";

        public bool isRunning;

        public Animator anim;
        private GameObject cameraHelper;
        private CharacterController controller;
        private GameObject mesh;

        private float smoothRotVelocity;
        private float smoothRotTime = 0.05f;
        

        private float gravity = 6f;
        private float speed = 3f;
        private float runSpeed = 6f;

        public PlayerMovingController(Player player)
        {
            gravity = player.gravity;
            speed = player.speed;
            runSpeed = player.runSpeed;

            cameraHelper = GetChildObject(player.transform, CAMERA_NAME);
            mesh = GetChildObject(player.transform, MESH_NAME);
            controller = player.GetComponent<CharacterController>();
            anim = mesh.GetComponent<Animator>();
        }

        public void UpdateMovement()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            float tempSpeed = GetSpeed(horizontal, vertical);
            if (tempSpeed > 0)
            {
                mesh.transform.localEulerAngles = new Vector3(
                    0,
                    Mathf.SmoothDampAngle(
                        mesh.transform.localEulerAngles.y, 
                        GetYDirection(horizontal, vertical),
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

        private float GetYDirection(float horizonal, float vertical)
        {
            float forwardRotation = cameraHelper.transform.localEulerAngles.y;

            if (horizonal > 0)
            {
                forwardRotation += GetVerticalRotation(vertical);
            }
            else if (horizonal < 0)
            {
                forwardRotation -= GetVerticalRotation(vertical);
            }
            else if (vertical < 0)
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

        private float GetSpeed(float horizonal, float vertical)
        {
            if (horizonal == 0 && vertical == 0) 
            {
                isRunning = false;
                return 0;
            }

            if (!isRunning)
            {
                isRunning = Input.GetAxis("Run") > 0;
            }

            return isRunning ? runSpeed : speed;
        }
    }
}
