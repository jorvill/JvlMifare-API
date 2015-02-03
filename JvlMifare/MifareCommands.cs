/** MifareCommands.cs
 * 
 * Author: Jorge Villicana Lemus
 * 
 * */


using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using JvlSCard;

namespace JvlMifare
{
    public partial class MifareCommands
    {
        private uint _hCard = 0;
        private string _strReaderName = "OMNIKEY Cardman 5x21-CL 0";
        private int _cardType = 0;

        public const int KEYA = 0x60;
        public const int KEYB = 0x61;
        

        private SCardRoutines scardr;

        #region CONSTRUCTORS

        public MifareCommands(uint  phCard)
        {
            _hCard = phCard;
            scardr = new SCardRoutines();
        }

        public MifareCommands(string strReaderName)
        {
            _strReaderName = strReaderName;
            scardr = new SCardRoutines();
        }

        public MifareCommands()
        {
            
            scardr = new SCardRoutines();
        }


#endregion


        //CARD CONECTION COMMANDS

        #region ConnectCard
        /// <summary>
        /// Get connection with the Mifare Card presented to the reader
        /// </summary>
        /// <returns>SUCCESS or   MifareStatusCode</returns>
        public int ConnectCard()
        {
            int status = scardr.StartCardConnection(ref _cardType, _strReaderName);
            if (status == 0)
                status = MifareStatusCodes.SUCCESS;
            else
                status = MifareStatusCodes.CARD_NOT_PRESENT;
            return status;
        }
        #endregion
       
        #region DisconnectCard
        /// <summary>
        /// TClose connection with the Mifare Card
        /// </summary>
        /// <returns>SUCCESS or  MifareStatusCode</returns>
        public int DisconnectCard()

        {
            int status = scardr.DisconnectCard();
            if (status == 0)
                status = MifareStatusCodes.SUCCESS;
            else
                status = MifareStatusCodes.CARD_NOT_PRESENT;
            return status;
        }
        #endregion

        #region CheckIfCardIsPresent
        /// <summary>
        /// Verifica si hay una tarjeta presente en el lector.
        /// </summary>
        /// <returns>True si hay una tarjeta presente, False en caso contrario</returns>
        public bool CheckIfCardIsPresent()
        {
            bool b = false;
       
            int cardstate = scardr.GetReaderState(_strReaderName);

            if (cardstate == SCardRoutines.CARD_PRESENT)
                b = true;
            if (cardstate == SCardRoutines.CARD_NOT_PRESENT)
                b = false;

            return b;
        }

        #endregion

        #region ListReaders
        /// <summary>
        /// Devuelve una lista de los lectores Omnikey conectados.
        /// </summary>
        /// <returns>Arraylist con cadena de nombre de lectores conectados.</returns>
        public ArrayList ListReaders()
        {           
            ArrayList readerlist = scardr.ListReaders();
            return readerlist;
        }
        #endregion

        #region SetReader
        public void SetReader(string strReaderName)
        {
            _strReaderName = strReaderName;
        }
        #endregion

        #region GetCardType
        /// <summary>
        /// Devuelve un entero con el identificador del tipo de tarjeta.
        /// </summary>
        /// <returns>tipo de tarjeta</returns>
        public int GetCardType()
        {
            return scardr.GetTypeOfCard();
        }

        #endregion




        //CARD IDENTIFICATION COMMANDS

