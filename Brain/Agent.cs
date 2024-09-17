using System;
using System.Net.Sockets;
using Terraria;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Text;

namespace TAgent.Brain
{
    public abstract class Agent
    {
        public bool active
        {
            get { return client.Connected; }
        }
        private float reward;
        private VectorSensor sensor = new();
        private readonly int port;
        private readonly string addr;

        private TcpClient client;

        public Agent(string addr, int port)
        {
            this.addr = addr;
            this.port = port;

            StartClient();
        }
        public void StartClient()
        {
            try
            {
                client = new(addr, port);
                var stream = client.GetStream();
                var info = new byte[] { 1, 1 };
                stream.Write(info, 0, 2);
            }
            catch (Exception e)
            {
                Main.NewText($"Exception: {e.Message}");
            }
        }
        public void SetReward(float value)
        {
            reward = value;
        }
        public void AddReward(float value)
        {
            reward += value;
        }
        public abstract void CollectObservations(VectorSensor sensor);

        public abstract void Heuristic(float[] actions);

        public abstract void OnBegin();

        private void SendSensor()
        {
            var bytes = new byte[sensor.ObservationCount * 4];
            Buffer.BlockCopy(sensor.GetObservations(), 0, bytes, 0, bytes.Length);

            NetworkStream stream = client.GetStream();

            stream.Write(BitConverter.GetBytes(reward), 0, 4);
            stream.Write(bytes, 0, bytes.Length);
        }
        private float[] ReadActions()
        {
            var actions = new float[64];
            NetworkStream stream = client.GetStream();
            var read = new byte[256]; //num discretes
            stream.Read(read, 0, read.Length);
            for (int i = 0; i < read.Length; i += 4)
            {
                float action = BitConverter.ToSingle(read, i);
                actions[i / 4] = action;
            }
            return actions;
        }
        public void Update()
        {
            Main.NewText("agent update");
            CollectObservations(sensor);
            SendSensor();

            sensor.Clear();

            //var actions = ReadActions();
            //Heuristic(actions);
        }
        public void Close()
        {
            NetworkStream stream = client.GetStream();
            byte[] stop = Encoding.ASCII.GetBytes("stop");
            stream.Write(stop, 0, 4);
            client.Dispose();
        }
    }
}

public class VectorSensor
{
    private List<float> observations;

    public VectorSensor()
    {
        observations = new List<float>();
    }

    // Add a single observation
    public void AddObservation(float value)
    {
        observations.Add(value);
    }
    public void AddVector2(Vector2 vector)
    {
        observations.Add(vector.X);
        observations.Add(vector.Y);
    } 

    // Add an array of observations
    public void AddObservation(float[] values)
    {
        observations.AddRange(values);
    }

    // Retrieve all observations as an array
    public float[] GetObservations()
    {
        return observations.ToArray();
    }

    // Clear the observations for a new episode
    public void Clear()
    {
        observations.Clear();
    }

    // Get the count of observations
    public int ObservationCount => observations.Count;
}