using UnityEngine;
using DerpyExpress.Player.Controllers;

namespace DerpyExpress.Player
{
    //Основной класс игрока
    public class Player : MonoBehaviour
    {
        public float gravity = 6f;
        public float jumpSpeed = 6f;
        public float crouchSpeed = 1.5f;
        public float speed = 2.5f;
        public float runSpeed = 6.5f;
        public float flySpeed = 10f;

        public bool isCrouching;
        public bool isFlying;
        private BaseController controller;
        private float jumpCooldown;

        public void Start() 
        {
            controller = new PlayerMovingController(this);
        }

        public void Update() 
        {
            controller.UpdateMovement();
        }

        public void SetMovementController(BaseController newController)
        {
            controller = newController;
        }
    }
}

