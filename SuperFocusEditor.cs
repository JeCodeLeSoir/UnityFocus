using UnityEditor;
using UnityEngine;

namespace JeCodeLeSoir
{
    public class SuperFocusEditor : EditorWindow
    {
        [MenuItem("Window/SuperFocus")]
        static void Init()
        {
            SuperFocusEditor window = (SuperFocusEditor)EditorWindow.GetWindow(typeof(SuperFocusEditor));
            window.Show();
        }

        [RuntimeInitializeOnLoadMethod]
        static void OnRuntimeMethodLoad()
        {
            Init();
        }

        void OnGUI()
        {
            GUILayout.Label("Infos", EditorStyles.boldLabel);

            GUILayout.Label("No select object and press F for focus", EditorStyles.label);
            GUILayout.Label("Press shift and Wheel (+, -) for Distance", EditorStyles.label);

            Scale = EditorGUILayout.Vector3Field("Scale:", Scale);

            GUILayout.Label("By JecodeLeSoir", EditorStyles.boldLabel);
        }

        Vector3 lastWorldPosition;
        float Zoom;
        Vector3 Scale = Vector3.one;

        private void OnEnable()
        {
            SceneView.duringSceneGui += this.OnSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= this.OnSceneGUI;
        }

        public void OnSceneGUI(SceneView sceneView)
        {
            var view = SceneView.currentDrawingSceneView;
            if (view)
            {
                Event guiEvent = Event.current;
                Vector3 mousePosition = guiEvent.mousePosition;
                mousePosition.z = Zoom;

                float ppp = EditorGUIUtility.pixelsPerPoint;
                mousePosition.y = view.camera.pixelHeight - mousePosition.y * ppp;
                mousePosition.x *= ppp;

                if (guiEvent.shift && guiEvent.type == EventType.ScrollWheel)
                {
                    Zoom -= (guiEvent.delta.y / 3);
                    Zoom = Mathf.Clamp(Zoom, 0, 10);
                    guiEvent.Use();
                }

                Vector3 worldPosition = view.camera.ScreenToWorldPoint(mousePosition);
                Ray ray = Camera.current.ScreenPointToRay(mousePosition);
                RaycastHit hit;

                if (Selection.activeObject == null)
                {
                    if (Physics.Raycast(ray, out hit))
                    {
                        worldPosition = hit.point;
                    }

                    if (guiEvent.type == EventType.KeyDown && guiEvent.keyCode == KeyCode.F)
                    {
                        SceneView.lastActiveSceneView.Frame(new Bounds(worldPosition, Scale), true);
                        guiEvent.Use();
                    }

                    Handles.color = Color.blue;
                    Handles.DrawWireCube(worldPosition, Scale);
                    if (lastWorldPosition != worldPosition)
                    {
                        sceneView.Repaint();
                        lastWorldPosition = worldPosition;
                    }
                }
            }
        }
    }
}
