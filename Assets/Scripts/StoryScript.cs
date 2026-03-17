using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StoryScript : MonoBehaviour
{
    public Transform head;
    public float spawnDistance = 2;
    public GameObject canvas;

    void Update()
    {
        canvas.transform.position = head.position + new Vector3(head.forward.x,0,head.forward.z).normalized * spawnDistance;

        canvas.transform.LookAt(new Vector3 (head.position.x, canvas.transform.position.y, head.position.z));
        canvas.transform.forward *= -1;
    }
}
