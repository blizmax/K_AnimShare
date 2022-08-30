using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class demo05 : MonoBehaviour
{
    private Animator anim;

    public Transform target;
    public float weight;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void OnAnimatorIK()
    {
        anim.SetLookAtPosition(target.position);
        anim.SetLookAtWeight(weight);
    }
    
    
}
