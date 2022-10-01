using UnityEngine;
using DerpyExpress.Player.Controllers;

namespace DerpyExpress.Player
{
    public class HeadRotation
    {
        const float STEP = 0.02f;
        const float ROTATION_SPEED = 6f;

        private Transform camera;
        private Transform player;
        private Animator anim;

        private float horizontalDot, verticalDot;

        private float rotationSpeed => ROTATION_SPEED * Time.deltaTime;

        public HeadRotation(Player playerObj)
        {
            GameObject mesh = playerObj.transform.Find(BaseController.MESH_NAME).gameObject;
            camera = playerObj.transform.Find(BaseController.CAMERA_NAME);
            player = mesh.transform;
            anim = mesh.GetComponent<Animator>();
        }

        public void Update()
        {
            float horizontalTarget = GetVerticalDot();
            float verticalTarget = GetHorizontalDot();

            horizontalDot = BaseController.GetLepredValue(
                horizontalDot, horizontalTarget, 
                STEP, rotationSpeed, rotationSpeed
            );

            verticalDot = BaseController.GetLepredValue(
                verticalDot, verticalTarget, 
                STEP, rotationSpeed, rotationSpeed
            );

            anim.SetFloat("look_vertical", horizontalDot);
            anim.SetFloat("look_horizontal", verticalDot);
        }

        private float GetVerticalDot()
        {
            Vector3 playerPos = player.forward.normalized;
            Vector3 cameraPos = -camera.up.normalized;
            Vector2 verticalPlayer = new Vector2(playerPos.z, playerPos.y);
            Vector2 verticalCamera = new Vector2(cameraPos.z, cameraPos.y);

            return Vector2.Dot(verticalPlayer, verticalCamera);
        }

        private float GetHorizontalDot()
        {
            Vector3 playerPos = player.forward.normalized;
            Vector3 cameraPos = camera.right.normalized;
            if (IsLookForward())
            {
                cameraPos = -cameraPos;
            }

            Vector2 horizontalPlayer = new Vector2(playerPos.z, playerPos.x);
            Vector2 horizontalCamera = new Vector2(cameraPos.z, cameraPos.x);
            
            return Vector2.Dot(horizontalPlayer, horizontalCamera);
        }

        private bool IsLookForward()
        {
            Vector3 playerPos = player.forward.normalized;
            Vector3 cameraPos = camera.forward.normalized;
            float angle = Vector3.Angle(playerPos, cameraPos);
            return angle < 90;
        }
    }
}
