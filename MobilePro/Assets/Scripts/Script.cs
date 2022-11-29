using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script : MonoBehaviour
{


    public float speed;
    float hAxis;
    float vAxis;
    bool wDown;
    bool jDown;

    bool isJump;
    bool isDodge;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Animator anime;
    Rigidbody rigid;



    // Start is called before the first frame update
    void Start()
    {
        anime = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
        GetInput();
        Move();
        Turn();
        Jump();
        Dodge();
    }// update

    void GetInput(){
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");

    }//getinput

    void Move(){

        moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        if(isDodge){
            moveVec = dodgeVec;
        }

        //walk or run
        if(wDown){
            transform.position += moveVec * speed * 0.3f * Time.deltaTime;
        }else{
            transform.position += moveVec * speed * Time.deltaTime;
        }

        // anime
        anime.SetBool("isRun", moveVec != Vector3.zero);
        anime.SetBool("isWalk", wDown);


    }//move

    void Turn(){

        transform.LookAt(transform.position + moveVec);

    }//turn

    void Jump(){

        if(jDown && moveVec == Vector3.zero && !isJump && !isDodge ){
            rigid.AddForce(Vector3.up * 15, ForceMode.Impulse);
            anime.SetBool("isJump", true);
            anime.SetTrigger("doJump");
            isJump = true;
        }

    }//jump

    void Dodge(){ // 회피 

        if(jDown && moveVec != Vector3.zero && !isJump && !isDodge ){
            dodgeVec = moveVec;
            speed *= 2;
            anime.SetTrigger("doDodge");
            isDodge = true;

            Invoke("DodgeOut", 0.4f);
        }

    }//dodge

    void DodgeOut(){
        isDodge = false;
        speed *= 0.5f;
    }


    void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.tag == "Floor"){
            anime.SetBool("isJump", false);
            isJump = false;
        }
    }

}
