using System.Data.SqlClient;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Heyworks.PocketShooter
{
    public static class TransformMenu
    {
        private class TransformSnapshot
        {
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Scale;
            public bool Local;
        }

        private static TransformSnapshot copied = new TransformSnapshot();

        [MenuItem("CONTEXT/Transform/Copy Local Transform")]
        static void CopyLocalTransform(MenuCommand command)
        {
            var transform = (Transform)command.context;

            copied.Position = transform.localPosition;
            copied.Rotation = transform.localRotation;
            copied.Scale = transform.localScale;
            copied.Local = true;
        }

        [MenuItem("CONTEXT/Transform/Paste Transform")]
        static void PasteTransform(MenuCommand command)
        {
            var transform = (Transform)command.context;

            transform.localPosition = copied.Position;
            transform.localRotation = copied.Rotation;
            transform.localScale = copied.Scale;
        }
    }
}