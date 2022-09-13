using UnityEngine;

namespace DerpyExpress.Player.Controllers
{
    public abstract class BaseController
    {
        const string CAMERA_NAME = "rotationHelper";
        const string MESH_NAME = "derpy";

        public Animator anim;
        protected GameObject cameraHelper;
        protected CharacterController controller;
        protected GameObject mesh;
        protected Player player;

        private float tempSpeed = 0f;
        protected float acceleration = 4f;
        protected float deacceleration = 8f;

        protected float smoothRotVelocity;
        private float smoothRotTime = 0.16f;

        public BaseController(Player player)
        {
            this.player = player;

            cameraHelper = GetChildObject(player.transform, CAMERA_NAME);
            mesh = GetChildObject(player.transform, MESH_NAME);
            controller = player.GetComponent<CharacterController>();
            anim = mesh.GetComponent<Animator>();
        }

        public abstract void UpdateMovement();

        protected GameObject GetChildObject(Transform transform, string name)
        {
            return transform.Find(name).gameObject;
        }

        protected float GetLepredValue(float value, float needValue, float step, float acceleration, float deacceleration)
        {
            if (value > needValue + step)
            {
                value -= deacceleration;
            }
            else if(value < needValue - step)
            {
                value += acceleration;
            }
            else 
            {
                value = needValue;
            }

            return value;
        }

        protected virtual Vector3 GetRotation()
        {
            Vector2 input = GetInput();
            return new Vector3(
                0,
                Mathf.SmoothDampAngle(
                    mesh.transform.localEulerAngles.y, 
                    GetYDirection(input),
                    ref smoothRotVelocity,
                    smoothRotTime),
                0
            );
        }

        protected virtual float GetYDirection(Vector2 input)
        {
            float forwardRotation = cameraHelper.transform.localEulerAngles.y;

            if (input.x > 0)
            {
                forwardRotation += GetYRotation(input.y);
            }
            else if (input.x < 0)
            {
                forwardRotation -= GetYRotation(input.y);
            }
            else if (input.y < 0)
            {
                forwardRotation += 180f;
            }

            return forwardRotation;
        }

        private float GetYRotation(float vertical)
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

        protected float GetSpeed()
        {
            Vector2 input = GetInput();
            float target = GetSpeedTarget(input);

            tempSpeed = GetLepredValue(
                tempSpeed, target, 0.2f, 
                acceleration * Time.deltaTime, 
                deacceleration * Time.deltaTime
            );

            return tempSpeed;
        }

        protected virtual float GetSpeedTarget(Vector2 input)
        {
            if (input.magnitude == 0) 
            {
                return 0;
            }

            return player.speed;
        }

        protected Vector2 GetInput()
        {
            return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }
    }
}
