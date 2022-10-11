using UnityEngine;

namespace DerpyExpress.Interface
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] public GameObject menu;
        private bool paused;

        public void SetPause(bool pause)
        {
            paused = pause;
            Time.timeScale = pause ? 0 : 1;
            menu.SetActive(pause);
            Cursor.lockState = pause ? CursorLockMode.None : CursorLockMode.Locked;
        }

        public void Exit()
        {
            Application.Quit();
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update() 
        {
            if (Input.GetButtonDown("Cancel"))
            {
                SetPause(!paused);
            }
        }
    }
}
