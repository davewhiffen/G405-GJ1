using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class FireInteract : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider) => FireText.score += 1;

}
