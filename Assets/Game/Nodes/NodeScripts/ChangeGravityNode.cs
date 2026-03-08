using UnityEngine;
using TMPro;

namespace RuntimeNodeEditor.Eggode
{
    public class ChangeGravityNode : Node
    {
        public TMP_Text statusText;
        public SocketInput triggerSocket;  // Вход для активации уничтожения
        public SocketInput objectSocket;   // Вход для объекта
        public SocketInput boolSocket;   // Вход для bool
        public SocketOutput outputSocket;   // Выход для выполнения следующих действий

        public override void Setup()
        {
            Register(triggerSocket);
            Register(objectSocket);
            Register(boolSocket);
            Register(outputSocket);

            SetHeader("ChangeGravity");

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