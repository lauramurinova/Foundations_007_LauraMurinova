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
    
    [Header("FPS")]
    private float _deltaTime = 0.0f;
    private float _fpsUpdateInterval = 0.5f; // Update FPS every 0.5 seconds
    private float _accum = 0.0f;
    private int _frames = 0;
    private float _timeleft;

   /// <summary>
   /// Toggles material on all the assigned renderers.
   /// </summary>
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
    
   /// <summary>
   /// Sets the assigned light on and off.
   /// </summary>
    public void ToggleLights()
    {
        _lightsOn = !_lightsOn;
        _light.gameObject.SetActive(_lightsOn);
    }
    
    /// <summary>
    /// Sets the gravity value of each assigned rigidbodies on and off.
    /// </summary>
    public void ToggleGravity()
    {
        _gravityOn = !_gravityOn;
        foreach (var rb in _rigidbodies)
        {
            rb.useGravity = _gravityOn;
        }
    }
    
    /// <summary>
    /// Sets the kinematic value of each assigned rigidbodies on and off.
    /// </summary>
    public void ToggleKinematic()
    {
        _kinematic = !_kinematic;
        foreach (var rb in _rigidbodies)
        {
            rb.isKinematic = _kinematic;
        }
    }

    /// <summary>
    /// Changes the constant force y value based on the value.
    /// </summary>
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

    /// <summary>
    /// Called by a ColorChannelSlider, sets the color of the light based on the values given by the ColorChannelSlider.
    /// </summary>
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
    
    /// <summary>
    /// Called by a ColorButton, sets the color of the light to the color assigned at the ColorButton.
    /// ColorButton is used so that the color can be assigned via the editor.
    /// </summary>
    public void ChangeLightsColor(ColorButton colorButton)
    {
        _light.color = colorButton.color;

        // Set the color values to the sliders too.
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
    
    private void Update()
    {
        CalculateFps();
    }

    /// <summary>
    /// Calculates the current fps on the device, sets the ui text.
    /// </summary>
    private void CalculateFps()
    {
        _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;

        _timeleft -= Time.deltaTime;
        _accum += Time.timeScale / Time.deltaTime;
        ++_frames;

        if (_timeleft <= 0.0f)
        {
            float fps = _accum / _frames;
            _fpsText.text = fps.ToString("F2");

            _timeleft = _fpsUpdateInterval;
            _accum = 0.0f;
            _frames = 0;
        }
    }
    
}
