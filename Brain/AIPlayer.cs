using System;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using TAgent.System;
using Terraria.GameInput;

namespace TAgent.Brain
{
	public class AIPlayer : ModPlayer
	{
        #nullable enable
        private PlayerAgent? agent;

        public bool Training
        {
            get => agent != null && agent.active;
        }

        public override void PreUpdateMovement()
        {
            if (agent != null)
            {
                if (agent.active)
                    agent.Update();
            }
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (KeybindSystem.Train.JustPressed)
            {
                if (!Training)
                {
                    agent = new PlayerAgent(Player);
                    if (agent.active)
                        Main.NewText($"Training Started");
                    else
                        Main.NewText("Connection Failed :(");
                } else
                {
                    agent.
                }
            }
        }
    }
    public class PlayerAgent : Agent
    {
        private readonly Player player;

        public PlayerAgent(Player player, string server = "127.0.0.1", int port = 65432) : base(server, port) {
            this.player = player;
        }
        public override void OnBegin()
        {
            
        }
        public override void Heuristic(float[] actions)
        {
            if (Math.Round(actions[0]) == 1) {
                player.JumpMovement();
            }
        }
        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(1.0f);
            //sensor.AddVector2(player.velocity);
            //sensor.AddVector2(player.position - Main.screenPosition);

            //for (int i = 0; i < Main.maxNPCs; i++)
            //{
            //    var npc = Main.npc[i];
            //    if (npc.active)
            //    {
            //        var screenPos = Main.screenPosition - npc.Center;
            //        if (screenPos.X >= -Main.screenWidth && screenPos.X <= 0 && screenPos.Y >= -Main.screenHeight && screenPos.Y <= 0)
            //        {
            //            if (!npc.friendly)
            //            {
            //                sensor.AddVector2(npc.Center);
            //            }
            //        }
            //    }
            //}
        }
    }
}