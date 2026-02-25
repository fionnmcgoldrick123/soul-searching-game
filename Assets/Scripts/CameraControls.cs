using UnityEngine;

public class CameraControls : MonoBehaviour
{

    [SerializeField] private Transform player;

    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        cam.transform.position = new Vector3(player.position.x, player.position.y, -10f);
    }
}
