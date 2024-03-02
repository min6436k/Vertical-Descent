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

        // UI 요소의 위치를 스크린 좌표에 맞게 설정
        transform.localPosition = SliderPosition;
    }
}
