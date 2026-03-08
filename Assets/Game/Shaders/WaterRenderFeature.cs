using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Linq;

public class WaterRenderFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public Material waterMaterial;
        public RenderPassEvent renderEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public Settings settings = new Settings();
    private WaterPass _waterPass;

    public override void Create()
    {
        _waterPass = new WaterPass(settings.waterMaterial, settings.renderEvent);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.waterMaterial == null)
        {
            var mats = Resources.FindObjectsOfTypeAll<Material>();
            settings.waterMaterial = mats.FirstOrDefault(m => m.shader.name == "Custom/WaterEffect");

            if (settings.waterMaterial == null)
            {
                Debug.LogError("Water material missing! Create a material with 'Custom/WaterEffect' shader and assign it.");
                return;
            }
        }

        renderer.EnqueuePass(_waterPass);
    }

    private class WaterPass : ScriptableRenderPass
    {
        private readonly Material _waterMaterial;
        private RenderTargetIdentifier _source;
        private RenderTargetHandle _tempTexture;

        public WaterPass(Material material, RenderPassEvent renderEvent)
        {
            _waterMaterial = material;
            this.renderPassEvent = renderEvent;
            _tempTexture.Init("_WaterTempTexture");
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            _source = renderingData.cameraData.renderer.cameraColorTarget;
            cmd.GetTemporaryRT(_tempTexture.id, renderingData.cameraData.cameraTargetDescriptor);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // ѕропускаем выполнение дл€ камер пост-обработки (Volume)
            if (_waterMaterial == null || renderingData.cameraData.cameraType == CameraType.Reflection ||
                renderingData.cameraData.cameraType == CameraType.Preview)
                return;

            var cmd = CommandBufferPool.Get("WaterEffect");

            Blit(cmd, _source, _tempTexture.Identifier(), _waterMaterial);
            Blit(cmd, _tempTexture.Identifier(), _source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(_tempTexture.id);
        }
    }
}