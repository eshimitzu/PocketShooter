using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Heyworks.PocketShooter.Utils.Extensions
{
    public static class GameObjectExtension
    {
        private static List<SkinnedMeshRenderer> smrList = new List<SkinnedMeshRenderer>();
        private static List<MeshFilter> mfList = new List<MeshFilter>();

        /// <summary>
        /// Render the specified gameobject with position and rotation.
        /// </summary>
        /// <param name="gameobject">Gameobject.</param>
        /// <param name="position">Position.</param>
        /// <param name="rotation">Rotation.</param>
        public static void Render(this GameObject gameobject, Vector3 position, Quaternion rotation)
        {
            gameobject.Render(position, rotation, Vector3.one);
        }

        /// <summary>
        /// Render the specified gameobject with position, rotation and scale.
        /// </summary>
        /// <param name="gameobject">Gameobject.</param>
        /// <param name="position">Position.</param>
        /// <param name="rotation">Rotation.</param>
        /// <param name="scale">Scale.</param>
        public static void Render(this GameObject gameobject, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            smrList.Clear();
            gameobject.GetComponentsInChildren<SkinnedMeshRenderer>(smrList);
            foreach (SkinnedMeshRenderer smr in smrList)
            {
                GameObject renderObject = smr.gameObject;
                Mesh mesh = smr.sharedMesh;
                Material[] materials = smr.GetComponent<Renderer>().sharedMaterials;
                Quaternion q = rotation * renderObject.transform.rotation;
                Vector3 roscale = new Vector3(
                    renderObject.transform.lossyScale.x * scale.x,
                    renderObject.transform.lossyScale.y * scale.y,
                    renderObject.transform.lossyScale.z * scale.z);
                Matrix4x4 matrix = Matrix4x4.TRS(position + (renderObject.transform.position - gameobject.transform.position), q, roscale);

                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i] != null && mesh != null)
                    {
                        materials[i].SetPass(0);
                        Graphics.DrawMeshNow(mesh, matrix, i);
                    }
                }
            }

            mfList.Clear();
            gameobject.GetComponentsInChildren<MeshFilter>(mfList);
            foreach (MeshFilter mf in mfList)
            {
                GameObject renderObject = mf.gameObject;
                Mesh mesh = mf.sharedMesh;
                Material[] materials = mf.GetComponent<Renderer>().sharedMaterials;
                Quaternion q = rotation * renderObject.transform.rotation;
                Vector3 roscale = new Vector3(
                    renderObject.transform.lossyScale.x * scale.x,
                    renderObject.transform.lossyScale.y * scale.y,
                    renderObject.transform.lossyScale.z * scale.z);
                Matrix4x4 matrix = Matrix4x4.TRS(position + (renderObject.transform.position - gameobject.transform.position), q, roscale);

                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i] != null && mesh != null)
                    {
                        materials[i].SetPass(0);
                        Graphics.DrawMeshNow(mesh, matrix, i);
                    }
                }
            }
        }
    }
}