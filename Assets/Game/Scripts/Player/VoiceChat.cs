using UnityEngine;
using Mirror;
using Steamworks;
using System;

public class VoiceChat : NetworkBehaviour
{
    private AudioSource audioSource;
    private AudioClip micClip;
    private float[] audioBuffer;
    private int sampleRate = 44100;
    private int bufferSize = 1024; // меньше задержка
    private bool isRecording = false;
    private string selectedMic;
    private bool wasRecording = false;

    private SkinLoader skinLoader;

    // Иконки
    public GameObject otherPlayerIcon;
    public GameObject localPlayerIcon;

    // Для воспроизведения входящего звука
    private float[] playbackBuffer;
    private int writePos = 0;
    private int readPos = 0;
    private bool isPlaying = false;

    private int jitterSamples;   // сколько держим в запасе (0.2 сек)
    private int bufferedSamples; // сколько реально накоплено

    // Флаг, чтобы не воспроизводить свой голос
    private bool isSendingVoiceData = true;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;

        jitterSamples = (int)(sampleRate * 0.2f); // 200мс буфера

        if (isLocalPlayer)
        {
            skinLoader = GetComponent<SkinLoader>();
            audioSource.volume = 0f; // не воспроизводим свой голос

            if (Microphone.devices.Length == 0)
            {
                Debug.LogError("Микрофон не найден!");
                return;
            }

            selectedMic = Microphone.devices[settingsController.micronum];
            Debug.Log($"Выбран микрофон: {selectedMic}");

            if (SteamManager.Initialized)
                Debug.Log($"Voice chat initialized for player: {SteamUser.GetSteamID()}");
            else
                Debug.LogWarning("Steam не инициализирован!");

            audioBuffer = new float[bufferSize];

            if (localPlayerIcon != null) localPlayerIcon.SetActive(false);
            if (otherPlayerIcon != null) otherPlayerIcon.SetActive(false);
        }
        else
        {
            if (otherPlayerIcon != null) otherPlayerIcon.SetActive(true);
            if (localPlayerIcon != null) localPlayerIcon.SetActive(false);

            // Буфер для входящего звука (2 секунды)
            playbackBuffer = new float[sampleRate * 2];
            AudioClip clip = AudioClip.Create("RemoteStream", playbackBuffer.Length, 1, sampleRate, true, OnAudioRead, OnAudioSetPosition);
            audioSource.clip = clip;
            audioSource.loop = true;
        }
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.V) && !isRecording) StartRecording();
            else if (Input.GetKeyUp(KeyCode.V) && isRecording) StopRecording();

            if (isRecording)
                SendVoiceData();
        }

        UpdateIconsVisibility();
    }

    private void StartRecording()
    {
        if (string.IsNullOrEmpty(selectedMic)) return;

        if (!wasRecording)
        {
            micClip = Microphone.Start(selectedMic, true, 1, sampleRate);
            if (micClip == null)
            {
                Debug.LogError("Не удалось начать запись с микрофона!");
                return;
            }

            audioSource.clip = micClip;
            audioSource.loop = true;
            audioSource.Play();

            isRecording = true;
            wasRecording = true;
            isSendingVoiceData = true;
            Debug.Log("Запись начата");
        }
    }

    private void StopRecording()
    {
        if (string.IsNullOrEmpty(selectedMic)) return;

        if (wasRecording)
        {
            Microphone.End(selectedMic);
            audioSource.Stop();
            isRecording = false;
            wasRecording = false;
            isSendingVoiceData = false;
            Debug.Log("Запись остановлена");
        }
    }

    private void SendVoiceData()
    {
        int micPosition = Microphone.GetPosition(selectedMic);
        if (micPosition < bufferSize) return;

        micClip.GetData(audioBuffer, micPosition - bufferSize);

        bool hasData = false;
        foreach (float sample in audioBuffer)
        {
            if (Mathf.Abs(sample) > 0.001f) { hasData = true; break; }
        }

        if (!hasData) return;

        byte[] voiceData = FloatToByteArray(audioBuffer);
        CmdSendVoiceData(voiceData);
    }

    [Command]
    private void CmdSendVoiceData(byte[] voiceData)
    {
        RpcReceiveVoiceData(voiceData);
    }

    [ClientRpc]
    private void RpcReceiveVoiceData(byte[] voiceData)
    {
        if (isLocalPlayer || !isSendingVoiceData) return;

        float[] audioData = ByteToFloatArray(voiceData);

        // Записываем входящие данные в буфер (кольцевой)
        for (int i = 0; i < audioData.Length; i++)
        {
            playbackBuffer[writePos] = audioData[i];
            writePos = (writePos + 1) % playbackBuffer.Length;
            bufferedSamples++;
        }

        // Запускаем воспроизведение только когда есть запас
        if (!isPlaying && bufferedSamples > jitterSamples)
        {
            audioSource.Play();
            isPlaying = true;
        }
    }

    private void OnAudioRead(float[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            if (bufferedSamples > 0)
            {
                data[i] = playbackBuffer[readPos];
                playbackBuffer[readPos] = 0f;
                readPos = (readPos + 1) % playbackBuffer.Length;
                bufferedSamples--;
            }
            else
            {
                data[i] = 0f; // тишина если данных нет
            }
        }
    }

    private void OnAudioSetPosition(int newPosition)
    {
        readPos = newPosition % playbackBuffer.Length;
    }

    private byte[] FloatToByteArray(float[] floatArray)
    {
        byte[] byteArray = new byte[floatArray.Length * 4];
        Buffer.BlockCopy(floatArray, 0, byteArray, 0, byteArray.Length);
        return byteArray;
    }

    private float[] ByteToFloatArray(byte[] byteArray)
    {
        float[] floatArray = new float[byteArray.Length / 4];
        Buffer.BlockCopy(byteArray, 0, floatArray, 0, byteArray.Length);
        return floatArray;
    }

    private void OnDestroy()
    {
        if (isRecording) StopRecording();
    }

    private void UpdateIconsVisibility()
    {
        if (isLocalPlayer)
        {
            if (localPlayerIcon != null)
                localPlayerIcon.SetActive(isRecording);
        }
        else
        {
            if (otherPlayerIcon != null)
                otherPlayerIcon.SetActive(isRecording);
        }
    }
}