using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace RuntimeNodeEditor.Eggode
{
    public class StringNode : Node
    {
        public TMP_InputField valueField;
        public SocketOutput outputSocket;

        public override void Setup()
        {
            Register(outputSocket);
            SetHeader("string");

            valueField.text = "NULL";
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
            serializer.Add("stringValue", valueField.text);
        }

        public override void OnDeserialize(Serializer serializer)
        {
            var value = serializer.Get("stringValue");
            valueField.SetTextWithoutNotify(value);

            HandleFieldValue(value);
        }
    }
}
