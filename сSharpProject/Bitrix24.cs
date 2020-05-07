using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace сSharpProject
{
    class In
    {
        public void Intro()
        {
            Bitrix24 bx_logon = new Bitrix24();
            //https://dev.1c-bitrix.ru/rest_help/

            //bx_logon.SendCommand("task.item.list", "ORDER[]=&FILTER[GROUP_ID]=44&PARAMS[]=&SELECT[]=");
            //string TaskListByJSON = bx_logon.SendCommand("task.commentitem.get", "TASKID=57337&ITEMID=379512");
            //string jsonResponse = bx_logon.SendCommand("disk.storage.uploadfile", "id=2233&data[NAME]=test.jpg", "fileContent[0]=test.jpg&fileContent[1]=" + file);

            string filename = @"C:\Users\andreydruzhinin\Desktop\1.xlsx";
            string contents = Convert.ToBase64String(File.ReadAllBytes(filename), Base64FormattingOptions.None);

            string jsonResponse = bx_logon.SendCommand("log.blogpost.add", "",
            "USER_ID=1827" +
            "&POST_TITLE=заголовок" +
            "&POST_MESSAGE=текст сообщения" +
            "&DEST[0]=SG402" +
            "&FILES[0][0]=test.xlsx&FILES[0][1]=" + HttpUtility.UrlEncode(contents)  //+
                                                                                     //"&FILES[1][0]=2.xlsx&FILES[1][1]=" + file +
                                                                                     //"&FILES[2][0]=3.xlsx&FILES[2][1]=" + file     
            );
        }
    }
    class Bitrix24
    {
        private const string BX_ClientID = "local.5e8255b199d0e7.70800677"; //боевой
        private const string BX_ClientSecret = "730hvSv5yrgmVv0gcwkxUz2ATaGGIx0fZmuOXMQjSkehKaFzMz"; //боевой
        private const string BX_Portal = "https://bitrix.eltransplus.ru"; //боевой

        //private const string BX_ClientID = "local.5e834c45530933.17923150"; 
        //private const string BX_ClientSecret = "lBA05N9gi8dw3sVX4A7V7R7oSqY5EU0L779BZNi0TGutrIzVFh";
        //private const string BX_Portal = "https://bitest.eltransplus.ru"; 


        private const string BX_OAuthSite = "https://oauth.bitrix.info";
        private string AccessToken;
        private string RefreshToken;
        private DateTime RefreshTime;
        private string Code;
        private string Cookie;

        public Bitrix24()
        {
            Connect();
        }

        private void Connect()
        {
            //Создание HTTP подключения
            string BX_URI = BX_Portal + "/oauth/authorize/?client_id=" + BX_ClientID;
            HttpWebRequest requestLogonBitrix24 = (HttpWebRequest)WebRequest.Create(BX_URI);

            //Логин и пароль администратора, под которым будут выполняться запросы
            string username = "andreydruzhinin@eltransplus.ru";
            string password = "J4e3Yv";


            //Настройка запроса
            string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(username + ":" + password));
            requestLogonBitrix24.Headers.Add("Authorization", "Basic " + svcCredentials);
            requestLogonBitrix24.AllowAutoRedirect = false; //обязательное условие, чтобы не было автоматической переадресации на другую страницу (теряются пользовательские сведения об авторизации)
            requestLogonBitrix24.Method = "POST";            
 
            HttpWebResponse responseLogonBitrix24 = (HttpWebResponse)requestLogonBitrix24.GetResponse();

            //Проверяем что статус-код 302, должны предложить переадресацию, иначе авторизация не требуется, мы и так авторизированы
            if (responseLogonBitrix24.StatusCode == HttpStatusCode.Found)
            {
                //Получаем из заголовков ответа Куки и параметры адреса переадресации (из поля "Location"), параметр Code
                Uri locationURI = new Uri(responseLogonBitrix24.Headers["Location"]);

                //Получаем параметры из строки ответа (нужен System.Web) 
                Regex regex = new Regex("code=.*?&", RegexOptions.IgnoreCase);
                Match match = regex.Match(locationURI.Query);

                if (match.Success)
                {
                    Code = match.Value.Remove(match.Value.Length - 1).Remove(0, 5);
                }

                Cookie = responseLogonBitrix24.Headers["Set-Cookie"];

                //Вызываем исключение, если Код мы не смогли получить, без него далее никак.
                if (String.IsNullOrEmpty(Code))
                {
                    throw new FormatException("CodeNotFound");
                }

                //Закрываем подключение
                responseLogonBitrix24.Close();

                //Если код успешно получили, то формируем новый HTTP запрос для получения Токенов авторизации
                string BX_OAuth_URI = BX_OAuthSite + "/oauth/token" + "/?" + "grant_type=authorization_code" + "&" +
                "client_id=" + BX_ClientID + "&" +
                "client_secret=" + BX_ClientSecret + "&" +
                "code=" + Code;
                SetToken(BX_OAuth_URI);
            }

        }

        //Закрытый метод для получения и записи Токенов авторизации
        private void SetToken(string BX_OAuth_URI)
        {
            //Формируем новый HTTP запрос для получения Токенов авторизации
            HttpWebRequest requestLogonBitrixOAuth = (HttpWebRequest)WebRequest.Create(BX_OAuth_URI);
            requestLogonBitrixOAuth.Method = "POST";
            requestLogonBitrixOAuth.Headers["Cookie"] = Cookie; //Используем Куки полученный в предыдущем запросе авторизации

            //Подключаемся (отправляем запрос)
            HttpWebResponse responseLogonBitrixOAuth = (HttpWebResponse)requestLogonBitrixOAuth.GetResponse();

            //Если в ответ получаем статус-код отличный от 200, то это ошибка, вызываем исключение
            if (responseLogonBitrixOAuth.StatusCode != HttpStatusCode.OK)
            {
                throw new FormatException("ErrorLogonBitrixOAuth");
            }
            else
            {
                DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(objLogonBitrixOAuth));
                objLogonBitrixOAuth objLogonBitrixOAuth;

                using (Stream st = responseLogonBitrixOAuth.GetResponseStream())
                {
                    objLogonBitrixOAuth = (objLogonBitrixOAuth)deserializer.ReadObject(st);
                }

                //Записывем Токены авторизации в поля
                AccessToken = objLogonBitrixOAuth.access_token;
                RefreshToken = objLogonBitrixOAuth.refresh_token;
                RefreshTime = DateTime.Now.AddSeconds(Convert.ToDouble(objLogonBitrixOAuth.expires_in)); //Добавляем к текущей дате количество секунд действия токена, обычно это плюс один час
            }
        }

        //Закрытый метод для обновления Токенов авторизации, если истекло время их действия
        private void RefreshTokens()
        {
            if (RefreshTime == DateTime.MinValue) // Если RefreshTime пустая
            {
                //Тогда вызываем авторизацию по полной программе
                Connect();
                return;
            }

            //Проверяем, если истекло время действия Токена авторизации, то обновляем его
            if (RefreshTime.AddSeconds(-5) < DateTime.Now)
            {
                //Формируем новый HTTP запрос для обновления Токена авторизации, здесь Code уже не нужен
                string BX_OAuth_URI = BX_OAuthSite + "/oauth/token" + "/?" + "grant_type=refresh_token" + "&" +
                "client_id=" + BX_ClientID + "&" +
                "client_secret=" + BX_ClientSecret + "&" +
                "refresh_token=" + RefreshToken;
                SetToken(BX_OAuth_URI);
            }
        }

        //Открытый метод для отправки REST-запросов в Битрикс24
        public string SendCommand(string Command, string GetParams = "", string PostParams = "")
        {
            //Проверяем и обновлем Токены авторизации
            RefreshTokens();

            //Проверяем возможное указание параметров
            string BX_REST_URI = BX_Portal + "/rest/" + Command + "?auth=" + AccessToken;
            
            if (String.IsNullOrEmpty(GetParams) == false)
            {
                BX_REST_URI = BX_REST_URI + "&" + GetParams;
            }        

            //Создаем новое HTTP подключение для отправки REST-запроса в Битрикс24
            HttpWebRequest requestBitrixREST = (HttpWebRequest)WebRequest.Create(BX_REST_URI);
            requestBitrixREST.Method = "POST";
            requestBitrixREST.Accept = "application/json";
            requestBitrixREST.Headers["Cookie"] = Cookie; //Используем Куки полученный в запросе авторизации

            //Готовим тело запроса и вставляем его в тело POST-запроса  
            byte[] byteArrayBody = Encoding.UTF8.GetBytes(PostParams);         
            requestBitrixREST.ContentType = "application/x-www-form-urlencoded";
            requestBitrixREST.ContentLength = byteArrayBody.Length;

            Stream dataBodyStream = requestBitrixREST.GetRequestStream();
            dataBodyStream.Write(byteArrayBody, 0, byteArrayBody.Length);
            dataBodyStream.Close();

            //Отправляем данные в Битрикс24
            HttpWebResponse responseBitrixREST = (HttpWebResponse)requestBitrixREST.GetResponse();

            //Читаем тело ответа от Битрикс24
            Stream dataStreamBitrixREST = responseBitrixREST.GetResponseStream();
            var readerBitrixREST = new StreamReader(dataStreamBitrixREST);
            string stringBitrixREST = readerBitrixREST.ReadToEnd();

            //Закрываем все подключения и потоки
            readerBitrixREST.Close();
            dataStreamBitrixREST.Close();
            responseBitrixREST.Close();

            //Возвращаем строку ответа в формате JSON
            return stringBitrixREST;
        }

        [DataContract]
        public class objLogonBitrixOAuth
        {
            [DataMember]
            public string expires_in { get; set; }
            [DataMember]
            public string refresh_token { get; set; }
            [DataMember]
            public string access_token { get; set; }
        }
    }
}