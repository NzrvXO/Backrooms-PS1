using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

public class PSXRenderFeature : ScriptableRendererFeature
{
    class PSXPass : ScriptableRenderPass
    {
        Material material;

        public PSXPass(Material material)
        {
            this.material = material;
            renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        }

        class PassData
        {
            public Material material;
            public TextureHandle source;
        }

        static void Execute(PassData data, RasterGraphContext ctx)
        {
            Blitter.BlitTexture(
                ctx.cmd,
                data.source,
                new Vector4(1, 1, 0, 0),
                data.material,
                0
            );
        }

        public override void RecordRenderGraph(
            RenderGraph renderGraph,
            ContextContainer frameData)
        {
            var resources = frameData.Get<UniversalResourceData>();

            TextureHandle source = resources.activeColorTexture;

            using var builder = renderGraph.AddRasterRenderPass<PassData>(
                "PSX Pass",
                out var passData
            );

            passData.material = material;
            passData.source = source;

            builder.UseTexture(source, AccessFlags.Read);

            // 🔥 ВАЖНО: пишем прямо в camera color (RenderGraph сам делает safe swap)
            builder.SetRenderAttachment(
                resources.activeColorTexture,
                0
            );

            builder.SetRenderFunc((PassData data, RasterGraphContext ctx) =>
            {
                Execute(data, ctx);
            });
        }
    }

    [SerializeField] Shader shader;

    Material material;
    PSXPass pass;

    public override void Create()
    {
        if (shader == null)
            return;

        material = CoreUtils.CreateEngineMaterial(shader);
        pass = new PSXPass(material);
    }

    public override void AddRenderPasses(
        ScriptableRenderer renderer,
        ref RenderingData renderingData)
    {
        if (material == null)
            return;

        renderer.EnqueuePass(pass);
    }
}