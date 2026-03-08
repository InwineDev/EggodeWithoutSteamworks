using UnityEngine;
using TMPro;

namespace RuntimeNodeEditor.Eggode
{
    public class TpNode : Node
    {
        public TMP_Text statusText;
        public SocketInput triggerSocket;  // Вход для активации уничтожения
        public SocketInput VectorSocket;
        //public SocketInput ObjToTp;
        public SocketOutput outputSocket;   // Выход для выполнения следующих действий

        public override void Setup()
        {
            Register(triggerSocket);
            Register(VectorSocket);
            //Register(ObjToTp);
            Register(outputSocket);

            SetHeader("TeleportNode");

            outputSocket.SetValue(false); // Изначально false

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
/*                // Получаем объект
                _targetObject = objectSocket.HasConnection()
                    ? objectSocket.GetValue<GameObject>()
                    : null;

                // Проверяем триггер
                if (triggerSocket.HasConnection() && triggerSocket.GetValue<bool>() && _targetObject != null)
                {

                }
                else
                {
                    _isDestroyed = false;
                    outputSocket.SetValue(false);
                }*/
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