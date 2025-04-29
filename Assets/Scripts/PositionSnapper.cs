using UnityEditor;
using UnityEngine;

public class PositionSnapper : MonoBehaviour
{
    [MenuItem("GameObject/Snap Position to Grid %#s", false, 0)]
    private static void SnapPositionToGrid()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            Undo.RecordObject(obj.transform, "Snap Position");
            Vector3 pos = obj.transform.position;
            obj.transform.position = new Vector3(Mathf.Round(pos.x), 0f, Mathf.Round(pos.z));
            EditorUtility.SetDirty(obj.transform);
        }
    }
}
