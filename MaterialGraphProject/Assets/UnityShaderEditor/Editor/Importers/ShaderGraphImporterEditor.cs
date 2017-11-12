using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.AssetImporters;
using UnityEditor.ShaderGraph;
using UnityEditor.ShaderGraph.Drawing;
using UnityEngine;
using Debug = System.Diagnostics.Debug;
using Object = UnityEngine.Object;

[CustomEditor(typeof(ShaderGraphImporter))]
public class ShaderGraphImporterEditor : ScriptedImporterEditor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Open Shader Editor"))
        {
            AssetImporter importer = target as AssetImporter;
            Debug.Assert(importer != null, "importer != null");
            ShowGraphEditWindow(importer.assetPath);
        }
    }

    internal static bool ShowGraphEditWindow(string path)
    {
        var guid = AssetDatabase.AssetPathToGUID(path);
        var extension = Path.GetExtension(path);
        Type graphType;
        if (extension == ".ShaderGraph")
            graphType = typeof(MaterialGraph);
        else if (extension == ".LayeredShaderGraph")
            graphType = typeof(LayeredShaderGraph);
        else if (extension == ".ShaderSubGraph")
            graphType = typeof(SubGraph);
        else if (extension == ".ShaderRemapGraph")
            graphType = typeof(MasterRemapGraph);
        else
            return false;

        var foundWindow = false;
        foreach (var w in Resources.FindObjectsOfTypeAll<MaterialGraphEditWindow>())
        {
            if (w.selectedGuid == guid)
            {
                foundWindow = true;
                w.Focus();
            }
        }

        if (!foundWindow)
        {
            var window = ScriptableObject.CreateInstance<MaterialGraphEditWindow>();
            window.Show();
            window.ChangeSelection(guid, graphType);
        }
        return true;
    }

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        var path = AssetDatabase.GetAssetPath(instanceID);
        return ShowGraphEditWindow(path);
    }

}
