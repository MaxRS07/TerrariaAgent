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
            get => agent != null && agent.Active;
        }

        public override void PreUpdateMovement()
        {
            base.PreUpdateMovement();
        }
        public override void PostUpdate()
        {
            
            base.PostUpdate();
        }
        public override void PreUpdate()
        {
            if (agent != null)
            {
                if (agent.Active)
                    agent.Update();
            }
            base.PreUpdate();
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (KeybindSystem.Train.JustPressed)
            {
                if (!Training)
                {
                    agent = new PlayerAgent(Player.whoAmI, 2, 2);
                    if (agent.Active)
                        Main.NewText($"Training Started");
                    else
                        Main.NewText("Connection Failed :(");
                } else
                {
                    agent?.Close();
                    Main.NewText($"Training Ended. Brain Saved.");
                }
            }
            base.ProcessTriggers(triggersSet);
        }
    }
    public class PlayerAgent : Agent
    {
        private readonly int playerWhoAmI;

        private Player Player
        {
            get => Main.player[playerWhoAmI];
        }

        /// <summary>
        /// Makes a player agent
        /// </summary>
        /// <param name="player"></param>
        /// <param name="numObservations">number of floating point observations inputted</param>
        /// <param name="numActions">number of floating point actions returned</param>
        /// <param name="server"></param>
        /// <param name="port"></param>
        public PlayerAgent(int player, int numObservations, int numActions, string server = "127.0.0.1", int port = 65432) : base(server, port, numObservations, numActions) {
            playerWhoAmI = player;
        }
        public override void OnBegin()
        {
            
        }
        public override void Heuristic(float[] actions)
        {
            Main.NewText(string.Join(", ", actions));
            if (actions[0] > actions[1]) {
                PlayerInput.Triggers.Current.Left = true;
            } else
            {
                PlayerInput.Triggers.Current.Right = true;
            }

            SetReward(Player.direction);
        }
        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddVector2(new(1f, 1f));
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