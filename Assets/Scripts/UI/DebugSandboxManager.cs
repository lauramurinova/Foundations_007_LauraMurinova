using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugSandboxManager : MonoBehaviour
{
    [SerializeField] private ColorChannelSlider[] _colorChannelSliders;
    
    [SerializeField] private Light _light;
    [SerializeField] private MeshRenderer[] _renderers;
    [SerializeField] private Material[] _materials;
    [SerializeField] private Rigidbody[] _rigidbodies;
    [SerializeField] private Text _fpsText;

    private Color _lightsColor;
    private bool _lightsOn = true;
    private bool _gravityOn = false;
    private bool _kinematic = false;
    private int _currentMaterialIndex = 0;
    
    private float deltaTime = 0.0f;
    private float fpsUpdateInterval = 0.5f; // Update FPS every 0.5 seconds
    private float accum = 0.0f;
    private int frames = 0;
    private float timeleft;

    private void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        if (timeleft <= 0.0f)
        {
            float fps = accum / frames;
            _fpsText.text = fps.ToString("F2");

            timeleft = fpsUpdateInterval;
            accum = 0.0f;
            frames = 0;
        }
    }

    public void SwitchMaterial()
    {
        _currentMaterialIndex++;

        if (_currentMaterialIndex == _materials.Length)
        {
            _currentMaterialIndex = 0;
        }
        
        foreach (var rend in _renderers)
        {
            rend.material = _materials[_currentMaterialIndex];
        }
    }
    
    public void ToggleLights()
    {
        _lightsOn = !_lightsOn;
        _light.gameObject.SetActive(_lightsOn);
    }
    
    public void ToggleGravity()
    {
        _gravityOn = !_gravityOn;
        foreach (var rb in _rigidbodies)
        {
            rb.useGravity = _gravityOn;
        }
    }
    
    public void ToggleKinematic()
    {
        _kinematic = !_kinematic;
        foreach (var rb in _rigidbodies)
        {
            rb.isKinematic = _kinematic;
        }
    }

    public void ChangeConstantForce(float value)
    {
        foreach (var rb in _rigidbodies)
        {
            if(!rb.TryGetComponent(out ConstantForce constantForce)) continue;

            var force = constantForce.force;
            force.y = value;
            constantForce.force = force;
        }
    }

    public void ChangeLightColorChannel(ColorChannelSlider colorSlider)
    {
        Color color = _light.color;
        
        switch (colorSlider.channel)
        {
            case ColorChannel.Red:
            {
                color.r = colorSlider.slider.value;
                break;
            }
            case ColorChannel.Green:
            {
                color.g = colorSlider.slider.value;;
                break;
            }
            case ColorChannel.Blue:
            {
                color.b = colorSlider.slider.value;;
                break;
            }
        }
        _light.color = color;
    }
    
    public void ChangeLightsColor(ColorButton colorButton)
    {
        _light.color = colorButton.color;

        foreach (var colorChannelSlider in _colorChannelSliders)
        {
            switch (colorChannelSlider.channel)
            {
                case ColorChannel.Red:
                {
                    colorChannelSlider.slider.value = colorButton.color.r;
                    break;
                }
                case ColorChannel.Green:
                {
                    colorChannelSlider.slider.value = colorButton.color.g;
                    break;
                }
                case ColorChannel.Blue:
                {
                    colorChannelSlider.slider.value = colorButton.color.b;
                    break;
                }
            }
        }
    }
}