        #region GetCSN
        /// <summary>
        /// Obtiene el número serial de la tarjeta mifare.
        /// </summary>
        /// <param name="csn">Arreglo de 6 bytes donde se guarda el CSN.</param>
        /// <param name="numbytes">Número de bytes recibidos</param>
        /// <returns>SUCCESS en caso de Exito o MifareStatusCodes.</returns>
        public int GetCSN(ref byte[] csn, ref int numbytes)
        {
            int status = 0;
            byte[] ucByteSend = new byte[16];
            byte[] pucReceiveData = new byte[16];
            ucByteSend[0] = 0xFF;//CLA 
            ucByteSend[1] = 0xCA;//INS 
            ucByteSend[2] = 0x00;//P1 
            ucByteSend[3] = 0x00;//P2 
            ucByteSend[4] = 0x00; //Le
            int ulnByteSend = 5;
            int pullReceiveDataBufLen = 36;

            int rc = scardr.Transmit(ucByteSend, ulnByteSend, ref pucReceiveData, ref pullReceiveDataBufLen);
           
            if (rc == 0)
            {
                status = pucReceiveData[pullReceiveDataBufLen - 2] << 8;
                status = status | pucReceiveData[pullReceiveDataBufLen - 1];
                
                if (status == MifareStatusCodes.SUCCESS)
                {
                    for (int i = 0; i < pullReceiveDataBufLen - 2; i++)
                        csn[i] = pucReceiveData[i];
                    numbytes = pullReceiveDataBufLen - 2;
                }

            }


            return status;
        }
        #endregion


        //DATA BLOCK - READ & WRITE  COMMANDS
       
        #region ReadData
        /// <summary>
        /// Lee datos de un bloque de memoria. Se requiere previa autenticación con el comando Authenticate.
        /// </summary>
        /// <param name='blocknumber'>Número de bloque del área de memoria. 
        /// blocknumber = numero de sector * 4 + numero de bloque del sector (0 a 3)</param>
        /// <param name='blockdata'> bloque de datos (16 bytes) que se lee. Regresa null en caso de error.</param>
        /// <returns>SUCCESS en caso de Exito o MifareStatusCodes</returns>

        public int ReadData(int blocknumber, ref byte[] datablock)
        {
            int status = 0;
            byte[] pucSendata = new byte[8];
            byte[] pucReceiveData = new byte[24];

            pucSendata[0] = 0xFF; //CLA
            pucSendata[1] = 0xB0; //INS
            pucSendata[2] = (byte)(blocknumber >> 8); //P1 MSB de blocknumber
            pucSendata[3] = (byte)(blocknumber & 0x00FF); //P2 LSB de blocknumber
            pucSendata[4] = 0x10; // Le Recibe 16 bytes

            int ulSendDataBuflen = 5;
            int pullReceiveDataBufLen = 32;
            
            int rc = scardr.Transmit(pucSendata, ulSendDataBuflen, ref pucReceiveData, ref pullReceiveDataBufLen);
            //Analiza Respuesta
            if (rc == 0)
            {
                status = pucReceiveData[pullReceiveDataBufLen - 2] << 8;
                status = status | pucReceiveData[pullReceiveDataBufLen - 1];
                if (status == 0x9000)
                {
                    for (int i = 0; i < pullReceiveDataBufLen - 2; i++)
                        datablock[i] = pucReceiveData[i];
                }
                else
                {

                    datablock = null;
                }
            }
            else
            {
                status = -1;
            }
            return status;

        }
        #endregion

        #region WriteData
        /// <summary>
        /// Escribe en un bloque de datos de la tarjeta Mifare. Se requiere previa autenticacion con el comando Authenticate.
        /// </summary>
        /// <param name="blocknumber">Numero del bloque de datos. 
        /// blocknumber = numero de sector * 4 + numero de bloque del sector (0 a 3)</param>
        /// <param name="blockdata">Arreglo de 16 bytes con los datos del bloque</param>
        /// <returns>SUCCESS en caso de Exito o MifareStatusCode</returns>

        public int WriteData(int blocknumber, byte[] blockdata)
        {
            int status = 0;
            byte[] pucSendata = new byte[32];
            byte[] pucReceiveData = new byte[32];

            pucSendata[0] = 0xFF; //CLA
            pucSendata[1] = 0xD6; //INS
            pucSendata[2] = 0x00; //P1 MSB de blocknumber
            pucSendata[3] = (byte)(blocknumber & 0x00FF); //P2 LSB de blocknumber
            pucSendata[4] = 0x10; //LC
            for (int i = 0; i < 16; i++)
                pucSendata[5 + i] = blockdata[i];
            int ulSendDataBuflen = 21;
            int pullReceiveDataBufLen = 4;
            int rc = scardr.Transmit(pucSendata, ulSendDataBuflen, ref pucReceiveData, ref pullReceiveDataBufLen);
            //Analiza Respuesta
            if (rc == 0)
            {
                status = pucReceiveData[pullReceiveDataBufLen - 2] << 8;
                status = status | pucReceiveData[pullReceiveDataBufLen - 1];
            }
            else
            {
                status = -1;
            }
            return status;

        }
        #endregion


