using UnityEngine;
using DerpyExpress.Player.Controllers;

namespace DerpyExpress.Player
{
    //Основной класс игрока
    public class Player : MonoBehaviour
    {
        public float gravity = 6f;
        public float crouchSpeed = 1.5f;
        public float speed = 2.5f;
        public float runSpeed = 6.5f;

        public bool isCrouching;
        public bool isMayStopCrouching = true;
        public bool isFlying;
        private BaseController controller;
        private HeadRotation headRotation;
        private float jumpCooldown;

        public void Start() 
        {
            controller = new PlayerMovingController(this);
            headRotation = new HeadRotation(this);
        }

        public void Update() 
        {
            controller.UpdateMovement();
            headRotation.Update();
        }

        public void SetMovementController(BaseController newController)
        {
            controller = newController;
        }
    }
}

