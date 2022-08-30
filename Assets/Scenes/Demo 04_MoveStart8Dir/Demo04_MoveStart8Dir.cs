using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo04_MoveStart8Dir : MonoBehaviour
{
    private Animator anim;
    private bool moving = false;
    private bool shift = false;
    private float shiftValue = 0f;

    private Vector2 inputVec = Vector2.zero;

    private bool inputCheck = false;

    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        InputEvent.Instance.moveStarted += (Vector2) =>
        {
            moving = true;
            StartCoroutine(Movestart(0.3f));
            
        };

        InputEvent.Instance.movePerformed += (Vector2) =>
        {
            inputVec = Vector2;
            float inputMagnitude = inputVec.SqrMagnitude();
            anim.SetFloat("inptMagnitude", inputMagnitude + shiftValue);
        };

        InputEvent.Instance.moveCanceled += (Vector2) =>
        {
            moving = false;
            anim.SetFloat("inptMagnitude", 0);
        };

        InputEvent.Instance.shiftStarted += (f) =>
        {
            shift = true;
            shiftValue = f;
            if (moving)
            {
                anim.SetFloat("inptMagnitude", 1);
            }
        };

        InputEvent.Instance.shiftPerformed += (f) =>
        {
            shiftValue = f;
        };

        InputEvent.Instance.shiftCanceled += (f) =>
        {
            shiftValue = f;
            shift = false;
        };
    }

    IEnumerator Movestart(float time)
    {
        yield return new WaitForSeconds(time);
        float angle = MoveStartAngle(inputVec);
        anim.SetFloat("startAngle", angle);
    }
    float MoveStartAngle(Vector2 moveAxis)
    {
        Vector3 velocity = new Vector3(moveAxis.x, 0, moveAxis.y);
        float camY = Camera.main.transform.rotation.eulerAngles.y;
        Vector3 targetDir = Quaternion.Euler(0, camY, 0) * velocity;
        float dot = Vector3.Dot(transform.right, targetDir);
        float angle = Vector3.Angle(transform.forward, targetDir);
        int dirValue = dot >= 0 ? 1 : -1;
        return angle * dirValue;
    }
}