using BNB;
using BNB.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Sandbox.PhysicsContact;
using static System.Net.Mime.MediaTypeNames;

namespace payload
{
    internal class ChatCommands
    {
        public static void LocalChatCommand(string message)
        {
            var parse_message = message.ToLower().Replace("alexa,", "");

            bool success = false;
            string AlexaResponse = "Okay.";

            if(parse_message.Contains("kill "))
            {
                string kill_target = parse_message.Replace("kill ", "");

                if(kill_target.Contains("everyone"))
                {
                    success = KillAllPlayers();
                }
                else
                {
                    success = KillPlayer(kill_target);
                }
            }

            //if(parse_message.Contains("make me a ghost"))
            //{
            //    Ghost.enabled = true;

            //    success = true;
            //}

            //if(parse_message.Contains("return me to my body"))
            //{
            //    Ghost.enabled = false;

            //    success = true;
            //}

            if(parse_message.Contains("aim for me"))
            {
                Aimbot.enabled = true;
                success = true;
            }

            if(parse_message.Contains("stop aiming for me"))
            {
                Aimbot.enabled = false;
                success = true;
            }

            if (parse_message.Contains("set aim fov to "))
            {
                string parse_float = parse_message.Replace("set aim FOV to ", "");
                float fov = parse_message.ToFloat(45f);

                Utils.Log($"Setting FOV to {fov}");
                Aimbot.AimFOV = fov;
                success = true;
            }

            if(parse_message.Contains("aim mode"))
            {
                if(parse_message.Contains("rage"))
                {
                    Aimbot.RageMode = true;
                    success = true;
                }

                else if(parse_message.Contains("normal"))
                {
                    Aimbot.RageMode = false;
                    success = true;
                }
            }

            if(parse_message.Contains("rain"))
            {
                if(parse_message.Contains("rain death on "))
                {
                    var player_name = parse_message.Replace("rain death on ", "");
                    var p = Utils.GetPlayerByName(player_name);

                    if (p != null)
                    {
                        Grenade.GrenadeTargets.Add(p);
                        success = true;
                    }
                }
                else if(parse_message.Contains("stop raining on "))
                {
                    var player_name = parse_message.Replace("stop raining on ", "");
                    var p = Utils.GetPlayerByName(player_name);
                    if (p != null)
                    {
                        Grenade.GrenadeTargets.Remove(p);
                        success = true;
                    }
                }
            }

            if(parse_message.Contains("create bot"))
            {
                var local = Utils.GetLocalPlayer();

                if (local != null && local.IsAlive && local.IsValid() && GameManager.CurrentState != null)
                {
                    Bot.CreateBot(GameManager.CurrentState, local.Team);
                    success = true;
                }
            }

            if (parse_message.Contains("judge") || parse_message.Contains("judging"))
            {
                if (parse_message.Contains("judge "))
                {
                    var player_name = parse_message.Replace("judge ", "");
                    var p = Utils.GetPlayerByName(player_name);

                    if (p != null)
                    {
                        Judge.JudgeTargets.Add(p);
                        success = true;

                        AlexaResponse = $"{player_name}, you have been judged and found to be a threat to my success.";
                    }
                }
                else if (parse_message.Contains("stop judging "))
                {
                    var player_name = parse_message.Replace("stop judging ", "");
                    var p = Utils.GetPlayerByName(player_name);

                    if (p != null)
                    {
                        Judge.JudgeTargets.Remove(p);
                        success = true;

                        //AlexaResponse = $"{player_name}, you have been judged and found to be a threat to my success.";
                    }
                }
            }

            if (!success)
            {
                AlexaResponse = "Sorry, I'm having trouble understanding you right now.";
            }
            Chatbox.Message msg = new Chatbox.Message
            {
                Author = "System",
                Channel = Chatbox.ChannelType.Global,
                Text = AlexaResponse,
                Lifetime = 6f
            };

            ChatManager.SendMessage(msg);
        }

        public static bool KillPlayer(string player)
        {

            var target = Utils.GetPlayerByName(player);

            if (target == null)
            {
                Utils.Log($"Did not find a player with name {player}");
                return false;
            }

            return KillPlayer(target);
        }

        public static bool KillPlayer(BNB.Player player)
        {
            var target = player;

            if (player == null)
                return false;

            if (target.Unit == null)
            {
                Utils.Log($"player had a null unit, cannot continue");
                return false;
            }

            var attacker = Utils.GetLocalPlayer()?.Unit;
            var dmg_type = DamageType.Melee;

            if (attacker == null)
            {
                Utils.Log("Local unit was null - causing a suicide instead");
                attacker = target.Unit;
                dmg_type = DamageType.Cyanide;
            }
            else if (attacker.Team == target.Team && target.Team != Team.Unaligned)
            {
                Utils.Log($"Sharing the same team as player - suicide it is.");
                attacker = target.Unit;
                dmg_type = DamageType.Cyanide;
            }

            target.Unit.TryDamage(new BNB.DamageInfo
            {
                Type = dmg_type,
                Damage = int.MaxValue,
                Attacker = attacker
            });

            return true;
        }

        public static bool KillAllPlayers()
        {
            bool killed_one_or_more = false;

            var local = Utils.GetLocalPlayer();

            if (local == null)
            {
                return false;
            }

            foreach (var p in BNB.Player.GetAllLiving())
            {
                if (local == p)
                    continue;

                bool single_kill = false;

                single_kill = KillPlayer(p);

                if (single_kill && !killed_one_or_more)
                    killed_one_or_more = true;

            }

            return killed_one_or_more;
        }
    }
}
