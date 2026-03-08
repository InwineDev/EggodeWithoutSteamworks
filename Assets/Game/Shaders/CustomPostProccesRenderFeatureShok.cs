using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class CustomPostProccesRenderFeatureShok : ScriptableRendererFeature
{
    private CustomPostProcessShok shokProcess;

    [SerializeField]
    private Shader bloomShader;
    [SerializeField]
    private Shader compositeShader;
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(shokProcess);
    }

    public override void Create()
    {
        shokProcess = new CustomPostProcessShok();
    }

}
