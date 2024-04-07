using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public enum ColorChannel
{
    Red = 0,
    Green = 1,
    Blue = 2
}

public class ColorChannelSlider : MonoBehaviour
{
    public ColorChannel channel;
    public Slider slider;
}
