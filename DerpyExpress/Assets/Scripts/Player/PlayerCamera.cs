using UnityEngine;
using System.Collections;

namespace DerpyExpress.Player
{
    //Вращение камеры
    //Приближение камеры при коллизиях со стенами
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private Transform headBone;
        [SerializeField] private Transform cameraCollider;

        [SerializeField] private Transform rotationHelper;
        [SerializeField] private float speedH = 3.0f;
        [SerializeField] private float speedV = 2.5f;
        [SerializeField] private float maxPitch = 50f;
        [SerializeField] private float minPitch = -25f;

        private float yaw = 0.0f;
        private float pitch = 0.0f;

        private Coroutine cameraMoving;

        public void ChangePosition(bool close)
        {
            if (cameraMoving != null)
            {
                StopCoroutine(cameraMoving);
            }

            cameraMoving = StartCoroutine(MoveCamera(close));
        }

        IEnumerator MoveCamera(bool close)
        {
            Vector3 target = GetTarget(close);
            float distance = GetDistance(target);

            while (distance > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, 5f * Time.deltaTime);
                target = GetTarget(close);
                distance = GetDistance(target);
                yield return null;
            }
        }

        private Vector3 GetTarget(bool close ) => close ? headBone.position : cameraCollider.position;
        private float GetDistance(Vector3 target) => Vector3.Distance(transform.position, target);

        public void Update()
        {
            if (Time.timeScale == 0)
            {
                return;
            }
            
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            if (mouseX == 0 && mouseY == 0)
            {
                return;
            }

            yaw += speedH * mouseX;
            pitch -= speedV * mouseY;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            rotationHelper.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }
    }
}
