using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RuntimeNodeEditor.Eggode
{
    public class ForNode : Node
    {
        public SocketInput inputSocket;
        public SocketInput timeSocket;
        public SocketOutput outputSocket;
        public SocketOutput outputSocketElse;

        private List<IOutput> _incomingOutputs;

        public override void Setup()
        {
            _incomingOutputs = new List<IOutput>();

            Register(outputSocket);
            Register(outputSocketElse);
            Register(inputSocket);
            Register(timeSocket);

            SetHeader("for node");

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
        public override void OnSerialize(Serializer serializer)
        {
            // При необходимости можно добавить сериализацию состояния
        }

        public override void OnDeserialize(Serializer serializer)
        {
            // При необходимости можно добавить десериализацию
        }
    }
}