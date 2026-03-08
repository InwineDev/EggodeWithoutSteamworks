using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace RuntimeNodeEditor.Eggode
{
    public class GetVarNode : Node
    {
        public TMP_InputField valueField;
        public SocketOutput outputSocket;
        public SocketInput objectSocket;

        public override void Setup()
        {
            Register(outputSocket);
            Register(objectSocket);
            SetHeader("get var");

            valueField.text = "NULL";
            HandleFieldValue(valueField.text);

            valueField.contentType = TMP_InputField.ContentType.Standard;
            valueField.onEndEdit.AddListener(HandleFieldValue);
            OnConnectionEvent += OnConnection;
            OnDisconnectEvent += OnDisconnect;
        }
        public void OnConnection(SocketInput input, IOutput output)
        {
            output.ValueUpdated += OnConnectedValueUpdated;
            OnConnectedValueUpdated();
        }

        public void OnDisconnect(SocketInput input, IOutput output)
        {
            output.ValueUpdated -= OnConnectedValueUpdated;
            OnConnectedValueUpdated();
        }
        private void OnConnectedValueUpdated()
        {

        }
        private void HandleFieldValue(string value)
        {
            outputSocket.SetValue(value);
        }

        public override void OnSerialize(Serializer serializer)
        {
            serializer.Add("keyGet", valueField.text);
        }

        public override void OnDeserialize(Serializer serializer)
        {
            var value = serializer.Get("keyGet");
            valueField.SetTextWithoutNotify(value);

            HandleFieldValue(value);
        }
    }
}
