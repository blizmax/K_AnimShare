using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class Demo02_root : MonoBehaviour
{
    private Animator anim;

    private bool moving = false;
    private bool shift = false;

    private Vector2 inputVec = Vector2.zero;
    private Vector3 moveDir= Vector3.zero;

    private float currentAnimValue;
    void Start()
    {
        anim = GetComponent<Animator>();

        InputEvent.Instance.moveStarted += (Vector2) =>
        {
            moving = true;
        };

        InputEvent.Instance.movePerformed += (Vector2) =>
        {
            inputVec = Vector2;
        };;

        InputEvent.Instance.moveCanceled += (Vector2) =>
        {
            moving = false;

        };

        InputEvent.Instance.shiftStarted += (Vector2) =>
        {
            shift = true;
        };

        InputEvent.Instance.shiftPerformed += (Vector2) => { };

        InputEvent.Instance.shiftCanceled += (Vector2) =>
        {
            shift = false;
        };
    }
    
    
    void Update()
    {
        if (moving)
        {
            RotateAndMove(inputVec);
        }

        float targetValue = moving ? (shift ? 2 : 1) : 0;
        //float currentAnimValue = anim.GetFloat("move");
        currentAnimValue = Mathf.Lerp(currentAnimValue , targetValue, 0.1f);
        anim.SetFloat("move" , currentAnimValue);


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
        //rig.velocity = new Vector3(targetDir.x, rig.velocity.y, targetDir.z) * (shift ? runSpeed : walkSpeed);
    }
}
