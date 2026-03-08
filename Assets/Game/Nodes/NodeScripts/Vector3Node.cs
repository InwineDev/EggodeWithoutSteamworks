using UnityEngine;
using TMPro;

namespace RuntimeNodeEditor.Eggode
{
    public class Vector3Node : Node
    {
        public TMP_Text statusText;
        public SocketInput xSocket;
        public SocketInput ySocket;
        public SocketInput zSocket; 
        public SocketOutput outputSocket;

        public override void Setup()
        {
            Register(xSocket);
            Register(ySocket);
            Register(zSocket);
            Register(outputSocket);

            SetHeader("Vector3");

            outputSocket.SetValue(false);

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