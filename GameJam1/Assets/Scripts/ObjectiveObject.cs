using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveObject : MonoBehaviour
{
    public Slider slider;
    public int sliderMaxValue = 250;

    //[TextArea(3,10)]
    public string description = "*Objective Description Here*";

    [HideInInspector] public bool objectiveEnabled = false;
    private bool objectiveCompleted = false;
    private GameManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<GameManager>();
        slider.maxValue = sliderMaxValue;
    }

    // Update is called once per frame
    void Update()
    {
        if(!objectiveEnabled)
        {
            slider.value = 0;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!objectiveCompleted && objectiveEnabled)
        {
            if (other.gameObject.tag == "Player")
            {
                if (other.gameObject.GetComponent<PlayerController>().Interacting == true)
                {
                    slider.gameObject.SetActive(true);
                    slider.value++;

                    if (slider.value == sliderMaxValue)
                    {                       
                        objectiveCompleted = true;
                        slider.gameObject.SetActive(false);
                        slider.value = 0;
                        manager.objCompleted++;
                    }
                }
                else
                {
                    slider.gameObject.SetActive(false);
                    slider.value = 0;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        slider.gameObject.SetActive(false);
        slider.value = 0;

        if(objectiveCompleted)
        {
            objectiveCompleted = false;
        }
    }
}
