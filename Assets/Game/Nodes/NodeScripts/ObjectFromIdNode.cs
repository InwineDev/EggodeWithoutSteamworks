using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace RuntimeNodeEditor.Eggode
{
    public class ObjectFromIdNode : Node
    {
        public TMP_InputField valueField;
        public SocketOutput outputSocket;

        public override void Setup()
        {
            Register(outputSocket);
            SetHeader("ObjectFromIdNode");

            valueField.text = "";
            HandleFieldValue(valueField.text);

            valueField.contentType = TMP_InputField.ContentType.Standard;
            valueField.onEndEdit.AddListener(HandleFieldValue);
        }

        private void HandleFieldValue(string value)
        {
            outputSocket.SetValue(value);
        }

        public override void OnSerialize(Serializer serializer)
        {
            serializer.Add("idValue", valueField.text);
        }

        public override void OnDeserialize(Serializer serializer)
        {
            var value = serializer.Get("idValue");
            valueField.SetTextWithoutNotify(value);

            HandleFieldValue(value);
        }
    }
}
