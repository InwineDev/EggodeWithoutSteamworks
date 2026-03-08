using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RuntimeNodeEditor.Eggode
{
    public class SleepNode : Node
    {
        public TMP_Text resultText;
        public SocketInput inputSocket;
        public SocketInput timeSocket;
        public SocketOutput outputSocket;

        private List<IOutput> _incomingOutputs;

        public override void Setup()
        {
            _incomingOutputs = new List<IOutput>();

            Register(outputSocket);
            Register(inputSocket);
            Register(timeSocket);

            SetHeader("time sleep");
            outputSocket.SetValue(0f);

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
            try
            {

            }
            catch
            {

            }
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