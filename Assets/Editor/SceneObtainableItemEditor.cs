using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SceneObtainableItem))]
public class SceneObtainableItemEditor : Editor
{
  SerializedProperty m_ItemName;
  SerializedProperty m_ItemId;
  SerializedProperty m_ItemSprite;
  SerializedProperty m_ItemSpriteAssetPath;
  SerializedProperty m_SceneObtainablePrefab;
  SerializedProperty m_PrefabAssetPath;

  private const string kAssetPrefix = "Assets/Sprites";

  void OnEnable() {
    m_ItemName = serializedObject.FindProperty("m_ItemName");
    m_ItemId = serializedObject.FindProperty("m_ItemId");
    m_ItemSprite = serializedObject.FindProperty("m_ItemSprite");
    m_ItemSpriteAssetPath = 
      serializedObject.FindProperty("m_ItemSpriteAssetPath");
    m_SceneObtainablePrefab = serializedObject.FindProperty("m_SceneObtainablePrefab");
    m_PrefabAssetPath = serializedObject.FindProperty("m_PrefabAssetPath");
  }

  public override void OnInspectorGUI() {
    serializedObject.Update();
    EditorGUILayout.PropertyField(m_ItemName);
    EditorGUILayout.PropertyField(m_ItemId);
    EditorGUILayout.PropertyField(m_ItemSprite);
    using (new EditorGUI.DisabledScope(true)) {
      EditorGUILayout.PropertyField(m_ItemSpriteAssetPath);
    }
    EditorGUILayout.PropertyField(m_SceneObtainablePrefab);
    using (new EditorGUI.DisabledScope(true)) {
      EditorGUILayout.PropertyField(m_PrefabAssetPath);
    }

    if (m_ItemSprite.objectReferenceValue != null) {
      string assetPath =
        AssetDatabase.GetAssetPath(
          m_ItemSprite.objectReferenceValue.GetInstanceID());
      if (assetPath.StartsWith(kAssetPrefix))
      {
        m_ItemSpriteAssetPath.stringValue = assetPath;
      }
    }

    if (m_SceneObtainablePrefab.objectReferenceValue != null
        && PrefabUtility.IsPartOfPrefabAsset(m_SceneObtainablePrefab.objectReferenceValue)) {

      m_PrefabAssetPath.stringValue = 
        AssetDatabase.GetAssetPath(m_SceneObtainablePrefab
                                      .objectReferenceValue
                                      .GetInstanceID());

    }

    serializedObject.ApplyModifiedProperties();
  }
}
