using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Snapshotter
{
    [MenuItem("GameObject/Take Object Snapshot", false, 10)]
    public static void TakeObjectSnapshot() {
        var targetObject = Selection.activeGameObject;
        if (targetObject == null) {
            Debug.LogWarning("Snappshotter needs a selected GameObject");
        }
        var snapshotCamera = SnapshotCamera.MakeSnapshotCameraFromSceneView(30);
        var texture = snapshotCamera.TakeObjectSnapshot(
            targetObject, 
            Color.clear, 
            targetObject.transform.position - snapshotCamera.transform.position,
            targetObject.transform.rotation,
            targetObject.transform.localScale,
            SceneView.lastActiveSceneView.camera.pixelWidth,
            SceneView.lastActiveSceneView.camera.pixelHeight
        );
        SnapshotCamera.SavePNG(texture);
        Object.DestroyImmediate(snapshotCamera.gameObject);
    }
}
