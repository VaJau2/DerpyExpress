using UnityEngine;

namespace DerpyExpress.Player
{
    public class CameraCollider : MonoBehaviour
    {
        [SerializeField] private PlayerCamera playerCamera;
    
        private void OnTriggerEnter(Collider other)
        {
            playerCamera.ChangePosition(true);
        }

        private void OnTriggerExit(Collider other)
        {
            playerCamera.ChangePosition(false);
        }
    }
}