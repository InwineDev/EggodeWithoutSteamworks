using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace RuntimeNodeEditor.Eggode
{
    public class ThisObjectNode : Node
    {
        public SocketOutput outputSocket;

        public override void Setup()
        {
            Register(outputSocket);
            SetHeader("This object");
            HandleFieldValue();
        }

        private void HandleFieldValue()
        {
            outputSocket.SetValue(gameObject);
        }
/*
        public override void OnSerialize(Serializer serializer)
        {
            serializer.Add("GameObjectValue", gameObject);
        }

        public override void OnDeserialize(Serializer serializer)
        {
            var value = serializer.Get("GameObjectValue");

            HandleFieldValue(value);
        }*/
    }
}
