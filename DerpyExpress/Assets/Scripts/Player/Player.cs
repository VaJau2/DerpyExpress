using UnityEngine;

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
        
        private PlayerMovingController controller;

        public void Start() 
        {
            controller = new PlayerMovingController(this);
        }

        public void Update() 
        {
            controller.UpdateMovement();
            UpdateCrouching();
        }

        private void UpdateCrouching()
        {
            if (!Input.GetButtonDown("Crouch"))
            {
                return;
            }

            isCrouching = !isCrouching;
            controller.UpdateCrouching(isCrouching);
        }
    }
}

