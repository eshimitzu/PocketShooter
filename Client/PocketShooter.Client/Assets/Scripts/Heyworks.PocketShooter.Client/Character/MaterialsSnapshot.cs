using UnityEngine;

namespace Heyworks.PocketShooter.Character
{
    [System.Serializable]
    public class MaterialsSnapshot
    {
        [SerializeField]
        private Renderer renderer;

        [SerializeField]
        private Material[] materials;

        public MaterialsSnapshot(Renderer renderer, Material[] materials)
        {
            this.renderer = renderer;
            this.materials = materials;
        }

        public Material[] Materials => materials;

        public Renderer Renderer => renderer;

        public void Destroy()
        {
            foreach (Material mat in materials)
            {
                if (mat)
                {
                    Object.Destroy(mat);
                }
            }
        }

        public void Apply()
        {
            renderer.sharedMaterials = materials;
        }
    }
}