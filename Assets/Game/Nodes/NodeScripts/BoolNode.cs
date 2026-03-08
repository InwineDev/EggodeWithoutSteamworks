using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace RuntimeNodeEditor.Eggode
{
    public class BoolNode : Node
    {
        public Toggle valueToggle;
        public SocketOutput outputSocket;

        public override void Setup()
        {
            Register(outputSocket);
            SetHeader("bool");

            valueToggle.isOn = false;
            HandleToggleValue(valueToggle.isOn);

            valueToggle.onValueChanged.AddListener(HandleToggleValue);
        }

        private void HandleToggleValue(bool value)
        {
            outputSocket.SetValue(value);
        }

        public override void OnSerialize(Serializer serializer)
        {
            serializer.Add("boolValue", valueToggle.isOn.ToString());
        }

        public override void OnDeserialize(Serializer serializer)
        {
            var value= bool.Parse(serializer.Get("boolValue"));
            valueToggle.isOn = value;

            HandleToggleValue(value);
        }
    }
}
