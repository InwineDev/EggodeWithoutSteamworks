using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[VolumeComponentMenu("Eggode2/WaterEffect")]
public class WaterEffect : VolumeComponent, IPostProcessComponent
{
    public FloatParameter waveSpeed = new FloatParameter(0.5f);
    public FloatParameter waveStrength = new FloatParameter(0.1f);
    public FloatParameter distortion = new FloatParameter(0.2f);
    public ColorParameter waterColor = new ColorParameter(Color.cyan);

    public bool IsActive() => active && (waveStrength.value > 0f || distortion.value > 0f);
    public bool IsTileCompatible() => false;
}