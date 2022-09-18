using UnityEngine;
using DerpyExpress.Player.Controllers;

namespace DerpyExpress.Player
{
    public class HeadRotation
    {
        private Transform camera;
        private Transform player;
        private Animator anim;

        public HeadRotation(Player playerObj)
        {
            player = playerObj.transform;
            camera = playerObj.transform.Find(BaseController.CAMERA_NAME);
            GameObject mesh = playerObj.transform.Find(BaseController.MESH_NAME).gameObject;
            anim = mesh.GetComponent<Animator>();
        }

        public void Update()
        {
            //TODO запрогать определение поворота головы
        }
    }
}