        //SECURITY COMMANDS

        #region LoadKeyReader
        /// <summary>
        /// Carga llave en el lector
        /// </summary>
        /// <param name="keynumber">numero de locacion donde</param>
        /// <param name="P1"></param>
        /// <param name="key"></param>
        /// <param name="keylength"></param>
        /// <returns></returns>

        private int LoadReaderKey(int keynumber, int P1, byte[] key, int keylength)
        {
            int status = 0;
            byte[] pucSendata = new byte[16];
            byte[] pucReceiveData = new byte[8];

            pucSendata[0] = 0xFF; //CLA
            pucSendata[1] = 0x82; //INS
            pucSendata[2] = (byte)P1; //P1 Reader Key, Format for key location
            pucSendata[3] = (byte)keynumber; //P2 Keynumber
            pucSendata[4] = (byte)keylength; //Lc
            for (int i = 0; i < keylength; i++)
                pucSendata[5 + i] = key[i];
            int ulSendDataBuflen = keylength + 5;
            int pullReceiveDataBufLen = 32;
            int rc = scardr.Transmit(pucSendata, ulSendDataBuflen, ref pucReceiveData, ref pullReceiveDataBufLen);
            //Analiza Respuesta
            if (rc == 0)
            {
                status = pucReceiveData[pullReceiveDataBufLen - 2] << 8;
                status = status | pucReceiveData[pullReceiveDataBufLen - 1];
            }
            else
            {
                status = -1;
            }
            return status;

        }
        #endregion

        #region LoadVolatileKey
        /// <summary>
        /// Carga llave temporal en lector
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keylength"></param>
        /// <returns></returns>
        public int LoadVolatileKey(byte[] key, int keylength)
        {
            //0xF0 key number para KVAK. KVAK se mantiene por cada sesión. Es necesario volverla a cargar si hay una 
            //desconenxión.
            return LoadReaderKey(0x00, 0x00, key, keylength);


        }
        #endregion

        #region LoadKey
       /// <summary>
       /// Carga una llave Mifare en el lector Omnikey 5x21.
       /// </summary>
       /// <param name="key">arreglo de bytes que representa la llave de 6 bytes.</param>
       /// <param name="keynumber">locacion de memoria del lector Omnikey donde se almacenara la llave.</param>
       /// <param name="keylength">longitud de la llave.</param>
       /// <returns>SUCCES en caso de Exito o MifareStatusCode</returns>
        public int LoadKey(byte[] key, int keynumber, int keylength)
        {
            //0xF0 key number para KVAK. KVAK se mantiene por cada sesión. Es necesario volverla a cargar si hay una 
            //desconenxión.
            return LoadReaderKey(keynumber, 0x20, key, keylength);


        }


        #endregion

        #region Authenticate
       /// <summary>
       /// 
       /// </summary>
       /// <param name="keytype"></param>
       /// <param name="keynumber"></param>
       /// <param name="blocknumber"></param>
       /// <returns></returns>

        public int Authenticate(int keytype, int keynumber, int blocknumber)
        {
            int status = 0;
            byte[] pucSendata = new byte[16];
            byte[] pucReceiveData = new byte[16];

           
            ///ushort address = (ushort)(4 * sector);

            ushort address = (ushort)(blocknumber);
            
            pucSendata[0] = 0xFF; //CLA
            pucSendata[1] = 0x86; //INS
            pucSendata[2] = 0x00; //P1
            pucSendata[3] = 0x00; //P2
            pucSendata[4] = 0x05; //Lc
            pucSendata[5] = 0x01;
            pucSendata[6] = (byte)((address & 0xFF00) >> 8); //MSB Address
            pucSendata[7] = (byte)((address & 0x00FF)); //LSB Address
            pucSendata[8] = (byte)keytype; //Key type
            pucSendata[9] = (byte)keynumber; //Key number

            int ulSendDataBuflen = 10;
            int pullReceiveDataBufLen = 10;
            int rc = scardr.Transmit(pucSendata, ulSendDataBuflen, ref pucReceiveData, ref pullReceiveDataBufLen);
            //Analiza Respuesta
            if (rc == 0)
            {
                status = pucReceiveData[pullReceiveDataBufLen - 2] << 8;
                status = status | pucReceiveData[pullReceiveDataBufLen - 1];
            }
            else
            {
                status = -1;
            }
            return status;

        }
        #endregion


