using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeverParticle : MonoBehaviour
{
    public GameObject Slider;
    private Vector3 SliderPosition;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SliderPosition = Camera.main.ScreenToWorldPoint(Slider.transform.position);

        // UI ����� ��ġ�� ��ũ�� ��ǥ�� �°� ����
        transform.localPosition = SliderPosition;
    }
}
