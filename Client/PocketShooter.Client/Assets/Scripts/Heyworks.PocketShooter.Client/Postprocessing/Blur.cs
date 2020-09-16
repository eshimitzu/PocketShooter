using System;
using Heyworks.PocketShooter.Postprocessing;

namespace UnityEngine.Rendering.PostProcessing
{
    [Serializable]
    [PostProcess(typeof(BlurEffect), PostProcessEvent.AfterStack, "Custom/Blur")]
    public sealed class Blur : PostProcessEffectSettings
    {
        public IntParameter Downsample = new IntParameter { value = 1 };
        public IntParameter BlurIterations = new IntParameter { value = 1 };
        public FloatParameter BlurSize = new FloatParameter { value = 3.0f };
        public ShaderParameter shader = new ShaderParameter();
    }

    public sealed class BlurEffect : PostProcessEffectRenderer<Blur>
    {
        public enum Pass
        {
            Downsample = 0,
            BlurVertical = 1,
            BlurHorizontal = 2,
        }

        private int blurId;
        private int parameterId;
        private int[] textureIds;

        public override void Init()
        {
            base.Init();
            blurId = Shader.PropertyToID("_BlurPostProcessEffect");
            parameterId = Shader.PropertyToID("_Parameter");
        }


        public override void Render(PostProcessRenderContext context)
        {
            CommandBuffer command = context.command;

            command.BeginSample("BlurPostEffect");

            int downsample = settings.Downsample;
            int blurIterations = settings.BlurIterations;
            float blurSize = settings.BlurSize;
            float widthMod = 1.0f / (1.0f * (1 << downsample));

            int rtW = context.width >> downsample;
            int rtH = context.height >> downsample;

            PropertySheet sheet = context.propertySheets.Get(settings.shader.value);
            sheet.properties.Clear();
            sheet.properties.SetVector(
                parameterId,
                new Vector4(blurSize * widthMod, -blurSize * widthMod, 0.0f, 0.0f));

            command.GetTemporaryRT(blurId, rtW, rtH, 0, FilterMode.Bilinear);
            command.BlitFullscreenTriangle(context.source, blurId, sheet, (int)Pass.Downsample);

            GenerateTextureNames();
            for (int i = 0; i < blurIterations; i++)
            {
                float iterationOffs = i * 1.0f;
                sheet.properties.SetVector(
                    parameterId,
                    new Vector4(blurSize * widthMod + iterationOffs, -blurSize * widthMod - iterationOffs, 0.0f, 0.0f));

                // Vertical blur..
                int rtId2 = textureIds[2 * i];
                command.GetTemporaryRT(rtId2, rtW, rtH, 0, FilterMode.Bilinear);
                command.BlitFullscreenTriangle(blurId, rtId2, sheet, (int)Pass.BlurVertical);
                command.ReleaseTemporaryRT(blurId);
                blurId = rtId2;

                // Horizontal blur..
                rtId2 = textureIds[2 * i + 1];
                command.GetTemporaryRT(rtId2, rtW, rtH, 0, FilterMode.Bilinear);
                command.BlitFullscreenTriangle(blurId, rtId2, sheet, (int)Pass.BlurHorizontal);
                command.ReleaseTemporaryRT(blurId);
                blurId = rtId2;
            }

            command.Blit(blurId, context.destination);
            command.ReleaseTemporaryRT(blurId);

            command.EndSample("BlurPostEffect");
        }

        private void GenerateTextureNames()
        {
            int count = settings.BlurIterations * 2;
            if (textureIds == null || textureIds.Length != count)
            {
                textureIds = new int[count];
                for (int i = 0; i < count; i++)
                {
                    int rtId = Shader.PropertyToID("_BlurPostProcessEffect" + i);
                    textureIds[i] = rtId;
                }
            }
        }
    }
}