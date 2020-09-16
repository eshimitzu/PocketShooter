using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Heyworks.PocketShooter.Postprocessing
{
    [Serializable]
    [PostProcess(typeof(GrayscaleRenderer), PostProcessEvent.AfterStack, "Custom/Grayscale")]
    public sealed class Grayscale : PostProcessEffectSettings
    {
        [Range(0f, 1f), Tooltip("Grayscale effect intensity.")]
        public FloatParameter blend = new FloatParameter { value = 0.5f };

        public ShaderParameter shader = new ShaderParameter();
    }

    [Serializable]
    public sealed class ShaderParameter : ParameterOverride<Shader>
    {
    }


    public sealed class GrayscaleRenderer : PostProcessEffectRenderer<Grayscale>
    {
        private int blendId;

        public override void Init()
        {
            base.Init();
            blendId = Shader.PropertyToID("_Blend");
        }

        public override void Render(PostProcessRenderContext context)
        {
            PropertySheet sheet = context.propertySheets.Get(settings.shader.value);
            sheet.properties.SetFloat(blendId, settings.blend);
            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }
    }
}