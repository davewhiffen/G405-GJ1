using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour

{
    public AudioSource tickSource;

    Animator anim;




    // Start is called before the first frame update
    void Start()
    {
        tickSource = GetComponent<AudioSource>();


        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")


        {
            anim.SetTrigger("OpenDoor");
            tickSource.Play();

        }
    }


    
    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            anim.enabled = true;
            anim.SetTrigger("CloseDoor");
        }

    }
    

    void pauseAnimationEvent()
    {
           anim.enabled = false;  
    }


}
