using System;
using RuntimeNodeEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RuntimeNodeEditor.Eggode
{
    public class MapEditorNodeEditor : NodeEditor
    {
        private string _savePath;

        public override void StartEditor(NodeGraph graph)
        {
            base.StartEditor(graph);

            //_savePath = Application.dataPath + "/Examples/2_SimpleMathEditor/Resources/graph.json";
            
            Events.OnGraphPointerClickEvent           += OnGraphPointerClick;
            Events.OnNodePointerClickEvent            += OnNodePointerClick;
            Events.OnConnectionPointerClickEvent      += OnNodeConnectionPointerClick;
            Events.OnSocketConnect                    += OnConnect;

            Graph.SetSize(Vector2.one * 20000);
        }

        private void OnConnect(SocketInput arg1, SocketOutput arg2)
        {
            Graph.drawer.SetConnectionColor(arg2.connection.connId, Color.green);
        }

        private void OnGraphPointerClick(PointerEventData eventData)
        {
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Right:
                    {
                        var ctx = new ContextMenuBuilder()
                        .Add("nodes/Varibles/float", () => CreateNode("Nodes/FloatNode"))
                        .Add("nodes/Varibles/string", () => CreateNode("Nodes/StringNode"))
                        .Add("nodes/Varibles/int", () => CreateNode("Nodes/IntNode"))
                        .Add("nodes/Varibles/bool", () => CreateNode("Nodes/BoolNode"))
                        .Add("nodes/Varibles/Vector 3", () => CreateNode("Nodes/Vector3Node"))
                        .Add("nodes/Varibles/Get VAR", () => CreateNode("Nodes/GetVarNode"))
                        .Add("nodes/Varibles/Set VAR", () => CreateNode("Nodes/SetVarNode"))
                        .Add("nodes/Varibles/Booled", () => CreateNode("Nodes/BooledNode"))
                        .Add("nodes/Random/int", () => CreateNode("Nodes/IntRandomNode"))
                        .Add("nodes/Random/float", () => CreateNode("Nodes/FloatRandomNode"))
                        //.Add("nodes/math op",       CreateMatOpNode)
                        .Add("nodes/Starters/OnStart", () => CreateNode("Nodes/OnStartNode"))
                        .Add("nodes/Starters/OnTriggerEnter", () => CreateNode("Nodes/OnTriggerEnterNode"))
                        .Add("nodes/Starters/OnCollisionEnter", () => CreateNode("Nodes/OnCollisionEnterNode"))
                        .Add("nodes/Actions/Destroy", () => CreateNode("Nodes/DestroyNode"))
                        .Add("nodes/Actions/ShowMessage", () => CreateNode("Nodes/ShowMessageNode"))
                        .Add("nodes/Actions/Damage", () => CreateNode("Nodes/DamageNode"))
                        .Add("nodes/Actions/ChangeGravity", () => CreateNode("Nodes/ChangeGravityNode"))
                        .Add("nodes/Actions/Teleport", () => CreateNode("Nodes/TpNode"))
                        .Add("nodes/Actions/SleepTime", () => CreateNode("Nodes/TimeNode"))
                        .Add("nodes/Check/IfNode", () => CreateNode("Nodes/IfNode"))
                        .Add("nodes/Loop/For", () => CreateNode("Nodes/ForNode"))
                        .Add("nodes/Varibles/ThisObject", () => CreateNode("Nodes/ThisObjectNode"))
                        .Add("nodes/Varibles/Object from ID", () => CreateNode("Nodes/ObjectFromIdNode"))
                        //.Add("graph/load",          ()=>LoadGraph(_savePath))
                        .Add("graph/save", () => SaveGraph(_savePath))
                        .Build();

                        SetContextMenu(ctx);
                        DisplayContextMenu();
                    }
                    break;
                case PointerEventData.InputButton.Left: CloseContextMenu(); break;
            }
        }

        private void SaveGraph(string savePath)
        {
            CloseContextMenu();
            Graph.SaveFile(savePath);
        }

        public void LoadGraph(string savePath)
        {
            CloseContextMenu();
            Graph.Clear();
            Graph.LoadFile(savePath);
        }

        private void OnNodePointerClick(Node node, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                var ctx = new ContextMenuBuilder()
                .Add("duplicate",            () => DuplicateNode(node))
                .Add("clear connections",    () => ClearConnections(node))
                .Add("delete",               () => DeleteNode(node))
                .Build();

                SetContextMenu(ctx);
                DisplayContextMenu();
            }
        }

        private void OnNodeConnectionPointerClick(string connId, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                var ctx = new ContextMenuBuilder()
                .Add("clear connection", () => DisconnectConnection(connId))
                .Build();

                SetContextMenu(ctx);
                DisplayContextMenu();
            }
        }


        //  context item actions

        private void CreateNode(string pathToNode)
        {
            Graph.Create(pathToNode);
            CloseContextMenu();
        }

        private void DeleteNode(Node node)
        {
            Graph.Delete(node);
            CloseContextMenu();
        }
        
        private void DuplicateNode(Node node)
        {
            Graph.Duplicate(node);
            CloseContextMenu();
        }

        private void DisconnectConnection(string line_id)
        {
            Graph.Disconnect(line_id);
            CloseContextMenu();
        }

        private void ClearConnections(Node node)
        {
            Graph.ClearConnectionsOf(node);
            CloseContextMenu();
        }

    }
}