        //VALUE BLOCK COMMANDS

        #region Increment
        /// <summary>
        /// Incrementa el bloque de valor.
        /// </summary>
        /// <param name="blocknumber">Numero del bloque que se va incrementar. 
        /// blocknumber = 4 * Numero de Sector + Numero de bloque de Sector (0 a 3)</param>
        /// <param name="value">Cantidad en la que se va incrementar el bloque de valor.</param>
        /// <returns>SUCCES en caso de Exito o MifareStatusCode</returns>
        public int Increment(int blocknumber, uint value)
        {

            int status = 0;
            byte[] pucSendata = new byte[9];
            byte[] pucReceiveData = new byte[24];

            pucSendata[0] = 0xFF; //CLA
            pucSendata[1] = 0xD4; //INS
            pucSendata[2] = (byte)(blocknumber >> 8); //P1 MSB de blocknumber
            pucSendata[3] = (byte)(blocknumber & 0x00FF); //P2 LSB de blocknumber
            pucSendata[4] = 0x04; // LC
            pucSendata[5] = (byte)((value & 0xff000000) >> 24);
            pucSendata[6] = (byte)((value & 0x00ff0000) >> 16);
            pucSendata[7] = (byte)((value & 0x0000ff00) >> 8);
            pucSendata[8] = (byte)((value & 0x000000ff));


            int ulSendDataBuflen = 9;
            int pullReceiveDataBufLen = 32;


            int rc = scardr.Transmit(pucSendata, ulSendDataBuflen, ref pucReceiveData, ref pullReceiveDataBufLen);
            //Analiza Respuesta
            if (rc == 0)
            {
                status = pucReceiveData[pullReceiveDataBufLen - 2] << 8;
                status = status | pucReceiveData[pullReceiveDataBufLen - 1];

            }
            else
            {
                status = -1;
            }

            return status;
        }
        #endregion

        #region Decrement
        /// <summary>
        /// Decrementa bloque de valor
        /// </summary>
        /// <param name="blocknumber">Numero del bloque que se va decrementar</param>
        /// <param name="value">Cantidad en la que se decrementara el bloque</param>
        /// <returns>SUCCESS en caso de Exito o MifareStatusCode</returns>
        public int Decrement(int blocknumber, int value)
        {
            int status = 0;
            byte[] pucSendata = new byte[9];
            byte[] pucReceiveData = new byte[24];

            pucSendata[0] = 0xFF; //CLA
            pucSendata[1] = 0xD8; //INS
            pucSendata[2] = (byte)(blocknumber >> 8); //P1 MSB de blocknumber
            pucSendata[3] = (byte)(blocknumber & 0x00FF); //P2 LSB de blocknumber
            pucSendata[4] = 0x04; // LC
            pucSendata[5] = (byte)((value & 0xff000000) >> 32);
            pucSendata[6] = (byte)((value & 0x00ff0000) >> 16);
            pucSendata[7] = (byte)((value & 0x0000ff00) >> 8);
            pucSendata[8] = (byte)((value & 0x000000ff));


            int ulSendDataBuflen = 9;
            int pullReceiveDataBufLen = 32;
            int rc = scardr.Transmit(pucSendata, ulSendDataBuflen, ref pucReceiveData, ref pullReceiveDataBufLen);
            //Analiza Respuesta
            if (rc == 0)
            {
                status = pucReceiveData[pullReceiveDataBufLen - 2] << 8;
                status = status | pucReceiveData[pullReceiveDataBufLen - 1];

            }
            else
            {
                status = -1;
            }
            return status;
        }
        #endregion

      
       

        
    }
}
