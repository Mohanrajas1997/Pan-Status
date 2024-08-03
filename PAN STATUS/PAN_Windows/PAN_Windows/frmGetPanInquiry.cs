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
    public partial class frmGetPanInquiry : Form
    {
        public static string PanNo;
        public static string UserName;
        public static string PosCode;
        public static string EncryptPwd;
        public static string PassKey;
        public static string SoapAction_Url;
        public static string ErrorMsg;
        public static string ServiceUrl;
        public static string BodyUlrs;
        

        public frmGetPanInquiry()
        {
            InitializeComponent();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                    SecurityProtocolType.Tls11 |
                                    SecurityProtocolType.Tls12;            
        }

        private void btnPanStatus_Click(object sender, EventArgs e)
        {
            try
            {
                PanNo = TxtPanNo.Text;
                UserName = PanInquiry_Model.UserName;
                PosCode = PanInquiry_Model.PosCode;
                EncryptPwd = PanInquiry_Model.EncryptPwd;
                PassKey = PanInquiry_Model.PassKey;
                SoapAction_Url = ConfigurationManager.AppSettings["SoapAction_PanInquiry"].ToString();
                ServiceUrl = ConfigurationManager.AppSettings["ServiceUrl"].ToString();
                BodyUlrs = ConfigurationManager.AppSettings["BodyUrl"].ToString();
                GetPanStatus(SoapAction_Url);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        private void GetPanStatus(string SoapAction_Url)
        {
            try
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
                            new XElement(myns + "GetPanStatus",
                                new XElement(myns + "webApi",
                                    new XElement(myns + "pan", PanNo),
                                    new XElement(myns + "userName", UserName),
                                     new XElement(myns + "posCode", PosCode),
                                     new XElement(myns + "password", EncryptPwd),
                                     new XElement(myns + "passKey", PassKey)
                                  )
                             )
                        )
                    )
                 );

                soapEnvelopeXml.LoadXml(soapRequest.ToString());

                using (Stream stream = request.GetRequestStream())
                {
                    soapEnvelopeXml.Save(stream);
                }

                using (WebResponse response = request.GetResponse())
                {

                    OutPut_doc.Load(response.GetResponseStream());

                    foreach (XmlNode xmlNode in OutPut_doc.GetElementsByTagName("APP_PAN_INQ"))
                    {
                        foreach (XmlNode childNode in xmlNode.ChildNodes)
                        {
                            if (childNode.Name == "APP_PAN_NO")
                            {
                                //string App_Pan_No = childNode.InnerText;
                                TxtAppPanNo.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_NAME")
                            {
                                //string App_Name = childNode.InnerText;
                                TxtAppName.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_STATUS")
                            {
                                string APP_STATUS = childNode.InnerText;
                                string AppStatus_txt = "";

                                if (APP_STATUS == "000" || APP_STATUS == "100" || APP_STATUS == "200" || APP_STATUS == "300" || APP_STATUS == "400")
                                {
                                    AppStatus_txt = "Not Checked with respective KRA";
                                }
                                else if (APP_STATUS == "001" || APP_STATUS =="101" || APP_STATUS == "201" || APP_STATUS =="301" || APP_STATUS == "401")
                                {
                                    AppStatus_txt = "Submitted ";
                                }
                                else if (APP_STATUS == "002" || APP_STATUS == "102" || APP_STATUS =="202" || APP_STATUS =="302" || APP_STATUS =="402")
                                {
                                    AppStatus_txt = "KRA Verified";
                                }
                                else if (APP_STATUS == "003" || APP_STATUS == "103" || APP_STATUS =="203" || APP_STATUS == "303"|| APP_STATUS =="403")
                                {
                                    AppStatus_txt = "Hold";
                                }
                                else if (APP_STATUS == "004"|| APP_STATUS =="104" || APP_STATUS =="204" || APP_STATUS =="304" || APP_STATUS =="404")
                                {
                                    AppStatus_txt = "Rejected";
                                }
                                else if (APP_STATUS == "005" || APP_STATUS =="105" || APP_STATUS =="205" || APP_STATUS =="305" || APP_STATUS =="405")
                                {
                                    AppStatus_txt = "Not available";
                                }
                                else if (APP_STATUS == "006"|| APP_STATUS =="106" || APP_STATUS =="206" || APP_STATUS =="306"|| APP_STATUS =="406")
                                {
                                    AppStatus_txt = "Deactivated";
                                }
                                else if (APP_STATUS == "011"|| APP_STATUS =="111" || APP_STATUS =="211" || APP_STATUS =="311" || APP_STATUS =="411")
                                {
                                    AppStatus_txt = "Existing KYC Submitted";
                                }
                                else if (APP_STATUS == "012"|| APP_STATUS =="112"|| APP_STATUS =="212" || APP_STATUS =="312" || APP_STATUS =="412")
                                {
                                    AppStatus_txt = "Existing KYC Verified";
                                }
                                else if (APP_STATUS == "013" || APP_STATUS =="113" || APP_STATUS =="213" || APP_STATUS =="313" || APP_STATUS =="413")
                                {
                                    AppStatus_txt = "Existing KYC hold";
                                }
                                else if (APP_STATUS == "014"|| APP_STATUS == "114" || APP_STATUS =="214" || APP_STATUS =="314" || APP_STATUS =="414")
                                {
                                    AppStatus_txt = "Existing KYC Rejected";
                                }
                                else if (APP_STATUS == "022")
                                {
                                    AppStatus_txt = "KYC REGISTERED WITH CVLMF";
                                }
                                else if (APP_STATUS == "888")
                                {
                                    AppStatus_txt = "Not Checked with Multiple KRA";
                                }
                                else if (APP_STATUS == "999")
                                {
                                    AppStatus_txt = "Invalid PAN NO Format";
                                }

                                TxtAppStatus.Text = AppStatus_txt;
                            }
                            else if (childNode.Name == "APP_STATUSDT")
                            {
                                string APP_STATUSDT = childNode.InnerText;
                                DateTime dt;
                                if(APP_STATUSDT !="")
                                {
                                    dt = Convert.ToDateTime (childNode.InnerText);
                                    TxtStatusDt.Text = dt.ToString("dd/MM/yyyy");
                                }
                               
                            }
                            else if (childNode.Name == "APP_ENTRYDT")
                            {
                                string APP_ENTRYDT = childNode.InnerText;
                                DateTime dt_entry;
                                if(APP_ENTRYDT !="")
                                {
                                    dt_entry = Convert.ToDateTime(childNode .InnerText);
                                    TxtAppEntryDt.Text = dt_entry.ToString("dd/MM/yyyy");
                                }
                            }
                            else if (childNode.Name == "APP_MODDT")
                            {
                                string APP_MODDT = childNode.InnerText;
                                DateTime dt_Mod;
                                if(APP_MODDT !="")
                                {
                                    dt_Mod = Convert.ToDateTime(childNode .InnerText);
                                    TxtModDt.Text = dt_Mod.ToString("dd/MM/yyyy");
                                }
                            }
                            else if (childNode.Name == "APP_STATUS_DELTA")
                            {
                                string APP_STATUS_DELTA = childNode.InnerText;

                                if (APP_STATUS_DELTA=="00")
                                {
                                    TxtStatusDtl.Text = "All mandatory fields available";
                                }
                                else if(APP_STATUS_DELTA == "01")
                                {
                                    TxtStatusDtl.Text = "Name of the Applicant not available";
                                }
                                else if(APP_STATUS_DELTA == "02")
                                {
                                    TxtStatusDtl.Text = "Fathers / Spouse Name not available";
                                }
                                else if (APP_STATUS_DELTA == "03")
                                {
                                    TxtStatusDtl.Text = "Gender not available";
                                }
                                else if (APP_STATUS_DELTA == "04")
                                {
                                    TxtStatusDtl.Text = "Marital Status not available";
                                }
                                else if (APP_STATUS_DELTA == "05")
                                {
                                    TxtStatusDtl.Text = "DOB not available";
                                }
                                else if (APP_STATUS_DELTA == "06")
                                {
                                    TxtStatusDtl.Text = "Nationality not available";
                                }
                                else if (APP_STATUS_DELTA == "07")
                                {
                                    TxtStatusDtl.Text = "Residential Status not available";
                                }
                                else if (APP_STATUS_DELTA == "08")
                                {
                                    TxtStatusDtl.Text = "Proof of Identity not available";
                                }
                                else if (APP_STATUS_DELTA == "09")
                                {
                                    TxtStatusDtl.Text = "Correspondence Address not available";
                                }
                                else if (APP_STATUS_DELTA == "10")
                                {
                                    TxtStatusDtl.Text = "Proof of Correspondence Address not available";
                                }
                                else if (APP_STATUS_DELTA == "11")
                                {
                                    TxtStatusDtl.Text = "Permanent Address not available";
                                }
                                else if (APP_STATUS_DELTA == "12")
                                {
                                    TxtStatusDtl.Text = "Proof of Permanent Address not available";
                                }
                                else if (APP_STATUS_DELTA == "13")
                                {
                                    TxtStatusDtl.Text = "Applicants Signature not available";
                                }
                                else if (APP_STATUS_DELTA == "14")
                                {
                                    TxtStatusDtl.Text = "IPV details not available";
                                }
                                else if (APP_STATUS_DELTA == "15")
                                {
                                    TxtStatusDtl.Text = "Date of Incorporation not available";
                                }
                                else if (APP_STATUS_DELTA == "16")
                                {
                                    TxtStatusDtl.Text = "Place of Incorporation not available";
                                }
                                else if (APP_STATUS_DELTA == "17")
                                {
                                    TxtStatusDtl.Text = "Date of commencement of business not available";
                                }
                                else if (APP_STATUS_DELTA == "18")
                                {
                                    TxtStatusDtl.Text = "Status (Non Individual) not available";
                                }
                                else if (APP_STATUS_DELTA == "19")
                                {
                                    TxtStatusDtl.Text = "Number of Promoters/Partners/Karta/Trustees and whole time directors not available";
                                }
                                else if (APP_STATUS_DELTA == "20")
                                {
                                    TxtStatusDtl.Text = "Name of related person not available";
                                }
                                else if (APP_STATUS_DELTA == "21")
                                {
                                    TxtStatusDtl.Text = "Relation with Applicant not available";
                                }
                            }
                            else if (childNode.Name == "APP_UPDT_STATUS")
                            {
                                string APP_UPDT_STATUS = childNode.InnerText;
                                if (APP_UPDT_STATUS == "000" || APP_UPDT_STATUS == "100" || APP_UPDT_STATUS == "200" || APP_UPDT_STATUS == "300" || APP_UPDT_STATUS == "400")
                                {
                                    TxtUpdtStatus.Text  = "Not Checked with Respective KRA";
                                }
                                else if (APP_UPDT_STATUS == "001" || APP_UPDT_STATUS == "101" || APP_UPDT_STATUS == "201" || APP_UPDT_STATUS == "301" || APP_UPDT_STATUS=="401")
                                {
                                    TxtUpdtStatus.Text = "Modification Under Process";
                                }
                                else if (APP_UPDT_STATUS == "002" || APP_UPDT_STATUS == "102" || APP_UPDT_STATUS == "202" || APP_UPDT_STATUS == "302" || APP_UPDT_STATUS == "402")
                                {
                                    TxtUpdtStatus.Text = "Modification Registered ";
                                }
                                else if (APP_UPDT_STATUS == "003" || APP_UPDT_STATUS == "103" || APP_UPDT_STATUS == "203" || APP_UPDT_STATUS == "303" || APP_UPDT_STATUS == "403")
                                {
                                    TxtUpdtStatus.Text = "Modification Hold";
                                }
                                else if (APP_UPDT_STATUS == "004" || APP_UPDT_STATUS == "104" || APP_UPDT_STATUS == "204" || APP_UPDT_STATUS == "304" || APP_UPDT_STATUS == "404")
                                {
                                    TxtUpdtStatus.Text = "Modification Rejected ";
                                }
                                else if (APP_UPDT_STATUS == "005" || APP_UPDT_STATUS == "105" || APP_UPDT_STATUS == "205" || APP_UPDT_STATUS == "305" || APP_UPDT_STATUS == "405")
                                {
                                    TxtUpdtStatus .Text ="Not available (will not be displayed)";
                                }
                                else if (APP_UPDT_STATUS == "006" || APP_UPDT_STATUS == "106" || APP_UPDT_STATUS == "206" || APP_UPDT_STATUS == "306" || APP_UPDT_STATUS=="406")
                                {
                                    TxtUpdtStatus.Text = "Deactivated";
                                }
                                else if (APP_UPDT_STATUS == "888")
                                {
                                    TxtUpdtStatus.Text = "Not Checked with Multiple KRA ";
                                }
                            }
                            else if (childNode.Name == "APP_HOLD_DEACTIVE_RMKS")
                            {                               
                                TxtDectRmk.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_UPDT_RMKS")
                            {                                
                                TxtUpdtRmks.Text = childNode.InnerText;
                            }
                            else if (childNode.Name == "APP_KYC_MODE")
                            {
                                string APP_KYC_MODE = childNode.InnerText;
                                if (APP_KYC_MODE == "0")
                                {
                                    TxtKycMode.Text = "Normal KYC";
                                }
                                else if(APP_KYC_MODE == "1")
                                {
                                    TxtKycMode.Text = "e-KYC with OTP";
                                }
                                else if (APP_KYC_MODE == "2")
                                {
                                    TxtKycMode.Text = "e-KYC with Biometric";
                                }
                            }
                            else if (childNode.Name == "APP_IPV_FLAG")
                            {
                                string APP_IPV_FLAG = childNode.InnerText;
                                if (APP_IPV_FLAG == "Y")
                                {
                                    TxtIpvFlag.Text = "Yes";
                                }
                                else if (APP_IPV_FLAG == "N")
                                {
                                    TxtIpvFlag.Text = "No";
                                }
                                else if (APP_IPV_FLAG == "E")
                                {
                                    TxtIpvFlag.Text = "Exempted";
                                }
                            }
                            else if (childNode.Name == "APP_UBO_FLAG")
                            {
                                string APP_UBO_FLAG = childNode.InnerText;
                                if(APP_UBO_FLAG =="Y")
                                {
                                    TxtUboFlag.Text  = "Yes";
                                }
                                else if (APP_UBO_FLAG == "N")
                                {
                                    TxtUboFlag.Text = "No";
                                }
                            }

                        }
                    }

                    foreach (XmlNode xmlNode_Error in OutPut_doc.GetElementsByTagName("ERROR"))
                    {
                        foreach (XmlNode childNode in xmlNode_Error.ChildNodes)
                        {
                            if (childNode.Name == "ERROR_CODE")
                            {
                                string ErrorCode = childNode.InnerText;
                            }
                            else if (childNode.Name == "ERROR_MSG")
                            {
                                ErrorMsg = childNode.InnerText;
                            }
                        }
                    }
                    if (ErrorMsg != null)
                    {
                        MessageBox.Show(ErrorMsg.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmGetPanInquiry_Load(object sender, System.EventArgs e)
        {
            KeyPreview = true;
        }

        private void frmGetPanInquiry_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send("{tab}");
            }
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
    }
}
