using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BottleShop
{
    public class Security
    {
        private string publickey = @" <RSAKeyValue><Modulus>1fdHL+VlzaA001R1G9+sbw7WnO9SCcfwKhiG6tN6a4p36LS6jLJnfjyEgzYEG2mdEDlzuVDUek04CxRpNx2LtOcyE1cURG02WhNkf+xGIfpOyqm4k67oDM1ncY39SfCYf0wPuqddmuguUyVvzfkwz8nKTvTkbvsijuDbwptn4SkrN+f/CmJqxfHuKkmTagXVOKVq+cboXi4K0EjPbepOctni78I0S5QheU5itxMBYSq5zQP67XWm3pV57ZsxtUrb7GwvLVV1JbDcmcckW2f86JX7JFf/MlDgwgvzrcXvZ4Z5uIknrM4HlrN6bE95B5Mm4psUevGYBeFIr66VMcB4pQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        private string privatekey = @"<RSAKeyValue><Modulus>1fdHL+VlzaA001R1G9+sbw7WnO9SCcfwKhiG6tN6a4p36LS6jLJnfjyEgzYEG2mdEDlzuVDUek04CxRpNx2LtOcyE1cURG02WhNkf+xGIfpOyqm4k67oDM1ncY39SfCYf0wPuqddmuguUyVvzfkwz8nKTvTkbvsijuDbwptn4SkrN+f/CmJqxfHuKkmTagXVOKVq+cboXi4K0EjPbepOctni78I0S5QheU5itxMBYSq5zQP67XWm3pV57ZsxtUrb7GwvLVV1JbDcmcckW2f86JX7JFf/MlDgwgvzrcXvZ4Z5uIknrM4HlrN6bE95B5Mm4psUevGYBeFIr66VMcB4pQ==</Modulus><Exponent>AQAB</Exponent><P>9uZxAKKoqcXkpFamYDSmaFEggLvu3Ghw3rCsYpVzUjbIvYSvM1gsuG57jgOOl7xaoZ6XZR7froUKgv93doYw8G0Iq/AzcoW2a0/YHH7lCAo5dNZ79L3JfirX5OKz1+zNPm2iVefV3gqzkE5W3SvcHGrAeKIpC5uYa/mCUVOCR0k=</P><Q>3doYQuSOUGqT1Dm7Q1R8tk30FAN7e4xM3f0KlJ0aOZi9hWd4WtB+t5J+c5OKMrQEtohIMP93IYn8df5zog9kBOhUd2ML3VergxUxNi467mnf/3P6eqLSll4Ke/0u10lgNWAEWl1fZqMLDJJJD0XUZp9GuNVgt3WbsoJBa8l0Wn0=</Q><DP>wJBWTjKO7Wqkuu/B74LgzreHbCAnWcwzS9vrdzAss1B7HCTiKF968ZVp1Ac+LWYAIdF5Lqr3tjuZSTZKTCy4+qtvefprhcR31BZ/7SmjI7Qlv9SQ9P8Yqchsfd26eov8P9ZuKCYbDYS3K7ON2A4fLCKuXufKr3z5Ui1V0GwbTdE=</DP><DQ>xRG/C2zLnlOODa9a0Wxze92zOzP8tLrkbS0iAPGtIgy0DZTkLIeYiKjw+unMkU2oAhz9Q8kqofhY8tedEBBr4JAguAybtXrzN/XTSTW306DpAZqcneOU/U18MresrEDZ3Y/1TfAzlpCud/RaQd7d1msV/4pL56vpKqDaTE0ftmE=</DQ><InverseQ>GVtOfL0kHKkv10Z+2De+N6TWpegYBl+fSGysRUPvN68y0Cg9Hp/6yN7o9rfmO/AVamn6QVN5BTN0aIUFxOwf7AyuFFXHUvfWRN8n1yXZDuXanQUtnwXKwrEJjXiqM6u395/nRRk1z6jyuN3r+Tophk181Vxu7KMZqfSpiJPchB0=</InverseQ><D>sxxdxhVlBB3BOsrvgZba1Nj6BMM9zJxHSiXgAwFm8FEokW+A3jiV9BGa5++2vm/JoARIAHBOfdbYPEYo0Y91HMmQnKn/eExn/D1Rmthh2R2dCX/L2CzJabYkiuVOi6/RKHa2kAx8iikkZkvnbOfV2YtVRIUyGEk4I5zNufm3mr4AXsq5c1NQ2n+jV2PV5U5Dhwe8nXxRE0KE1eF6vi6nbGubfojd3EkoDs+rwkYF6cqHArpifROWxIbJHlwum+H8lqVsyXJSYVZZh2MC/L6kUKzRgeCVMPoFHzDTCHDckrKwBnDvfZReTP512vHoYKwpdipOFdK8bNnsgytrGPPhYQ==</D></RSAKeyValue>";

        public string Encription(string txt)
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(publickey);

                UnicodeEncoding ByteConverter = new UnicodeEncoding();
                byte[] dataToEncrypt = ByteConverter.GetBytes(txt);
                byte[] encryptedData = rsa.Encrypt(dataToEncrypt, false);
                return Convert.ToBase64String(encryptedData);
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public string Description(string txt)
        {
            try
            {
                //CspParameters cspParams = new CspParameters();
                //cspParams.KeyContainerName = "MyKeyContainer";
                //cspParams.ProviderType = 1;
                //cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
                //RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(cspParams);

                RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider();
                rsaProvider.FromXmlString(privatekey);
                byte[] buffer = Convert.FromBase64String(txt);
                byte[] plainBytes = rsaProvider.Decrypt(buffer, false);
                //string str1 = ASCIIEncoding.ASCII.GetString(plainBytes);
                string plainText = Encoding.UTF8.GetString(plainBytes, 0, plainBytes.Length);
                return plainText.Replace("\0", "");
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}
