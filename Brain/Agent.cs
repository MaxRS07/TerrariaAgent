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
        public bool Active
        {
            get { return client.Connected; }
        }
        private float reward;
        private VectorSensor sensor = new();
        private readonly int port;
        private readonly string addr;

        private readonly int numObservations;
        private readonly int numDiscrete;
        private readonly int numContinuous;

        private TcpClient client;

        public Agent(string addr, int port, int numDiscrete, int numContinuous, int numObservations)
        {
            this.addr = addr;
            this.port = port;
            this.numObservations = numObservations;
            this.numDiscrete = numDiscrete;
            this.numContinuous = numContinuous;

            StartClient();
        }
        public void StartClient()
        {
            try
            {
                client = new(addr, port);

                var info = new byte[12];
                var discrete = BitConverter.GetBytes(numDiscrete);
                var continuous = BitConverter.GetBytes(numContinuous);
                var observations = BitConverter.GetBytes(numObservations);

                observations.CopyTo(info, 0);
                discrete.CopyTo(info, 4);
                continuous.CopyTo(info, 8);

                var stream = client.GetStream();
                stream.Write(info, 0, 12);
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

        /// <summary>
        /// probably set rewards here
        /// </summary>
        /// <param name="actions"></param>
        public abstract void Heuristic(float[] actions);

        public abstract void OnBegin();

        private void SendSensor()
        {
            if (sensor.ObservationCount > numObservations)
            {
                Main.NewText($"Warning: Sensor exceeds expected length", Color.Red);
                Close();
            }
            var obs = new float[numObservations];
            for (int i = 0; i < sensor.ObservationCount; i++)
            {
                var ob = sensor.GetObservations()[i];
                obs[i] = ob;
            }
            var bytes = new byte[4 + sensor.ObservationCount * 4];
            Buffer.BlockCopy(sensor.GetObservations(), 0, bytes, 4, sensor.ObservationCount * 4);
            BitConverter.GetBytes(reward).CopyTo(bytes, 0);
            NetworkStream stream = client.GetStream();
            stream.Write(bytes, 0, bytes.Length);
            sensor.Clear();
        }
        private float[] ReadActions()
        {
            var actions = new float[numContinuous + numDiscrete];
            NetworkStream stream = client.GetStream();
            var read = new byte[actions.Length * 4];
            if (read.Length < 4)
            {
                return Array.Empty<float>();
            }
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
            CollectObservations(sensor);
            SendSensor();

            sensor.Clear();

            if (client.Available >= 4)
            {
                var actions = ReadActions();
                Heuristic(actions);
            }
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