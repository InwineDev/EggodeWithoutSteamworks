using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

namespace RuntimeNodeEditor.Eggode
{
    public class BooledNode : Node
    {
        public TMP_Dropdown dropdown;
        public SocketInput inputSocket;
        public SocketInput inputSocket2;
        public SocketOutput outputSocket;

        private List<IOutput> _incomingOutputs;


        public override void Setup()
        {
            _incomingOutputs = new List<IOutput>();

            Register(outputSocket);
            Register(inputSocket);
            Register(inputSocket2);

            SetHeader("BooledNode");
            outputSocket.SetValue(0f);

            dropdown.AddOptions(new List<TMP_Dropdown.OptionData>()
            {
                new TMP_Dropdown.OptionData(comparisons.equalequal.ToString()),
                new TMP_Dropdown.OptionData(comparisons.more.ToString()),
                new TMP_Dropdown.OptionData(comparisons.less.ToString()),
                new TMP_Dropdown.OptionData(comparisons.unequal.ToString())
            });

            dropdown.onValueChanged.AddListener(selected =>
            {
                OnConnectedValueUpdated();
            });

            OnConnectionEvent += OnConnection;
            OnDisconnectEvent += OnDisconnect;
        }

        public void OnConnection(SocketInput input, IOutput output)
        {
            output.ValueUpdated += OnConnectedValueUpdated;
            _incomingOutputs.Add(output);

            OnConnectedValueUpdated();
        }

        public void OnDisconnect(SocketInput input, IOutput output)
        {
            output.ValueUpdated -= OnConnectedValueUpdated;
            _incomingOutputs.Remove(output);

            OnConnectedValueUpdated();
        }

        public override void OnSerialize(Serializer serializer)
        {
            var output = outputSocket.GetValue<float>();
            serializer.Add("opType", dropdown.value.ToString());
        }

        public override void OnDeserialize(Serializer serializer)
        {
            var opType = int.Parse(serializer.Get("opType"));
            dropdown.SetValueWithoutNotify(opType);

            OnConnectedValueUpdated();
        }

        private void OnConnectedValueUpdated()
        {
        }

    }
}
