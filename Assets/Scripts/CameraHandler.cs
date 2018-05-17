using UnityEngine;
using System.Collections;

public class CameraHandler : MonoBehaviour {

    [Range(0, 1)]
    public float dampTime = 0.10f;
    private Vector3 velocity = Vector3.zero;
    public Transform target;
    GameMap gameMap;

    void Start()
    {
        gameMap = GameObject.Find("GameMap").GetComponent<GameMap>();
        target = gameMap.playerGameObject.transform;
    }

    void FixedUpdate()
    {
        if (target)
        {
            Vector3 point = GetComponent<Camera>().WorldToViewportPoint(target.position);
            Vector3 delta = target.position - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(.5F, .5F, point.z));
            Vector3 destination = transform.position + delta;
            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
        }

    }
}
