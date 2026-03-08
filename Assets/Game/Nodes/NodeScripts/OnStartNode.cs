using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using RuntimeNodeEditor;

public class OnStartNode : Node
    {
        public SocketOutput outputSocket;
    public string header;

        public override void Setup()
        {
            Register(outputSocket);
            SetHeader(header);

        }
    }