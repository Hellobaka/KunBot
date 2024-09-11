using me.cqp.luohuaming.iKun.Sdk.Cqp.EventArgs;
using me.cqp.luohuaming.iKun.Sdk.Cqp.Interface;
using me.cqp.luohuaming.iKun.PublicInfos;
using System;
using System.IO;
using System.Reflection;

namespace me.cqp.luohuaming.iKun.Code
{
    public class Event_StartUp : ICQStartup
    {
        public void CQStartup(object sender, CQStartupEventArgs e)
        {
            MainSave.AppDirectory = e.CQApi.AppDirectory;
            MainSave.CQApi = e.CQApi;
            MainSave.CQLog = e.CQLog;
            MainSave.ImageDirectory = CommonHelper.GetAppImageDirectory();
            foreach (var item in Assembly.GetAssembly(typeof(Event_GroupMessage)).GetTypes())
            {
                if (item.IsInterface)
                    continue;
                foreach (var instance in item.GetInterfaces())
                {
                    if (instance == typeof(IOrderModel))
                    {
                        IOrderModel obj = (IOrderModel)Activator.CreateInstance(item);
                        if (obj.ImplementFlag == false)
                            break;
                        MainSave.Instances.Add(obj);
                    }
                }
            }

            e.CQLog.Info("��ʼ��", "��������");
            ConfigHelper.ConfigPath = Path.Combine(MainSave.AppDirectory, "Config.json");
            ConfigHelper.Load();
            AppConfig.LoadConfig();
            AppConfig.EnableAutoReload();

            if (File.Exists(Path.Combine(MainSave.AppDirectory, "data.db")) is false)
            {
                e.CQLog.Info("��ʼ��", "�������ݿ�");
                SQLHelper.CreateDB();
            }

            e.CQLog.Info("��ʼ��", "�������ʳ�");
            SQLHelper.CreateItems();

            e.CQLog.Info("��ʼ��", "��ʼ�����");
        }
    }
}
