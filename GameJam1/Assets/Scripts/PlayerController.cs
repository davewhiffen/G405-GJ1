using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerController : MonoBehaviour
{
    //public
    public int CharacterNumber{ get; private set; }
    public int PlayerNumber { get; private set; }
    [SerializeField] PlayerNum playerNum;
    public int stamina { get;  set; }

    //private
    float MoveSpeed;
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
        //set basic player info
        MoveSpeed = 5;
        stamina = 100;
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
        }
        //
        InputHandle();
    }
    //move player
    void FixedUpdate()
    {
        rBody.velocity = moveDirection;
      
        rBody.MoveRotation(rBody.rotation* Quaternion.Euler(RotateDirection * Time.deltaTime));
    }

    void SetCharacter() {
        //create gameobject to get there mesh 
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(0, 100.5f, 0);

        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = new Vector3(0, 100.5f, 0);

        GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        capsule.transform.position = new Vector3(2, 100, 0);

        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.position = new Vector3(-2, 100, 0);

        switch (CharacterNumber) {
            case 0:
                GetComponent<MeshFilter>().mesh = cube.GetComponent<MeshFilter>().mesh;
                break;
            case 1:
                GetComponent<MeshFilter>().mesh = sphere.GetComponent<MeshFilter>().mesh;
                break;
            case 2:
                GetComponent<MeshFilter>().mesh = capsule.GetComponent<MeshFilter>().mesh;
                break;
            case 3:
                GetComponent<MeshFilter>().mesh = cylinder.GetComponent<MeshFilter>().mesh;
                break;
        }
        //after the mesh is set destroy the created ones 
        Destroy(cube);
        Destroy(sphere);
        Destroy(capsule);
        Destroy(cylinder);

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
        if (!rotating)
        {
            Quaternion rotation = Quaternion.LookRotation(new Vector3(moveDirection.x, RotateDirection.y, moveDirection.z));
            transform.rotation = rotation;
        }

        //Character running
        if (RewiredPlayer.GetButtonLongPress("Run") &&stamina>0) {
            MoveSpeed = 15;
            running = true;
            stamina--;
            if (stamina <= 0) {
                stamina = 0;
                MoveSpeed = 5;
            }
        }
        else if (RewiredPlayer.GetButtonUp("Run"))
            running = false;
        else if (!running)
        {
            MoveSpeed = 5;
            stamina++;
            if (stamina >= 100)
                stamina = 100;
        }
    }
}
