using UnityEngine;
using RuntimeNodeEditor;

namespace RuntimeNodeEditor.Eggode
{
    public class ApplicationStartup : MonoBehaviour
    {
        public RectTransform        editorHolder;
        public MapEditorNodeEditor    editor;

        private void Start()
        {
            Application.targetFrameRate = 60;
            var graph = editor.CreateGraph<NodeGraph>(editorHolder);
            editor.StartEditor(graph);
        }
    }
}