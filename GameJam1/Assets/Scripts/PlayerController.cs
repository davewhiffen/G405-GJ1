using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerController : MonoBehaviour
{
    //public
    public int CharacterNumber { get; private set; }
    public int PlayerNumber { get; private set; }
    [SerializeField] PlayerNum playerNum;
    public int stamina { get; set; }
    public int MaxStamina { get; set; }
    public Material[] CharactersMaterial;
    public bool Hiding { get; set; }
    public bool Interacting { get; set; }
    [HideInInspector]public bool completedObjective = false;
    [HideInInspector]public bool doingObjective = false;


    //private
    float MoveSpeed;
    float OriginalSpeed;
    float RunSpeed;


    bool start;
    bool running;
    bool rotating;
    bool CharacterStop;
    bool CharacterEffect;

    GameObject HideableObject;
    Player RewiredPlayer;
    Vector3 moveDirection;
    Vector3 RotateDirection;
    Vector3 OriginalPosition;
    Rigidbody rBody;


    enum PlayerNum
    {
        P1,
        P2,
        P3,
        P4
    }
    PlayerController[] players;

    // Start is called before the first frame update
    void Start()
    {
        if (!start)
        {
            for (int i = 0; i < CharactersMaterial.Length; i++)
            {
                CharactersMaterial[i].color = Color.white;
            }
        }
        rBody = GetComponent<Rigidbody>();
        players = FindObjectsOfType<PlayerController>();


        //set rewired controller
        switch (playerNum)
        {
            case PlayerNum.P1:
                PlayerNumber = 0;
                break;
            case PlayerNum.P2:
                PlayerNumber = 1;
                break;
            case PlayerNum.P3:
                PlayerNumber = 2;
                break;
            case PlayerNum.P4:
                PlayerNumber = 3;
                break;
            default:
                break;
        }
        RewiredPlayer = ReInput.players.GetPlayer(PlayerNumber);
        CharacterNumber = Random.Range(0, 4);

    }

    // Update is called once per frame
    void Update()
    {
        //give character random number
        if (!start)
        {
            foreach (PlayerController pl in players)
            {
                if (pl.gameObject.layer != gameObject.layer)
                {
                    if (CharacterNumber == pl.CharacterNumber)
                    {
                        CharacterNumber = Random.Range(0, 4);
                    }
                }
            }
            SetCharacter();
            //set basic player info
            switch (CharacterNumber)
            {
                //normal character 
                case 0:
                    MoveSpeed = 5;
                    stamina = 100;
                    MaxStamina = 100;
                    RunSpeed = 10;
                    break;
                //slower character but run longer
                case 1:
                    MoveSpeed = 2.5f;
                    stamina = 200;
                    MaxStamina = 200;
                    RunSpeed = 10;
                    break;
                //character regain stamina faster
                case 2:
                    MoveSpeed = 5;
                    stamina = 100;
                    MaxStamina = 100;
                    RunSpeed = 10;
                    break;
                //character run faster
                case 3:
                    MoveSpeed = 5;
                    stamina = 100;
                    MaxStamina = 100;
                    RunSpeed = 15;
                    break;
            }
            OriginalSpeed = MoveSpeed;
            StartCoroutine("CharacterSeted");
        }
        //
        InputHandle();
        if (Hiding) {
            StartCoroutine("ExitHiding");
        }
        //character effects and stats
        if (!CharacterEffect)
        {
            switch (CharacterNumber)
            {
                case 0:

                    break;
                case 1:

                    break;
                //character regain stamina faster
                case 2:
                    StartCoroutine("SmokerCharacter");
                    break;
                //character run faster
                case 3:
                    StartCoroutine("LimpingCharacter");
                    break;
            }
        }
    }
    //move player
    void FixedUpdate()
    {
        rBody.velocity = moveDirection;

        rBody.MoveRotation(rBody.rotation * Quaternion.Euler(RotateDirection * Time.deltaTime));
    }

    void SetCharacter() {
        //set material and player color 
        foreach (Transform g in transform.GetComponentsInChildren<Transform>())
        {
            if (g.GetComponent<Renderer>() != null)
            {
                switch (CharacterNumber)
                {
                    case 0:
                        g.GetComponent<Renderer>().material = CharactersMaterial[0];
                        break;
                    case 1:
                        g.GetComponent<Renderer>().material = CharactersMaterial[1];
                        break;
                    case 2:
                        g.GetComponent<Renderer>().material = CharactersMaterial[2];
                        break;
                    case 3:
                        g.GetComponent<Renderer>().material = CharactersMaterial[3];
                        break;
                }

                switch (playerNum)
                {
                    case PlayerNum.P1:
                        g.GetComponent<Renderer>().material.color = Color.yellow;
                        break;
                    case PlayerNum.P2:
                        g.GetComponent<Renderer>().material.color = Color.green;
                        break;
                    case PlayerNum.P3:
                        g.GetComponent<Renderer>().material.color = Color.blue;
                        break;
                    case PlayerNum.P4:
                        g.GetComponent<Renderer>().material.color = Color.red;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    IEnumerator CharacterSeted() {
        yield return new WaitForSeconds(0.3f);
        start = true;
        StopAllCoroutines();
    }

    void InputHandle() {
        //basic movement
        if (!CharacterStop)
        {
            moveDirection.x = RewiredPlayer.GetAxisRaw("horizontal");
            moveDirection.z = RewiredPlayer.GetAxisRaw("vertical");
            moveDirection = moveDirection.normalized * MoveSpeed;
        }
        //rigt joystick to rotate 1st method
        if (rotating)
        {
            //RotateDirection.x = RewiredPlayer.GetAxisRaw("RotateHorizontal");
            //RotateDirection.z = RewiredPlayer.GetAxisRaw("RotateVertical");
            //RotateDirection = RotateDirection.normalized * 180;

            //old 
            RotateDirection.y = RewiredPlayer.GetAxisRaw("RotateHorizontal");
            RotateDirection = RotateDirection.normalized * 180;
        }

        //check if right stick is moving if it is then 2nd method won't rotate
        if (RewiredPlayer.GetAxis("RotateHorizontal") != 0)
        {
            rotating = true;
        }
        else {
            rotating = false;
        }

        //rotate base on right joystick as you move 2nd method
        //mike's version
        //if (rotating)
        //{
        //    Quaternion rotation = Quaternion.LookRotation(new Vector3(RotateDirection.x, RotateDirection.y, RotateDirection.z));
        //    transform.rotation = rotation;
        //}
        //old 
        if (!rotating)
        {
            Quaternion rotation = Quaternion.LookRotation(new Vector3(moveDirection.x, RotateDirection.y, moveDirection.z));
            transform.rotation = rotation;
        }


        //Character running
        if (RewiredPlayer.GetButtonLongPress("Run") && stamina > 0&&!Hiding)
        {
            running = true;
            CharacterEffect = true;
            if (running)
            {
                MoveSpeed = RunSpeed;
                stamina--;
            }
            if (stamina <= 0)
            {
                stamina = 0;
                MoveSpeed = OriginalSpeed;
                running = false;
                CharacterEffect = false;
            }
        }
        else if (RewiredPlayer.GetButtonUp("Run"))
        {
            running = false;
            CharacterEffect = false;
        }
        //check if its smoker if it is then stamina plus 2 instead of one
        if (CharacterNumber == 2)
        {
            if (!running)
            {
                MoveSpeed = OriginalSpeed;
                stamina += 2;
                if (stamina >= MaxStamina)
                    stamina = MaxStamina;
            }
        }
        else {
            if (!running)
            {
                MoveSpeed = OriginalSpeed;
                stamina ++;
                if (stamina >= MaxStamina)
                    stamina = MaxStamina;
            }
        }

        // hiding 

        if (RewiredPlayer.GetButtonDown("Interact") && HideableObject != null && !Hiding&& HideableObject.GetComponent<HideObjects>().NumberOfPlayers==0)
        {
            OriginalPosition = transform.position;
            HideableObject.GetComponent<BoxCollider>().isTrigger = true;
            HideableObject.GetComponent<HideObjects>().NumberOfPlayers = 1;
            CharacterStop = true;
            transform.position = HideableObject.gameObject.transform.position;
            rBody.isKinematic = true;
            Hiding = true;
        }

        //Pressing the interact button
        if(RewiredPlayer.GetButtonDown("Interact"))
        {
            Interacting = true;
        }
        if (RewiredPlayer.GetButtonUp("Interact"))
        {
            Interacting = false;
        }


    }
    IEnumerator RegainStamina()
    {
        yield return new WaitForSeconds(2.0f);
         running = false;
        StopAllCoroutines();
    }


    IEnumerator SmokerCharacter (){
        yield return new WaitForSeconds(Random.Range(5, 7));
        MoveSpeed = 0;
        moveDirection = new Vector3(0, 0, 0);
        CharacterStop = true;
        yield return new WaitForSeconds(3.0f);
        MoveSpeed = 5;
        CharacterStop = false;
        StopAllCoroutines();
    }
    IEnumerator LimpingCharacter()
    {
        yield return new WaitForSeconds(0.5f);
        MoveSpeed = 2.5f;
        yield return new WaitForSeconds(0.5f);
        MoveSpeed = 5;
        StopAllCoroutines();
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HideableObject")) {
            HideableObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("HideableObject"))
        {
            HideableObject = null;
        }
    }


    IEnumerator ExitHiding() {
        yield return new WaitForSeconds(5.0f);
        transform.position = OriginalPosition;
        if (HideableObject != null)
            HideableObject.GetComponent<BoxCollider>().isTrigger = false;
        HideableObject.GetComponent<HideObjects>().NumberOfPlayers = 0;
        HideableObject = null;
        rBody.isKinematic = false;
        CharacterStop = false;
        yield return new WaitForSeconds(1.0f);
        Hiding = false;
        StopAllCoroutines();
    }

}
