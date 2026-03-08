using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace RuntimeNodeEditor.Eggode
{
    public class FloatRandomNode : Node
    {
        public TMP_InputField valueFieldFrom;
        public TMP_InputField valueFieldTo;
        public SocketOutput outputSocket;

        public override void Setup()
        {
            Register(outputSocket);
            SetHeader("FloatRandomNode");

            valueFieldFrom.text = "0";
            HandleFieldValue(valueFieldFrom.text);

            valueFieldFrom.contentType = TMP_InputField.ContentType.DecimalNumber;
            valueFieldFrom.onEndEdit.AddListener(HandleFieldValue);

            valueFieldTo.text = "0";
            HandleFieldValue(valueFieldTo.text);

            valueFieldTo.contentType = TMP_InputField.ContentType.DecimalNumber;
            valueFieldTo.onEndEdit.AddListener(HandleFieldValue);
        }

        private void HandleFieldValue(string value)
        {
            float floatValue = float.Parse(value);
            outputSocket.SetValue(floatValue);
        }

        public override void OnSerialize(Serializer serializer)
        {
            serializer.Add("floatValueFrom", valueFieldFrom.text);
            serializer.Add("floatValueTo", valueFieldTo.text);
        }

        public override void OnDeserialize(Serializer serializer)
        {
            var value = serializer.Get("floatValueFrom");
            valueFieldFrom.SetTextWithoutNotify(value);

            HandleFieldValue(value);

            var value1 = serializer.Get("floatValueTo");
            valueFieldTo.SetTextWithoutNotify(value1);

            HandleFieldValue(value1);
        }
    }
}
