using UnityEngine;

namespace DerpyExpress.Player
{
    //Основной класс игрока
    public class Player : MonoBehaviour
    {
        public float gravity = 6f;
        public float speed = 3f;
        public float runSpeed = 6f;
        
        private PlayerMovingController controller;

        public void Start() 
        {
            controller = new PlayerMovingController(this);
        }

        public void Update() 
        {
            controller.UpdateMovement();
        }
    }
}

