using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI sliderText;
    [SerializeField] private int hiveId;

    private void Start()
    {
        slider.onValueChanged.AddListener((v) =>
        {
            sliderText.text = v.ToString("0.00");
            HiveManager hm = HiveManager.Instance;
            if (hm.GetNumOfHives() > hiveId)
            {
                hm.SetViewAngle(v, hiveId);
            }
        });
    }
}
