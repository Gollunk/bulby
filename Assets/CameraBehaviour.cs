using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour {

    const float SPEED = 10.0f;

    public Transform target;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        this.LookTarget();
	}

    /// <summary>
    /// 
    /// </summary>
    void LookTarget()
    {
        Vector3 nextPosition = transform.position;
        nextPosition.x = this.target.position.x;
        nextPosition.y = this.target.position.y;
        transform.position = Vector3.Lerp(transform.position, nextPosition, Time.deltaTime * SPEED);
    }
}
