using UnityEngine;

namespace Heyworks.PocketShooter.Character
{
    public static class TrooperMaterialExtension
    {
        private static readonly int RimColorId = Shader.PropertyToID("_RimColor");
        private static readonly int OutlineColorId = Shader.PropertyToID("_OutlineColor");
        private static readonly int RimPowerId = Shader.PropertyToID("_RimPower");
        private static readonly int OutlineId = Shader.PropertyToID("_Outline");

        public static Color GetRimColor(this Material mat) => mat.GetColor(RimColorId);

        public static void SetRimColor(this Material mat, Color color) => mat.SetColor(RimColorId, color);

        public static Color GetOutlineColor(this Material mat) => mat.GetColor(OutlineColorId);

        public static void SetOutlineColor(this Material mat, Color color) => mat.SetColor(OutlineColorId, color);

        public static float GetRimPower(this Material mat) => mat.GetFloat(RimPowerId);

        public static void SetRimPower(this Material mat, float value) => mat.SetFloat(RimPowerId, value);

        public static float GetOutline(this Material mat) => mat.GetFloat(OutlineId);

        public static void SetOutline(this Material mat, float value) => mat.SetFloat(OutlineId, value);
    }
}