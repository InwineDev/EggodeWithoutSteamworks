using UnityEngine;
using TMPro;

namespace RuntimeNodeEditor.Eggode
{
    public class ShowMessageNode : Node
    {
        public TMP_Text statusText;
        public SocketInput triggerSocket;  // Вход для активации уничтожения
        public SocketInput floatSocket;   // Вход для задержки
        public SocketInput stringSocket;   // Вход для задержки
        public SocketOutput outputSocket;   // Выход для выполнения следующих действий

        private GameObject _targetObject;
        private bool _isDestroyed;

        public override void Setup()
        {
            Register(triggerSocket);
            Register(floatSocket);
            Register(stringSocket);
            Register(outputSocket);

            SetHeader("ShowMessageNode");

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