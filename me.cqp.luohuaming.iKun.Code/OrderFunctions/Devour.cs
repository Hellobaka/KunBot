using me.cqp.luohuaming.iKun.PublicInfos;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using me.cqp.luohuaming.iKun.Sdk.Cqp.EventArgs;
using me.cqp.luohuaming.iKun.Sdk.Cqp.Model;
using System;
using System.Linq;

namespace me.cqp.luohuaming.iKun.Code.OrderFunctions
{
    public class Devour : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => AppConfig.CommandDevour;

        public bool Judge(string destStr) => destStr.Replace("＃", "#").StartsWith(GetOrderStr());

        public FunctionResult Execute(CQGroupMessageEventArgs e)
        {
            FunctionResult result = new FunctionResult
            {
                Result = true,
                SendFlag = true,
            };
            SendText sendText = new SendText
            {
                SendID = e.FromGroup,
            };
            result.SendObject.Add(sendText);
            long target = -1;
            if (!e.Message.CQCodes.Any(x => x.Function == Sdk.Cqp.Enum.CQFunction.At)
                || !e.Message.CQCodes.First(x => x.Function == Sdk.Cqp.Enum.CQFunction.At).Items.TryGetValue("qq", out string targetValue)
                || !long.TryParse(targetValue, out target))
            {
                // 非CQ码，尝试解析昵称、卡片与QQ
                string raw = e.Message.Text.Substring(GetOrderStr().Length).Trim();
                if (string.IsNullOrEmpty(raw))
                {
                    // 无有效指令
                    sendText.MsgToSend.Add(string.Format(AppConfig.ReplyParamInvalid, $"或无法找到目标，示例：{GetOrderStr()} [QQ|At|昵称|卡片]"));
                    return result;
                }
                if (!long.TryParse(raw, out target))
                {
                    // 获取昵称与卡片列表
                    if (!MainSave.GroupMemberInfos.TryGetValue(e.FromGroup, out var infos))
                    {
                        infos = e.FromGroup.GetGroupMemberList();
                        MainSave.GroupMemberInfos[e.FromGroup] = infos;
                    }
                    target = infos.FirstOrDefault(x => x.Nick.Contains(raw) || x.Card.Contains(raw)).QQ;
                }
            }

            if (target < QQ.MinValue)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyParamInvalid, $"，示例：{GetOrderStr()} [QQ|At|昵称|卡片]"));
                return result;
            }
            if (target == e.FromQQ)
            {
                sendText.MsgToSend.Add(AppConfig.ReplyDevourSelf);
                return result;
            }

            var player = Player.GetPlayer(e.FromQQ);
            if (player == null)
            {
                sendText.MsgToSend.Add(AppConfig.ReplyNoPlayer);
                return result;
            }
            var targetPlayer = Player.GetPlayer(target);
            if (targetPlayer == null)
            {
                sendText.MsgToSend.Add(AppConfig.ReplyNoTargetPlayer);
                return result;
            }
            var kun = Kun.GetKunByQQ(player.QQ);
            if (kun == null)
            {
                sendText.MsgToSend.Add(AppConfig.ReplyNoKun);
                return result;
            }
            var targetKun = Kun.GetKunByQQ(targetPlayer.QQ);
            if (targetKun == null)
            {
                sendText.MsgToSend.Add(AppConfig.ReplyNoTargetPlayerKun);
                return result;
            }
            kun.Initialize();
            targetKun.Initialize();

            if (DateTime.Now - player.DevourAt < TimeSpan.FromMinutes(AppConfig.ValueDevourCD))
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyDevourInCD, (player.DevourAt.AddMinutes(30)).ToString("G")));
                return result;
            }

            var r = kun.Devour(targetKun);
            if (r.Success is false)
            {
                sendText.MsgToSend.Add("吞噬方法过程发生异常，查看日志获取更多信息");
                return result;
            }
            player.DevourAt = DateTime.Now;
            player.Update();
            string playerInfo = AppConfig.EnableAt ? $"[CQ:at,qq={target}]" : e.FromGroup.GetGroupMemberInfo(target).Card;
            if (r.Dead)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyDevourFailAndDead, kun.ToString(), playerInfo, targetKun.ToString(), r.TargetDecrement.ToShortNumber(), r.TargetCurrentWeight.ToShortNumber()));
            }
            else if (r.Increment < 0)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyDevourFail, kun.ToString(), playerInfo, targetKun.ToString(), r.Increment.ToShortNumber(), r.CurrentWeight.ToShortNumber(), r.TargetDecrement.ToShortNumber(), r.TargetCurrentWeight.ToShortNumber()));
            }
            else if (r.Escaped)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyDevourEscaped, kun.ToString(), playerInfo, targetKun.ToString()));
            }
            else
            {
                string send = string.Format(AppConfig.ReplyDevourSuccess, kun.ToString(), playerInfo, targetKun.ToString(), r.Increment.ToShortNumber(), r.CurrentWeight.ToShortNumber(), r.TargetDecrement.ToShortNumber(), r.TargetCurrentWeight.ToShortNumber());
                if (r.WeightLimit)
                {
                    send += $"\n{AppConfig.ReplyWeightLimit}";
                }
                sendText.MsgToSend.Add(send);
            }
            return result;
        }

        public FunctionResult Execute(CQPrivateMessageEventArgs e)
        {
            FunctionResult result = new FunctionResult
            {
                Result = false,
                SendFlag = false,
            };
            return result;
        }
    }
}