using me.cqp.luohuaming.iKun.PublicInfos;
using me.cqp.luohuaming.iKun.PublicInfos.Models;
using me.cqp.luohuaming.iKun.Sdk.Cqp;
using me.cqp.luohuaming.iKun.Sdk.Cqp.EventArgs;
using me.cqp.luohuaming.iKun.Sdk.Cqp.Model;
using System;
using System.Linq;

namespace me.cqp.luohuaming.iKun.Code.OrderFunctions
{
    public class Attack : IOrderModel
    {
        public bool ImplementFlag { get; set; } = true;

        public string GetOrderStr() => AppConfig.CommandAttack;

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
                    sendText.MsgToSend.Add(string.Format(AppConfig.ReplyParamInvalid, $"，示例：{GetOrderStr()} [QQ|At|昵称|卡片]"));
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
                    var info = infos.FirstOrDefault(x => x.Nick.Contains(raw) || x.Card.Contains(raw));
                    if (info != null) 
                    {
                        target = info.QQ;
                    }
                }
            }

            if (target < QQ.MinValue)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyParamInvalid, $"或无法找到目标，示例：{GetOrderStr()} [QQ|At|昵称|卡片]"));
                return result;
            }
            if (target == e.FromQQ)
            {
                sendText.MsgToSend.Add(AppConfig.ReplyAttackSelf);
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
            kun.Initialize();
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
            var targetKun = Kun.GetKunByQQ(targetPlayer.QQ);
            if (targetKun == null)
            {
                sendText.MsgToSend.Add(AppConfig.ReplyNoTargetPlayerKun);
                return result;
            }
            targetKun.Initialize();
            if (AutoPlay.CheckKunAutoPlay(targetKun))
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyAutoPlaying, targetKun));
                return result;
            }

            if (DateTime.Now - player.AttackAt < TimeSpan.FromMinutes(AppConfig.ValueAttackCD))
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyAttackInCD, (player.AttackAt.AddMinutes(AppConfig.ValueAttackCD)).ToString("G")));
                return result;
            }

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

            var r = kun.Attack(targetKun);
            if (r.Success is false)
            {
                sendText.MsgToSend.Add("攻击方法过程发生异常，查看日志获取更多信息");
                return result;
            }
            player.AttackAt = DateTime.Now;
            player.Update();
            if (r.Dead)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyAttackFailAndDead, kun.ToString(), playerInfo, targetKun.ToString(), r.TargetDecrement.ToShortNumber(), r.TargetCurrentWeight.ToShortNumber()));
            }
            else if (r.TargetDead)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyAttackSuccessAndTargetDead, kun.ToString(), playerInfo, targetKun.ToString(), r.Increment.ToShortNumber(), r.CurrentWeight.ToShortNumber()));
            }
            else if (r.Increment < 0)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyAttackFail, kun.ToString(), playerInfo, targetKun.ToString(), r.Increment.ToShortNumber(), r.CurrentWeight.ToShortNumber(), r.TargetDecrement.ToShortNumber(), r.TargetCurrentWeight.ToShortNumber()));
            }
            else if (r.Escaped)
            {
                sendText.MsgToSend.Add(string.Format(AppConfig.ReplyAttackEscaped, kun.ToString(), playerInfo, targetKun.ToString()));
            }
            else
            {
                string send = string.Format(AppConfig.ReplyAttackSuccess, kun.ToString(), playerInfo, targetKun.ToString(), r.Increment.ToShortNumber(), r.CurrentWeight.ToShortNumber(), r.TargetDecrement.ToShortNumber(), r.TargetCurrentWeight.ToShortNumber());
                if (r.WeightLimit)
                {
                    send += $"\n{AppConfig.ReplyWeightLimit}";
                }
                sendText.MsgToSend.Add(send);
            }

            if (!sameGroup && AppConfig.EnableNotSameGroupAttackBoardcast)
            {
                if (r.Escaped && AppConfig.EnableNotSameGroupAttackEscapeBoardcast)
                {
                    e.CQApi.SendGroupMessage(notSameGroupId, string.Format(AppConfig.ReplyAttackedNotSameGroupButEscaped, CQApi.CQCode_At(target)));
                }
                else if (r.TargetDead)
                {
                    e.CQApi.SendGroupMessage(notSameGroupId, string.Format(AppConfig.ReplyAttackedNotSameGroupAndDead, CQApi.CQCode_At(target)));
                }
                else if (r.Increment > 0)
                {
                    double loss = r.TargetDecrement / (r.TargetCurrentWeight + r.TargetDecrement) * 100;
                    if (loss > AppConfig.ValueNotSameGroupNoticeMinimalPercent)
                    {
                        e.CQApi.SendGroupMessage(notSameGroupId, string.Format(AppConfig.ReplyAttackedNotSameGroup, CQApi.CQCode_At(target), r.TargetDecrement.ToShortNumber(), r.TargetCurrentWeight.ToShortNumber()));
                    }
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