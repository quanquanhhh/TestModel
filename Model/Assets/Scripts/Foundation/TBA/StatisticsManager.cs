using System;
using System.Collections.Generic;
using Foundation.CommonModel;
using UnityEngine;

namespace Foundation.TBA
{
    public class StatisticsManager : Singleton<StatisticsManager>
    {
        public int isHit = -1; //- 0 命中  1 未命中
        public bool cloakBack = false;
        
        
        private static string UserIDFA = "";//Device.advertisingIdentifier;
        private static string UserIDFV = "";// Device.vendorIdentifier;
        private const string MessageReleaseUrl = "https://behave.sheriffbunny.com/plaque/mesmeric";

        private string CloakUrl = "https://syzygy.sheriffbunny.com/freshman/sombre?";
        private const string gamename = "com.SheriffBunny.WildWest.Ios";

        private Dictionary<string, string> topDic = new Dictionary<string, string>()
        {
            { "griswold",SystemInfo.operatingSystem.EncodingText()},
        };
        public void SendInstallEvent(string referrer, string referrerVersion)
        {
            Dictionary<string, object> synonymy  = new Dictionary<string, object>();
            synonymy ["freya"] = "build /2022.3.61f1".EncodingText();
            synonymy["willa"] = referrer;
            synonymy["affirm"] = referrerVersion;
            synonymy["sob"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36";
            synonymy["platform"] = "flung".EncodingText();
            synonymy["pinkish"] = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            synonymy["dusty"] = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            synonymy["shred"] = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            synonymy["rye"] = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            synonymy["finn"] = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            synonymy["manioc"] = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            
            
            Dictionary<string, object> baseInfo = new Dictionary<string, object>();
            baseInfo.Add("synonymy", synonymy);
            SendPostEvent(baseInfo, "Install");
        }

        private void SendPostEvent(Dictionary<string, object> connect, string posId)
        {
            string logId = Guid.NewGuid().EncodingText();

            Dictionary<string, object> airedale = new Dictionary<string, object>();
            airedale["tacit"]   = gamename.EncodingText();
            airedale["morocco"] = logId.EncodingText();
            airedale["gallus"]  =  new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            airedale["numeral"] = "Apple".EncodingText();
            airedale["feeble"]  =  SystemInfo.operatingSystem.EncodingText();
            airedale["ogle"]    =   "en_US".EncodingText(); 
            
            Dictionary<string, object> grunt = new Dictionary<string, object>();
            grunt["portico"]  = "hewitt".EncodingText();
            grunt["consume"]  = Application.version.EncodingText(); 
            grunt["sydney"]   = SystemInfo.deviceUniqueIdentifier.EncodingText();
            grunt["oblivion"] = "Apple".EncodingText();
            grunt["legend"]   = "mcc".EncodingText();
            
            
            Dictionary<string, object> flee = new Dictionary<string, object>();
 

            Dictionary<string, object> sendDic = new Dictionary<string, object>();
            sendDic.Add("airedale", airedale);
            sendDic.Add("grunt", grunt);
            sendDic.Add("flee", flee);

            foreach (var item in connect)
            {
                sendDic.Add(item.Key, item.Value);
            }

            MessageManager.Instance.Post<GetData>(
                  MessageReleaseUrl, logId, posId, topDic, sendDic, 60);
        }



        public void SendSessionEvent()
        {
            Dictionary<string, object> baseInfo = new Dictionary<string, object>();
            
            
            Dictionary<string, object> baseInfo2 = new Dictionary<string, object>();
            baseInfo2.Add("brazil", baseInfo); 
            SendPostEvent(baseInfo2, "Session");
        }

        private string RevenueStr = "";
        private double Revenue = 0;

        public void SendAdGetReward(MaxSdkBase.AdInfo adInfo)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();

            dic.Add("alginate", (long)(adInfo.Revenue * 1e6));
            dic.Add("charity", "USD".EncodingText());
            dic.Add("thereon", adInfo.NetworkName.EncodingText());
            dic.Add("rajah", "max".EncodingText());
            dic.Add("skimpy", adInfo.AdUnitIdentifier.EncodingText());
            dic.Add("dung", adInfo.Placement.EncodingText());
            dic.Add("casey", adInfo.AdFormat.EncodingText());

            Dictionary<string, object> baseInfo = new Dictionary<string, object>();
            baseInfo.Add("slid", dic);
#if DEBUG || DEBUG_APP
            Revenue += (adInfo.Revenue * 1000);
            RevenueStr += "," + (adInfo.Revenue * 1000);
            GUIUtility.systemCopyBuffer = RevenueStr + " \n total : " + Revenue;
            Event.Instance.SendEvent(new ShowSystemTips(" ecpm : " + (adInfo.Revenue * 1000)));
#endif
            
            SendPostEvent(baseInfo, adInfo.Placement);
        }
        
        public void SendLogEvent(string eventName, Dictionary<string, object> arrValue = null)
        {
            Dictionary<string, object> baseInfo = new Dictionary<string, object>();
            Dictionary<string, object> dic = new Dictionary<string, object>();
            baseInfo["debussy"] = eventName;
            if (arrValue != null)
            {
                foreach (var item in arrValue)
                {
                    dic.Add(item.Key, item.Value);
                }
                baseInfo.Add(eventName,dic);
            }
            SendPostEvent(baseInfo, eventName);
        }
        //Android 项目需要移除此项
         public void SendCloakEventData()
         {
             List<string> info = new List<string>();
             info.Clear();


             info.Add($"{"tacit"}={gamename.EncodingText()}");
             info.Add($"{"portico"}={"hewitt".EncodingText()}");
             info.Add($"{"consume"}={Application.version.EncodingText()}");
             info.Add($"{"gallus"}={new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds()}");
             
             info.Add($"{"numeral"}={SystemInfo.deviceModel.EncodingText()}");
             info.Add($"{"feeble"}={SystemInfo.operatingSystem.EncodingText()}");
             
             info.Add($"{"semite"}={UserIDFV.EncodingText()}");
             info.Add($"{"geoduck"}={UserIDFA.EncodingText()}");


             string url = CloakUrl + string.Join("&", info);
             Debug.Log("TODO: " + url);
             MessageManager.Instance.Get<GetData>(url, "cloak", topDic, 12, data =>
             {
                 if (data.ErrorCode == 200)
                 {
                     if (data.RawData.Contains("host"))
                     {
                         SendLogEvent("target_hit");
                         isHit = 0; 
                     }
                     else
                     {
                         isHit = 1; 
                     }

                     cloakBack = true;
                 }
             });
        }
    }
}