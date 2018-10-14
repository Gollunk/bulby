using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float speed;

    private Vector2 direction;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        this.GetInput();
        this.Move();
	}

    /// <summary>
    /// GetInput
    /// </summary>
    void GetInput()
    {
        this.direction = Vector2.zero;

        if (Input.GetKey(KeyCode.Z)){
            this.direction += Vector2.up * this.speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S)){
            this.direction += Vector2.down * this.speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Q)){
            this.direction += Vector2.left * this.speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D)){
            this.direction += Vector2.right * this.speed * Time.deltaTime;
        }

    }

    /// <summary>
    /// Move
    /// </summary>
    void Move()
    {
        transform.Translate(this.direction);
    }
}
