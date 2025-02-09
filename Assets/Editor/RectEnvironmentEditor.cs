using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RectEnvironment))]
public class RectEnvironmentEditor : Editor
{
    private const float MIN_DISTANCE_BETWEEN_START_END = 0.2f;
    private RectEnvironment _rectEnvironment;


    private void OnEnable()
    {
        _rectEnvironment = (RectEnvironment) target;
    }

    private void OnSceneGUI()
    {
        serializedObject.Update();
        Handles.color = Color.blue;
        var startGlobalPosition = _rectEnvironment.transform.TransformPoint(_rectEnvironment.StartPointLocalPosition);
        var style = new GUIStyle(EditorStyles.label) {normal = {textColor = Color.blue}};
        Handles.Label(
            startGlobalPosition + new Vector3(HandleUtility.GetHandleSize(startGlobalPosition) * 0.2f,
                HandleUtility.GetHandleSize(startGlobalPosition) * 0.2f, 0),
            "Start Line", style);

        Handles.color = Color.white;
        EditorGUI.BeginChangeCheck();
        var fmh_30_13_638746938116694278 = _rectEnvironment.transform.rotation; startGlobalPosition = Handles.FreeMoveHandle(startGlobalPosition 
            , 1.5f
            , Vector3.zero, ArrowHandleCap);


        if (EditorGUI.EndChangeCheck() &&
            _rectEnvironment.transform.InverseTransformPoint(startGlobalPosition).z +
            MIN_DISTANCE_BETWEEN_START_END < _rectEnvironment.EndPointLocalPosition.z)
        {
            if (_rectEnvironment.transform.TransformPoint(_rectEnvironment.EndPointLocalPosition).z >
                startGlobalPosition.z + MIN_DISTANCE_BETWEEN_START_END)
            {
                Undo.RecordObject(_rectEnvironment, "start pos");
                var point = _rectEnvironment.transform.InverseTransformPoint(startGlobalPosition);
                point.y = 0;
                point.x = 0f;
                _rectEnvironment.StartPointLocalPosition = point;
            }
        }

        if (_rectEnvironment.EndPointLocalPosition.z <
            _rectEnvironment.StartPointLocalPosition.z + MIN_DISTANCE_BETWEEN_START_END)
        {
            _rectEnvironment.StartPointLocalPosition = _rectEnvironment.EndPointLocalPosition -
                                                       Vector3.forward * MIN_DISTANCE_BETWEEN_START_END;
        }


        Handles.color = Color.blue;
        var endGlobalPosition = _rectEnvironment.transform.TransformPoint(_rectEnvironment.EndPointLocalPosition);
        Handles.Label(
            endGlobalPosition + new Vector3(HandleUtility.GetHandleSize(endGlobalPosition) * 0.2f,
                HandleUtility.GetHandleSize(endGlobalPosition) * 0.2f),
            "end Line", style);
        Handles.color = Color.white;
        EditorGUI.BeginChangeCheck();
        var fmh_66_71_638746938116713249 = _rectEnvironment.transform.rotation; endGlobalPosition = Handles.FreeMoveHandle(endGlobalPosition,
            1.5f, Vector3.zero, ArrowHandleCap);


        if (EditorGUI.EndChangeCheck())
        {
            if (endGlobalPosition.z > startGlobalPosition.z + MIN_DISTANCE_BETWEEN_START_END)
            {
                Undo.RecordObject(_rectEnvironment, "end pos");

                var point = _rectEnvironment.transform.InverseTransformPoint(endGlobalPosition);
                point.y = 0;
                point.x = 0;
                _rectEnvironment.EndPointLocalPosition = point;
            }
        }

        if (_rectEnvironment.EndPointLocalPosition.z <
            _rectEnvironment.StartPointLocalPosition.z + MIN_DISTANCE_BETWEEN_START_END)
        {
            _rectEnvironment.EndPointLocalPosition = _rectEnvironment.StartPointLocalPosition +
                                                     Vector3.forward * MIN_DISTANCE_BETWEEN_START_END;
        }


        serializedObject.ApplyModifiedProperties();
    }

    public void ArrowHandleCap(int controlID, Vector3 position, Quaternion rotation, float size,
        EventType eventType)
    {
//        var distanceToStart =
//            ( position.ToXZ() -
//             _rectEnvironment.transform.TransformPoint(_rectEnvironment.StartPointLocalPosition).ToXZ()).magnitude;
//        var distanceToEnd =
//            (position.ToXZ() -
//             _rectEnvironment.transform.TransformPoint(_rectEnvironment.EndPointLocalPosition).ToXZ()).magnitude;
//        var startMove = distanceToStart < distanceToEnd;
        Handles.ArrowHandleCap(controlID, position
            , rotation
            , size, eventType);
    }


    [MenuItem("Environment", menuItem = "GameObject/Rect Environment")]
    static void CreateBlockGroup()
    {
        var rectEnv = new GameObject("Environment");
        if (Selection.activeTransform != null)
        {
            rectEnv.transform.parent = Selection.activeTransform;
            rectEnv.transform.position = Selection.activeTransform.position;
        }

        var bGroup = rectEnv.AddComponent<RectEnvironment>();
        bGroup.StartPointLocalPosition = new Vector3(0, 0, 0f);
        bGroup.EndPointLocalPosition = new Vector3(0, 0, 5f);

        Selection.activeObject = rectEnv;
    }
}