using me.cqp.luohuaming.iKun.PublicInfos;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using me.cqp.luohuaming.iKun.Sdk.Cqp;
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

        /// <summary>
        /// 执行吞噬操作的方法。
        /// </summary>
        /// <param name="e">事件参数，包含消息来源和内容。</param>
        /// <returns>执行结果，包括是否成功、是否发送消息以及发送的消息内容。</returns>
        public FunctionResult Execute(CQGroupMessageEventArgs e)
        {
            // 初始化执行结果
            FunctionResult result = new FunctionResult
            {
                Result = true,
                SendFlag = true,
            };

            // 创建发送文本对象并设置发送的目标群
            SendText sendText = new SendText
            {
                SendID = e.FromGroup,
            };
            result.SendObject.Add(sendText);

            // 尝试从消息中解析出目标QQ号
            long target = -1;
            if (!e.Message.CQCodes.Any(x => x.Function == Sdk.Cqp.Enum.CQFunction.At)
                || !e.Message.CQCodes.First(x => x.Function == Sdk.Cqp.Enum.CQFunction.At).Items.TryGetValue("qq", out string targetValue)
                || !long.TryParse(targetValue, out target))
            {
                // 如果没有@，则尝试解析昵称、卡片或QQ号
                string raw = e.Message.Text.Substring(GetOrderStr().Length).Trim();
                if (string.IsNullOrEmpty(raw))
                {
                    // 没有有效指令
                    sendText.MsgToSend.Add(string.Format(AppConfig.ReplyParamInvalid, $"或无法找到目标，示例：{GetOrderStr()} [QQ|At|昵称|卡片]"));
                    return result;
                }
                if (!long.TryParse(raw, out target))
                {
                    // 获取群成员信息，尝试匹配昵称或卡片
                    if (!MainSave.GroupMemberInfos.TryGetValue(e.FromGroup, out var infos))
                    {
                        infos = e.FromGroup.GetGroupMemberList();
                        MainSave.GroupMemberInfos[e.FromGroup] = infos;
                    }
                    var info = infos.FirstOrDefault(x => x.Nick.Contains(raw) || x.Card.Contains(raw));
                    if (info != null)
                    {
                        target = info.QQ;
                    }
                }
            }

            // 检查目标QQ号是否有效
            if (target < QQ.MinValue)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyParamInvalid, $"，示例：{GetOrderStr()} [QQ|At|昵称|卡片]"));
                return result;
            }

            // 检查是否吞噬自己
            if (target == e.FromQQ)
            {
                sendText.MsgToSend.Add(AppConfig.ReplyDevourSelf);
                return result;
            }

            // 获取玩家和目标玩家的信息
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

            // 获取玩家和目标玩家的Kun信息
            var kun = Kun.GetKunByQQ(player.QQ);
            if (kun == null)
            {
                sendText.MsgToSend.Add(AppConfig.ReplyNoKun);
                return result;
            }
            kun.Initialize();

            // 检查Kun是否在自动模式下
            if (AutoPlay.CheckKunAutoPlay(kun))
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyAutoPlaying, kun));
                return result;
            }
            if (AutoPlay.CheckKunAutoPlay(kun, PublicInfos.Enums.AutoPlayType.Coin))
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyWorking, kun));
                return result;
            }

            // 获取目标玩家的Kun信息
            var targetKun = Kun.GetKunByQQ(targetPlayer.QQ);
            if (targetKun == null)
            {
                sendText.MsgToSend.Add(AppConfig.ReplyNoTargetPlayerKun);
                return result;
            }
            targetKun.Initialize();

            // 检查目标Kun是否在自动模式下
            if (AutoPlay.CheckKunAutoPlay(targetKun))
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyAutoPlaying, targetKun));
                return result;
            }

            // 检查冷却时间
            if (DateTime.Now - player.DevourAt < TimeSpan.FromMinutes(AppConfig.ValueDevourCD))
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyDevourInCD, (player.DevourAt.AddMinutes(AppConfig.ValueDevourCD)).ToString("G")));
                return result;
            }

            // 检查是否在同一个群
            bool sameGroup = CommonHelper.CheckSameGroup(target, e.FromGroup);
            long notSameGroupId = 0;
            if (!sameGroup)
            {
                var record = Record.GetRecordByKunID(targetKun.Id);
                if (record != null)
                {
                    notSameGroupId = record.Group;
                }
            }

            // 构建目标玩家信息
            string playerInfo = "";
            if (AppConfig.EnableAt)
            {
                playerInfo = $"[CQ:at,qq={target}]";
            }
            else if (sameGroup)
            {
                playerInfo = e.FromGroup.GetGroupMemberInfo(target)?.Card ?? target.ToString();
            }
            else
            {
                playerInfo = e.CQApi.GetGroupMemberInfo(notSameGroupId, target)?.Card ?? target.ToString();
            }

            // 执行吞噬操作
            var r = kun.Devour(targetKun);
            if (r.Success is false)
            {
                sendText.MsgToSend.Add("吞噬方法过程发生异常，查看日志获取更多信息");
                return result;
            }

            // 更新玩家的吞噬时间
            player.DevourAt = DateTime.Now;
            player.Update();

            // 处理吞噬结果
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

            // 如果不在同一个群且启用了跨群广播
            if (!sameGroup && AppConfig.EnableNotSameGroupDevourBoardcast)
            {
                if (r.Escaped)
                {
                    e.CQApi.SendGroupMessage(notSameGroupId, string.Format(AppConfig.ReplyDevouredNotSameGroupButEscaped, CQApi.CQCode_At(target)));
                }
                else if (r.Increment > 0)
                {
                    e.CQApi.SendGroupMessage(notSameGroupId, string.Format(AppConfig.ReplyDevouredNotSameGroup, CQApi.CQCode_At(target)));
                }
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