using System.Collections.Generic;
using UnityEngine;

namespace Heyworks.PocketShooter.Character
{
    public class RenderView
    {
        private List<MaterialsSnapshot> initialMaterials = new List<MaterialsSnapshot>();
        private List<MaterialsSnapshot> clonedMaterials = new List<MaterialsSnapshot>();
        private List<MaterialsSnapshot> currentMaterials;

        public IReadOnlyList<MaterialsSnapshot> ClonedMaterials
        {
            get
            {
                if (clonedMaterials.Count == 0)
                {
                    foreach (MaterialsSnapshot original in initialMaterials)
                    {
                        var snapshot = new MaterialsSnapshot(original.Renderer, original.Renderer.materials);
                        clonedMaterials.Add(snapshot);
                    }

                    currentMaterials = clonedMaterials;
                }

                return clonedMaterials;
            }
        }

        public IReadOnlyList<MaterialsSnapshot> InitialMaterials => initialMaterials;

        public IReadOnlyList<MaterialsSnapshot> CurrentMaterials => currentMaterials;

        public RenderView(Renderer[] renderers)
        {
            foreach (Renderer renderer in renderers)
            {
                var snapshot = new MaterialsSnapshot(renderer, renderer.sharedMaterials);
                initialMaterials.Add(snapshot);
            }

            currentMaterials = initialMaterials;
        }

        public void ApplyDefaultMaterials()
        {
            foreach (MaterialsSnapshot material in CurrentMaterials)
            {
                material.Apply();
            }
        }

        public void Clean()
        {
            foreach (MaterialsSnapshot mat in clonedMaterials)
            {
                mat.Destroy();
            }
        }
    }
}