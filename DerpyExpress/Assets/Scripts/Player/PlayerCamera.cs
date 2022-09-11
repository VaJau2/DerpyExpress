using UnityEngine;

namespace DerpyExpress.Player
{
    //Вращение камеры
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private float speedH = 3.0f;
        [SerializeField] private float speedV = 2.5f;

        private float yaw = 0.0f;
        private float pitch = 0.0f;

        public void Update()
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            if (mouseX == 0 && mouseY == 0)
            {
                return;
            }

            yaw += speedH * mouseX;
            pitch -= speedV * mouseY;
            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }
    }
}
