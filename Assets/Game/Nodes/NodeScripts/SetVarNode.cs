using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace RuntimeNodeEditor.Eggode
{
    public class SetVarNode : Node
    {
        public TMP_InputField valueField;
        public SocketOutput outputSocket;
        public SocketInput objectSocket;
        public SocketInput triggerSocket;
        public SocketInput varSocket;

        public override void Setup()
        {
            Register(outputSocket);
            Register(objectSocket);
            Register(triggerSocket);
            Register(varSocket);
            SetHeader("set var");

            valueField.text = "NULL";
            HandleFieldValue(valueField.text);

            valueField.contentType = TMP_InputField.ContentType.Standard;
            valueField.onEndEdit.AddListener(HandleFieldValue);

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
        private void HandleFieldValue(string value)
        {
            outputSocket.SetValue(value);
        }

        public override void OnSerialize(Serializer serializer)
        {
            serializer.Add("keySet", valueField.text);
        }

        public override void OnDeserialize(Serializer serializer)
        {
            var value = serializer.Get("keySet");
            valueField.SetTextWithoutNotify(value);

            HandleFieldValue(value);
        }
    }
}
