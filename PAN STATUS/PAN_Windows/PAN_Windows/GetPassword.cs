using System;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Configuration;

namespace PAN_Windows
{
    public partial class GetPassword : Form
    {

        public static string Password;
        public static string PassKey;
        public static string EncryptPwd;
        public static string SoapAction_Url;
        public static string PanNo;
        public static string UserName;
        public static string ServiceUrl;
        public static string BodyUlrs;

        public GetPassword()
        {
            InitializeComponent();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                     SecurityProtocolType.Tls11 |
                                     SecurityProtocolType.Tls12;
        }

        private void btnGetPassword_Click(object sender, EventArgs e)
        {
            Password = TxtPassword.Text;
            PassKey = TxtPasskey.Text;
            SoapAction_Url = ConfigurationManager.AppSettings["SoapAction_EncryptPassword"].ToString();
            ServiceUrl = ConfigurationManager.AppSettings["ServiceUrl"].ToString();
            BodyUlrs = ConfigurationManager.AppSettings["BodyUrl"].ToString();
            Execute(SoapAction_Url);

            PanInquiry_Model.EncryptPwd = EncryptPwd;
            PanInquiry_Model.UserName = TxtUserName.Text;
            PanInquiry_Model.Password = Password;
            PanInquiry_Model.PassKey = PassKey;
            PanInquiry_Model.PosCode = TxtPosCode .Text ;
           

            if (EncryptPwd == "ERROR")
            {
                MessageBox.Show(EncryptPwd.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                this.Hide();
            }

        }
        public static HttpWebRequest CreateWebRequest(string SoapAction_Url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(ServiceUrl);
            //webRequest.Headers.Add(@"SOAP:Action");
            webRequest.Headers.Add("SOAPAction", SoapAction_Url);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }
        public static void Execute(string SoapAction_Url)
        {
            HttpWebRequest request = CreateWebRequest(SoapAction_Url);
            XmlDocument soapEnvelopeXml = new XmlDocument();
            XmlDocument OutPut_doc = new XmlDocument();

            XNamespace ns = "http://schemas.xmlsoap.org/soap/envelope/";
            XNamespace myns = BodyUlrs;

            XDocument soapRequest = new XDocument(
                new XDeclaration("1.0", "UTF-8", "no"),
                new XElement(ns + "Envelope",
                    new XElement(ns + "Body",
                        new XElement(myns + "GetPassword",
                            new XElement(myns + "webApi",
                                new XElement(myns + "password", Password),
                                new XElement(myns + "passKey", PassKey))
                        )
                    )
                ));

            soapEnvelopeXml.LoadXml(soapRequest.ToString());

            using (Stream stream = request.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }

            using (WebResponse response = request.GetResponse())
            {
                OutPut_doc.Load(response.GetResponseStream());
                EncryptPwd = OutPut_doc.InnerText.ToString();               
            }

            
        }

        private void GetPassword_Load(object sender, System.EventArgs e)
        {
            KeyPreview = true;
        }

        private void GetPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{tab}");
            }
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            Application.Exit();
        }
    }
}
