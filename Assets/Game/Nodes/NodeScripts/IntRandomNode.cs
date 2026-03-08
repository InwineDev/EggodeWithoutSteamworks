using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace RuntimeNodeEditor.Eggode
{
    public class IntRandomNode : Node
    {
        public TMP_InputField valueFieldFrom;
        public TMP_InputField valueFieldTo;
        public SocketOutput outputSocket;

        public override void Setup()
        {
            Register(outputSocket);
            SetHeader("IntRandomNode");

            valueFieldFrom.text = "0";
            HandleFieldValue(valueFieldFrom.text);

            valueFieldFrom.contentType = TMP_InputField.ContentType.IntegerNumber;
            valueFieldFrom.onEndEdit.AddListener(HandleFieldValue);

            valueFieldTo.text = "0";
            HandleFieldValue(valueFieldTo.text);

            valueFieldTo.contentType = TMP_InputField.ContentType.IntegerNumber;
            valueFieldTo.onEndEdit.AddListener(HandleFieldValue);
        }

        private void HandleFieldValue(string value)
        {
            float floatValue = float.Parse(value);
            outputSocket.SetValue(floatValue);
        }

        public override void OnSerialize(Serializer serializer)
        {
            serializer.Add("intValueFrom", valueFieldFrom.text);
            serializer.Add("intValueTo", valueFieldTo.text);
        }

        public override void OnDeserialize(Serializer serializer)
        {
            var value = serializer.Get("intValueFrom");
            valueFieldFrom.SetTextWithoutNotify(value);

            HandleFieldValue(value);

            var value1 = serializer.Get("intValueTo");
            valueFieldTo.SetTextWithoutNotify(value1);

            HandleFieldValue(value1);
        }
    }
}
