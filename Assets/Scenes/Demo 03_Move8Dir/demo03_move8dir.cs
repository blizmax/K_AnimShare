using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class demo03_move8dir : MonoBehaviour
{
    private Animator anim;
    private bool moving = false;
    private bool shift = false;

    private Vector2 inputVec = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        InputEvent.Instance.moveStarted += (Vector2) =>
        {
            moving = true;
            
            anim.SetBool("move", moving);
        };

        InputEvent.Instance.movePerformed += (Vector2) => { inputVec = Vector2; };
        ;

        InputEvent.Instance.moveCanceled += (Vector2) =>
        {
            moving = false;
            anim.SetBool("move", moving);
        };

        InputEvent.Instance.shiftStarted += (Vector2) => { shift = true; };

        InputEvent.Instance.shiftPerformed += (Vector2) => { };

        InputEvent.Instance.shiftCanceled += (Vector2) => { shift = false; };
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            Vector2 v2 = inputVec * (shift ? 2 : 1);
            float x = Mathf.Lerp(anim.GetFloat("inputX"), v2.x,
                2 * Time.deltaTime);
            anim.SetFloat("inputX", x);
            
            float y = Mathf.Lerp(anim.GetFloat("inputY"), v2.y,
                2 * Time.deltaTime);
            anim.SetFloat("inputY", y);
        }


        // float targartAngles = MoveStartAngle(inputVec);
        // float angles = Mathf.Lerp(anim.GetFloat("inputAngle"), targartAngles,
        //     2 * Time.deltaTime);
        // anim.SetFloat("inputAngle" , angles);
        //
        // //走跑切换
        // float targetMoveY = shift?1f:0.7f;
        // if (Mathf.Abs(targetMoveY - anim.GetFloat("InputMagnitude"))>0.001f)
        // {
        //     float moveY = Mathf.Lerp(anim.GetFloat("InputMagnitude"), targetMoveY,
        //         2 * Time.deltaTime);
        //     anim.SetFloat("InputMagnitude",moveY);
        // }
    }


   
}