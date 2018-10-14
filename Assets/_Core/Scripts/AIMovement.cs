using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovement : MonoBehaviour {
    public string[] STATES_STRING = {
        "Idle",
        "IDLE_TO_WALK",
        "WALK",
       "WALK_TO_IDLE",
        "WALK_TO_ATTACK",
        "ATTACK",
        "ATTACK_TO_REST",
        "REST"
    };
    const int STATE_IDLE = 0;
    const int STATE_IDLE_TO_WALK = 1;
    const int STATE_WALK = 2;
    const int STATE_WALK_TO_IDLE = 3;
    const int STATE_WALK_TO_ATTACK = 4;
    const int STATE_ATTACK = 5;
    const int STATE_ATTACK_TO_REST = 6;
    const int STATE_REST = 7;

    const float SIGHT_DISTANCE = 10.0f;
    const float ATTACK_DISTANCE = 4.0f;

    const float ATTACK_FORCE = 15.0f;

    const float REST_DELAY = 2.0f;

    public float speed = 2.0f;
    public Transform target;

    private Vector3 direction;
    private int currentState = STATE_IDLE;
    private Animator animator;
    private Vector3 attackDirection;

	// Use this for initialization
	void Start () {
        this.animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        this.HandleBehaviour();
	}

    /// <summary>
    /// Follow
    /// </summary>
    void Follow (){
        this.direction = target.position - transform.position;
        this.direction.Normalize();
        this.Move();
    }

    /// <summary>
    /// Attack
    /// </summary>
    void Attack(){
        transform.position += this.attackDirection * ATTACK_FORCE * Time.deltaTime;
    }

    /// <summary>
    /// 
    /// </summary>
    void StopAttack(){
        this.currentState = STATE_ATTACK_TO_REST;
        //this.animator.SetInteger("currentState", this.currentState);
    }

    /// <summary>
    /// 
    /// </summary>
    void StopRest(){
        this.currentState = STATE_IDLE;
        this.animator.SetInteger("currentState", this.currentState);
    }

    /// <summary>
    /// Move
    /// </summary>
    void Move(){
        transform.position += this.direction * this.speed * Time.deltaTime;
    }

    /// <summary>
    /// 
    /// </summary>
    void HandleBehaviour()
    {
        Debug.ClearDeveloperConsole();
        Debug.Log(this.STATES_STRING[this.currentState]);
        switch (this.currentState)
        {
            case STATE_IDLE:
                if(Vector3.Distance(transform.position, this.target.position) <= SIGHT_DISTANCE){
                    this.currentState = STATE_IDLE_TO_WALK;
                }
                break;

            case STATE_IDLE_TO_WALK:
                this.currentState = STATE_WALK;
                this.animator.SetInteger("currentState", this.currentState);
                break;

            case STATE_WALK:
                if (Vector3.Distance(transform.position, this.target.position) > SIGHT_DISTANCE){
                    this.currentState = STATE_WALK_TO_IDLE;
                }   else{
                    if (Vector3.Distance(transform.position, this.target.position) <= ATTACK_DISTANCE){ 
                        this.currentState = STATE_WALK_TO_ATTACK;
                    }   else{
                        this.Follow();
                    }
                }
                break;

            case STATE_WALK_TO_IDLE:
                this.currentState = STATE_IDLE;
                this.animator.SetInteger("currentState", this.currentState);
                break;

            case STATE_WALK_TO_ATTACK:
                this.attackDirection = target.position - transform.position;
                this.attackDirection.Normalize();
                this.currentState = STATE_ATTACK;
                this.animator.SetInteger("currentState", this.currentState);
                break;

            case STATE_ATTACK:
                this.Attack();
                break;

            case STATE_ATTACK_TO_REST:
                Invoke("StopRest", REST_DELAY);
                this.currentState = STATE_REST;
                this.animator.SetInteger("currentState", this.currentState);
                break;

            case STATE_REST:
                break;
        }
    }
}
