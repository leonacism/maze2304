using UnityEditor;
using UnityEngine;
using UnityEngine.Windows;
using Scenes.InGame.Items;

namespace Editor
{
    public class ItemThumbnailGenerator : EditorWindow
    {
        public int width = 256;
        public int height = 256;
        public int depth = 24;

        public float distance = 1.0f;
        private Vector3 lookAt = Vector3.zero;
        private Vector3 eulerAngle = new Vector3(20.0f, 150.0f, 0.0f);
        public float fieldOfView = 15.0f;

        [MenuItem("Window/ItemThumbnailGenerator")]
        private static void ShowWindow()
        {
            GetWindow<ItemThumbnailGenerator>("ItemThumbnailGenerator");
        }
    
        private void OnGUI()
        {
            width = EditorGUILayout.IntField("Width", width);
            height = EditorGUILayout.IntField("Height", height);
            depth = EditorGUILayout.IntField("Depth", depth);
            distance = EditorGUILayout.FloatField("Distance", distance);
            lookAt = EditorGUILayout.Vector3Field("Look At", lookAt);
            eulerAngle = EditorGUILayout.Vector3Field("Euler Angles", eulerAngle);
            fieldOfView = EditorGUILayout.FloatField("FOV", fieldOfView);

            if (GUILayout.Button("Generate"))
            {
                GenerateThumbnail();
            }
        }

        private void GenerateThumbnail()
        {
            var itemObject = Selection.activeGameObject;
            if (itemObject == null)
            {
                Debug.LogError("No GameObject selected");
                return;
            }

            Camera thumbnailCamera = new GameObject("Thumbnail Camera").AddComponent<Camera>();
            Quaternion rotation = Quaternion.Euler(itemObject.transform.rotation.eulerAngles) * Quaternion.Euler(eulerAngle);
            thumbnailCamera.transform.position = rotation * Vector3.back * distance + itemObject.transform.position;
            thumbnailCamera.transform.rotation = rotation;
            thumbnailCamera.fieldOfView = fieldOfView;
            thumbnailCamera.clearFlags = CameraClearFlags.SolidColor;
            thumbnailCamera.backgroundColor = Color.clear;
            thumbnailCamera.nearClipPlane = 1e-5f;
            
            var itemStack = itemObject.GetComponent<ItemStack>();
            if (itemStack == null)
            {
                Debug.LogError("Selected GameObject does not have an ItemStack component");
                return;
            }

            var renderTexture = new RenderTexture(width, height, depth);
            thumbnailCamera.targetTexture = renderTexture;

            thumbnailCamera.Render();

            var previousActive = RenderTexture.active;
            RenderTexture.active = renderTexture;

            var texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
            texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture2D.Apply();

            RenderTexture.active = previousActive;

            byte[] bytes = texture2D.EncodeToPNG();
            string filePath = EditorUtility.SaveFilePanel("Save Thumbnail", "", $"InventryThumbnail_{itemStack.ItemName}.png", "png");
            if (!string.IsNullOrEmpty(filePath))
            {
                File.WriteAllBytes(filePath, bytes);
                AssetDatabase.Refresh();

                string relativePath = $"Assets{filePath.Substring(Application.dataPath.Length)}";
                TextureImporter textureImporter = AssetImporter.GetAtPath(relativePath) as TextureImporter;
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.mipmapEnabled = false;
                textureImporter.filterMode = FilterMode.Bilinear;
                AssetDatabase.ImportAsset(relativePath);

                itemStack.ThumbnailSprite = AssetDatabase.LoadAssetAtPath<Sprite>(relativePath);
            }

            DestroyImmediate(thumbnailCamera.gameObject);
        }
    }
}
