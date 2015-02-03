using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JvlMifare
{
    public partial class ConvertArrays
    {
         //Mensajes de Rspuesta

        public const int SUCCESS = 0x9000;
        public const int DATOS_INCOMPLETOS = 0x4001;
        public const int FORMATO_INCORRECTO = 0x4002;

        #region ConvertStringHexToByteArray
        /// <summary>
        /// Convierte una cadena de texto representando un número hexadecimal a un arreglo de bytes.
        /// </summary>
        /// <param name="str"> Cadena de texto que representa el número en hexadecimal</param>
        /// <param name="byteArray">Arreglo de bytes</param>
        /// <param name="byteArrayLength">Longitud del arreglo de bytes</param>
        /// <returns>Regresa iClassStatusCodes. SUCCESS en caso de Exito.</returns>
        static public int ConvertStringHexToByteArray(string str, ref byte[] byteArray, int byteArrayLength)
        {
            int resp = SUCCESS;
            int i, k = 0;

            String strBuffData = str;

            //Borra los espacios entre bytes
            for (i = 0; i < strBuffData.Length; i++)
            {
                if (strBuffData.Substring(i, 1).CompareTo(" ") == 0)
                    strBuffData = strBuffData.Remove(i, 1);

            }

            if (strBuffData.Length == 2 * byteArrayLength)
            {
                //Copia la cadena a un arreglo de enteros
                try
                {
                    k = 0;
                    for (i = 0; i < byteArrayLength; i++)
                    {
                        byteArray[i] = Convert.ToByte(strBuffData.Substring(k, 2), 16);
                        k = k + 2;
                    }

                    resp = SUCCESS;

                }
                catch (IndexOutOfRangeException siobe)
                {
                    System.Console.WriteLine("Exception..." + siobe.Message);
                    //MessageBox.Show("Datos Incompletos. Deben ser " + Convert.ToString(byteArrayLength * 2) + " caracteres en formato hexadecimal", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    resp = DATOS_INCOMPLETOS;
                }
                catch (ArgumentOutOfRangeException are)
                {
                    System.Console.WriteLine("Exception..." + are.Message);
                    //MessageBox.Show("Datos Incompletos. Deben ser " + Convert.ToString(byteArrayLength * 2) + " caracteres en formato hexadecimal", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    resp = DATOS_INCOMPLETOS;
                }
                catch (FormatException fe)
                {
                    System.Console.WriteLine("Exception..." + fe.Message);
                    //MessageBox.Show("Formato  Incorrecto. Deben ser " + Convert.ToString(byteArrayLength * 2) + " caracteres en formato hexadecimal", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    resp = FORMATO_INCORRECTO;
                }

            }
            else
            {
                //MessageBox.Show("Datos Incompletos. Deben ser " + Convert.ToString(byteArrayLength * 2) + " caracteres en formato hexadecimal", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                resp = DATOS_INCOMPLETOS;
            }

            return resp;
        }

        #endregion

        #region ConvertByteArrayToDateTime

        public static DateTime ConvertByteArrayToDateTime(byte[] byteArray)
        {
            DateTime dt = new DateTime();
            try
            {

                int day = (int)byteArray[0];
                int month = (int)byteArray[1];
                int year = (((int)byteArray[2]) << 8) | (byteArray[3] & 0xFF);
                int hour = (int)byteArray[4];
                int min = (int)byteArray[5];
                int sec = (int)byteArray[6];

                dt = new DateTime(year, month, day, hour, min, sec);
            }
            catch (ArgumentOutOfRangeException aue)
            {
                Console.WriteLine("ConvertByteArrayToDateTime.." + aue.Message);
            }
            return dt;
        }
        #endregion

        #region  ConvertDateTimeToByteArray
        public static  byte[] ConvertDateTimeToByteArray(DateTime dateTime)
        {
            byte[] byteDateTime = new byte[8];

            byteDateTime[0] = (byte)dateTime.Day;
            byteDateTime[1] = (byte)dateTime.Month;
            byteDateTime[2] = (byte)((dateTime.Year & 0xFF00) >> 8);
            byteDateTime[3] = (byte)(dateTime.Year & 0x00FF);
            byteDateTime[4] = (byte)dateTime.Hour;
            byteDateTime[5] = (byte)dateTime.Minute;
            byteDateTime[6] = (byte)dateTime.Second;


            return byteDateTime;
        }

        #endregion

        #region ByteArrayToStr
        /// <summary>
        /// Convierte un arreglo de bytes a string. Codificación UTF8
        /// </summary>
        /// <param name="bytearray">Arreglo de Bytes a convertir</param>
        /// <returns>Cadena de Texto ASCII UTF8</returns>
        public static string ByteArrayToStr(byte[] bytearray)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            return encoding.GetString(bytearray);
        }
        #endregion

        #region StrToByteArray
        /// <summary>
        /// Convierte un String a arreglo de bytes. Codificación UTF8
        /// </summary>
        /// <param name="str">Cadena de texto</param>
        /// <returns>regresa arreglo de bytes</returns>
        public static byte[] StrToByteArray(string str)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            return encoding.GetBytes(str);
        }
        #endregion

        #region ConvertByteArrayToStringHex
        /// <summary>
        /// Convierte un arreglo de bytes a una cadena de texto con formato hexadecimal
        /// </summary>
        /// <param name="bytearray">Arreglo de bytes a convertir.</param>
        /// <param name="length">Longitud del Arreglo de Bytes</param>
        /// <returns>Regresa cadena de Texto</returns>
        public static string ConvertByteArrayToStringHex(byte[] bytearray, int length)
        {
            StringBuilder strB = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                //if (bytearray[i] < 0x10)
                //    strB.Append("0" + Convert.ToString(bytearray[i], 16));
                //else
                //    strB.Append(Convert.ToString(bytearray[i], 16));
                strB.Append(bytearray[i].ToString("X2"));
            }
            return strB.ToString();
        }

        #endregion

        #region ConvertByteArrayToStringHexReverse
        /// <summary>
        /// Convierte un arreglo de bytes a una cadena de texto con formato hexadecimal. Toma
        /// el  ultimo byte como el mas significativo.
        /// </summary>
        /// <param name="bytearray">Arreglo de bytes a convertir.</param>
        /// <param name="length">Longitud del Arreglo de Bytes</param>
        /// <returns>Regresa cadena de Texto</returns>
        public static string ConvertByteArrayToStringHexReverse(byte[] bytearray, int length)
        {
            StringBuilder strB = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                //if (bytearray[i] < 0x10)
                //    strB.Append("0" + Convert.ToString(bytearray[i], 16));
                //else
                //    strB.Append(Convert.ToString(bytearray[i], 16));
                strB.Append(bytearray[length - 1 - i].ToString("X2"));
            }
            return strB.ToString();
        }

        #endregion

        #region EraseStringBlankSpace
        public static string EraseStringBlankSpace(string strBuffData)
        {
            //Borra los espacios entre bytes
            for (int i = 0; i < strBuffData.Length; i++)
            {
                if (strBuffData.Substring(i, 1).CompareTo(" ") == 0)
                    strBuffData = strBuffData.Remove(i, 1);

            }
            return strBuffData;
        }

        #endregion

    
    
    }
}
