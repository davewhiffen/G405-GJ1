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
    public Material[] CharactersMaterial;


    //private
    float MoveSpeed;
    float OriginalSpeed;
    float RunSpeed;
    int MaxStamina;
    bool start;
    bool running;
    bool rotating;
    Player RewiredPlayer;
    Vector3 moveDirection;
    Vector3 RotateDirection;
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
            StartCoroutine("CharacterSeted");
            //set basic player info
            switch (CharacterNumber)
            {
                case 0:
                    MoveSpeed = 5;
                    stamina = 100;
                    MaxStamina = 100;
                    RunSpeed = 15;
                    break;
                case 1:
                    MoveSpeed = 2.5f;
                    stamina = 200;
                    MaxStamina = 150;
                    RunSpeed = 15;
                    break;
                case 2:
                    MoveSpeed = 5;
                    stamina = 100;
                    MaxStamina = 100;
                    RunSpeed = 15;
                    break;
                case 3:
                    MoveSpeed = 5;
                    stamina = 100;
                    MaxStamina = 100;
                    RunSpeed = 20;
                    break;
            }
            OriginalSpeed = MoveSpeed;
        }
        //
        InputHandle();
        if (!running)
        {
            switch (CharacterNumber)
            {
                case 0:

                    break;
                case 1:

                    break;
                case 2:
                    StartCoroutine("SmokerCharacter");
                    break;
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
        moveDirection.x = RewiredPlayer.GetAxisRaw("horizontal");
        moveDirection.z = RewiredPlayer.GetAxisRaw("vertical");
        moveDirection = moveDirection.normalized * MoveSpeed;

        //rigt joystick to rotate 1st method
        if (rotating)
        {
            RotateDirection.x = RewiredPlayer.GetAxisRaw("RotateHorizontal");
            RotateDirection.z = RewiredPlayer.GetAxisRaw("RotateVertical");
            // RotateDirection = RotateDirection.normalized * 180;

            //old 
            //RotateDirection.y = RewiredPlayer.GetAxisRaw("RotateHorizontal");
            //RotateDirection = RotateDirection.normalized * 180;
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
        if (rotating)
        {
            Quaternion rotation = Quaternion.LookRotation(new Vector3(RotateDirection.x, RotateDirection.y, RotateDirection.z));
            transform.rotation = rotation;

            //old 
            //if (!rotating)
            //{
            //    Quaternion rotation = Quaternion.LookRotation(new Vector3(moveDirection.x, RotateDirection.y, moveDirection.z));
            //    transform.rotation = rotation;
            //}
        }

        //Character running
        if (RewiredPlayer.GetButtonLongPress("Run") && stamina > 0) {
            MoveSpeed = RunSpeed;
            running = true;
            stamina--;
            if (stamina <= 0) {
                stamina = 0;
                MoveSpeed = OriginalSpeed;
            }
        }
        else if (RewiredPlayer.GetButtonUp("Run"))
            running = false;
        else if (!running)
        {
            MoveSpeed = OriginalSpeed;
            stamina++;
            if (stamina >= MaxStamina)
                stamina = MaxStamina;
        }
    }

    IEnumerator SmokerCharacter (){
        yield return new WaitForSeconds(Random.Range(5, 7));
        MoveSpeed = 0;
        yield return new WaitForSeconds(3.0f);
        MoveSpeed = 5;
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

}
