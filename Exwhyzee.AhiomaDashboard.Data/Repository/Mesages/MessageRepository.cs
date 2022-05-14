using Exwhyzee.AhiomaDashboard.Data.Repository.Orders;
using Exwhyzee.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Exwhyzee.AhiomaDashboard.Data.Repository.Mesages
{
    public class MessageRepository : IMessageRepository
    {
        public async Task<bool> AddMessage(AddMessageDto sms)
        {
            try
            {
                string xapiurl = $"http://notify.ahioma.com/api/Messages";
                string xapiUrl = String.Format(xapiurl);
                WebRequest xrequestObj = WebRequest.Create(xapiUrl);
                xrequestObj.Method = "POST";
                xrequestObj.ContentType = "application/json";
                using (var streamWriter = new StreamWriter(xrequestObj.GetRequestStream()))
                {
                    string jsonModel = Newtonsoft.Json.JsonConvert.SerializeObject(sms);
                    streamWriter.Write(jsonModel);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
                HttpWebResponse xresponse = (HttpWebResponse)xrequestObj.GetResponse();

                Stream responseStream = xresponse.GetResponseStream();
                return true;
            } catch (Exception f)
            {
                return false;
            }
        }

        public async Task<string> GetMessage(ContentType contentType)
        {
            int idx = (int)contentType;
            string smsMessage = "";
            try
            {
                string apiurl = $"http://notify.ahioma.com/api/MessageContents/GetMessageToSend?enumId=" +idx;
                string apiUrl = String.Format(apiurl);
                WebRequest requestObj = WebRequest.Create(apiUrl);
                requestObj.Method = "GET";
                HttpWebResponse responseGet = null;
                responseGet = (HttpWebResponse)requestObj.GetResponse();

                string result = null;
                using (Stream stream = responseGet.GetResponseStream())
                {
                    StreamReader sr = new StreamReader(stream);

                    result = sr.ReadToEnd();
                    MessageDto rmesage = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<MessageDto>(result));
                    smsMessage = rmesage.Content;
                    sr.Close();
                }
                return smsMessage;
            }catch(Exception c)
            {
                return smsMessage;
            }
        }
    }
}
