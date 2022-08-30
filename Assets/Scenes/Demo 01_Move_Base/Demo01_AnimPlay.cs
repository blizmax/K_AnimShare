using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class Demo01_AnimPlay : MonoBehaviour
{
    private Animator anim;
    private Rigidbody rig;
    
    private bool moving = false;
    private bool shift = false;

    private Vector2 inputVec = Vector2.zero;
    private Vector3 moveDir = Vector3.zero;

    //public float RotateSpeed = 10;
    public AnimatorController Controller;
    public float walkSpeed = 1;
    public float runSpeed = 2;
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.runtimeAnimatorController = Controller;
        rig = GetComponent<Rigidbody>();

        InputEvent.Instance.moveStarted += (Vector2) =>
        {
            moving = true;
            anim.Play(shift ? "run" : "walk");
            //anim.CrossFade(shift?"run":"walk" , 0.1f);
        };

        InputEvent.Instance.movePerformed += (Vector2) => { inputVec = Vector2; };
        ;

        InputEvent.Instance.moveCanceled += (Vector2) =>
        {
            moving = false;
            anim.Play("idle");
            //anim.CrossFade("idle", 0.2f);
        };

        InputEvent.Instance.shiftStarted += (Vector2) =>
        {
            shift = true;
            if (moving)
                anim.Play("run");
            //anim.CrossFade("run", 0.2f);
        };

        InputEvent.Instance.shiftPerformed += (Vector2) => { };

        InputEvent.Instance.shiftCanceled += (Vector2) =>
        {
            shift = false;

            if (moving)
                anim.Play("walk");
            //anim.CrossFade("walk", 0.2f);
        };
    }


    void Update()
    {
        if (moving)
        {
            RotateAndMove(inputVec);
        }
        
        
    }

    void RotateAndMove(Vector2 target)
    {
        // moveDir.x = target.x;
        // moveDir.z = target.y;
        // Quaternion targetDir = Quaternion.LookRotation(moveDir , Vector3.up);
        // transform.rotation = Quaternion.RotateTowards(transform.rotation, targetDir, RotateSpeed * Time.deltaTime);


        float h = target.x;
        float v = target.y;
        Vector3 velocity = new Vector3(h, 0, v);
        float camY = Camera.main.transform.rotation.eulerAngles.y;
        Vector3 targetDir = Quaternion.Euler(0, camY, 0) * velocity;
        transform.forward = targetDir;
        rig.velocity = new Vector3(targetDir.x, rig.velocity.y, targetDir.z) * (shift ? runSpeed : walkSpeed);
    }

}