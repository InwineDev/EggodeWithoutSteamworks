using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace RuntimeNodeEditor.Eggode
{
    public class IntNode : Node
    {
        public TMP_InputField valueField;
        public SocketOutput outputSocket;

        public override void Setup()
        {
            Register(outputSocket);
            SetHeader("int");

            valueField.text = "0";
            HandleFieldValue(valueField.text);

            valueField.contentType = TMP_InputField.ContentType.IntegerNumber;
            valueField.onEndEdit.AddListener(HandleFieldValue);
        }

        private void HandleFieldValue(string value)
        {
            float floatValue = float.Parse(value);
            outputSocket.SetValue(floatValue);
        }

        public override void OnSerialize(Serializer serializer)
        {
            serializer.Add("intValue", valueField.text);
        }

        public override void OnDeserialize(Serializer serializer)
        {
            var value = serializer.Get("intValue");
            valueField.SetTextWithoutNotify(value);

            HandleFieldValue(value);
        }
    }
}
