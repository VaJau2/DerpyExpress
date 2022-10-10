using UnityEngine;

public class PlayerFace : MonoBehaviour
{
    [SerializeField] private Texture openEyes;
    [SerializeField] private Texture closedEyes;

    const float OPEN_TIME_MAX = 2f;
    const float OPEN_TIME_MIN = 1f;
    const float CLOSED_TIME = 0.2f;

    private SkinnedMeshRenderer mesh;
    private float timer;
    private bool closed;

    private void Start()
    {
        mesh = GetComponent<SkinnedMeshRenderer>();
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            closed = !closed;
            timer = closed ? CLOSED_TIME : Random.Range(OPEN_TIME_MIN, OPEN_TIME_MAX);
            Texture newTexture = closed ? closedEyes : openEyes;
            mesh.materials[0].mainTexture = newTexture;
        }
    }
}
