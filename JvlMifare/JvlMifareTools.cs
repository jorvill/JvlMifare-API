/**
 * Author: Jorge Villicana
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace JvlMifare
{
    public partial class JvlMifareTools
    {

        public JvlMifareTools()
        {

        }

        #region GetCardTypeInfo
        public string GetCardTypeInfo(int cardType)
        {
            string strCT = "Unknown";

            switch (cardType)
            {
                case  0x0001: strCT = "Mifare_Stda_1k"; break;
                case  0x0002: strCT = "Mifare_Stda_4k"; break;
                case  0x0003: strCT = "Mifare_Ultralight"; break;
                case  0x0026: strCT = "Mifare_Mini"; break;
                default: strCT = "Unknown"; break;
            }

            return strCT;
        }

        #endregion

        #region GetAccessBitsInfo
        /// <summary>
        /// Obtiene informacion de los bits de acceso del sector seleccionado.
        /// </summary>
        /// <param name="blockData">datos del bloque 3 del sector seleccionado</param>
        /// <returns>cadena de texto con la interpretacion de los bits de acceso</returns>
        public string GetAccessBitsInfo(byte[] blockData)
        {
            StringBuilder strInfo = new StringBuilder();

            int[,] accessbits = new int[4, 3];

            accessbits[0, 0] = (blockData[7] & 0x10) > 1 ? 1 : 0;
            accessbits[1, 0] = (blockData[7] & 0x20) > 1 ? 1 : 0;
            accessbits[2, 0] = (blockData[7] & 0x40) > 1 ? 1 : 0;
            accessbits[3, 0] = (blockData[7] & 0x80) > 1 ? 1 : 0;

            accessbits[0, 1] = (blockData[8] & 0x01) >= 1 ? 1 : 0;
            accessbits[1, 1] = (blockData[8] & 0x02) > 1 ? 1 : 0;
            accessbits[2, 1] = (blockData[8] & 0x04) > 1 ? 1 : 0;
            accessbits[3, 1] = (blockData[8] & 0x08) > 1 ? 1 : 0;

            accessbits[0, 2] = (blockData[8] & 0x10) > 1 ? 1 : 0;
            accessbits[1, 2] = (blockData[8] & 0x20) > 1 ? 1 : 0;
            accessbits[2, 2] = (blockData[8] & 0x40) > 1 ? 1 : 0;
            accessbits[3, 2] = (blockData[8] & 0x80) > 1 ? 1 : 0;


            int value = 0;

            strInfo.Append("ACCESS CONDITIONS FOR DATA BLOCKS\n");
            strInfo.Append("READ   WRITE  INCREMENT   DECREMENT/TRANS\n");
            
            for (int i = 0; i < 3; i++)
            {

                value = accessbits[i, 0] << 1;
                value = (value + accessbits[i, 1]) << 1;
                value = value + accessbits[i, 2];

                strInfo.Append(InterpAccessBitsDataBlocks(value));
                strInfo.Append("\n");
            }
            strInfo.Append("\n");
            strInfo.Append("ACCESS CONDITIONS FOR SECTOR TRAILER\n"); 
            strInfo.Append("   Key A        AccessBits     Key B\n"); 
            strInfo.Append("READ  WRITE    READ  WRITE    READ  WRITE\n"); 
            strInfo.Append("\n");
            value = accessbits[3, 0] << 1;
            value = (value + accessbits[3, 1]) << 1;
            value = value + accessbits[3, 2];
            strInfo.Append(InterpAccessBitsSectorTrailer(value)); strInfo.Append("\n\n"); 

            return strInfo.ToString();
        }
        #endregion

        #region Access Bits Interpretation for Data Blocks
        String InterpAccessBitsDataBlocks(int accessbits)
        {
            string result;
            switch (accessbits)
            {
                case 0x0:
                    result = "A|B     A|B      A|B        A|B              Transporte";
                    break;
                case 0x2:
                    result = "A|B     never    never      never            RW Block";
                    break;
                case 0x4:
                    result = "A|B      B       never      never            RW Block";
                    break;
                case 0x6:
                    result = "A|B      B        B         A|B              Value Block";
                    break;
                case 0x1:
                    result = "A|B      never    never     A|B              Value Block";
                    break;
                case 0x3:
                    result = "B        B        never     never            RW Block";
                    break;
                case 0x5:
                    result = "B        never    never     never            RW Block";
                    break;
                case 0x7:
                    result = "never    never    never     never            RW Block";
                    break;
                default:
                    result = "Incorrect";
                    break;



            }
            return result;

        }
        #endregion


        #region Access Bits Interpretation for Trailer
        String InterpAccessBitsSectorTrailer(int accessbits)
        {
            string result;
            switch (accessbits)
            {
                case 0x0:
                    result = "never   A       A   never     A     A";
                    break;
                case 0x2:
                    result = "never   never   A   never     A     never";
                    break;
                case 0x4:
                    result = "never   B       A|B never     never B";
                    break;
                case 0x6:
                    result = "never   never   A|B never     never never";
                    break;
                case 0x1:
                    result = "never   A       A    A        A     A";
                    break;
                case 0x3:
                    result = "never   B       A|B  B        never B";
                    break;
                case 0x5:
                    result = "never   never   A|B  B        never never";
                    break;
                case 0x7:
                    result = "never   never   A|B  never    never never";
                    break;
                default:
                    result = "Incorrect";
                    break;



            }
            return result;
        }
        #endregion
    }
}
