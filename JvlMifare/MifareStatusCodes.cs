using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JvlMifare
{
   /// <summary>
   /// Codigos de respuesta de la clase MifareCommands y MifareUI
   /// </summary>
    public partial class MifareStatusCodes
    {
        //Códigos de Estatus
        /// <summary>
        /// Exito al ejecutar la función. Status = 0x9000
        /// </summary>
        public const int SUCCESS = 0x9000;

        /// <summary>
        /// La tarjeta no es invalida. Esto ocurre cuando se presenta una tarjeta
        /// que no es iClass. Status = 0x2001.
        /// </summary>
        public const int INVALID_CARD = 0x2001;
        /// <summary>
        /// La tarjeta no esta presente en el lector. Status = 0x2002.
        /// </summary>
        public const int CARD_NOT_PRESENT = 0x2002;
        /// <summary>
        /// El arreglo de datos no tiene el suficiente espacio. Status = 0x2003.
        /// </summary>
        public const int NOT_ENOUGH_MEMORY = 0x2003;
        /// <summary>
        /// El bloque seleccionado no tiene el formato de bloque de valor. Status = 0x2004
        /// </summary>
        public const int INVALID_VALUE_BLOCK = 0x2004;
        /// <summary>
        /// El valor del bloque excede el límite de 65535. Status = 0x2005.
        /// </summary>
        public const int VALUE_LIMIT = 0x2005;
        /// <summary>
        /// El valor del bloque de valor es menor que cero. Status = 0x2006.
        /// </summary>
        public const int VALUE_LESS_THAN_ZERO = 0x2006;
        /// <summary>
        /// La longitud de la llave es incorrecta. Status = 0x2007.
        /// </summary>
        public const int WRONG_KEY_LENGTH = 0x2007;
        /// <summary>
        /// El arreglo de bytes esta vacio o es nulo. Status = 0x2008
        /// </summary>
        public const int NULL_VALUE = 0x2008;
        /// <summary>
        /// La clave de activación para lecturas y escritura es incorrecta
        /// </summary>
        public const int WRONG_ACTIVATION_KEY = 0x2009;
        /// <summary>
        /// Falla en iniciar una sesión segura con lector Omnikey. Se requiere sesión segura para
        /// cambiar llaves en tarjeta y obtener el ID.
        /// </summary>
        public const int FAILED_START_SECURE_SESSION = 0x2010;
        /// <summary>
        /// No se puede identificar la encriptación del ID.
        /// </summary>
        public const int WRONG_ID_ENCRYPTION = 0x2011;
        /// <summary>
        /// Los datos a convertir están incompletos. Status = 0x4001
        /// </summary>
        public const int DATOS_INCOMPLETOS = 0x4001;
        /// <summary>
        /// El formato es incorrecto. No tiene formato hexadecimal. Status = 0x4002.
        /// </summary>
        public const int FORMATO_INCORRECTO = 0x4002;
        
        /// <summary>
        /// La autenticación del área no pudo ser hecha. Esto puede ser ocasionado por seleccionar
        /// una llave incorrecta. Status = 0x6983
        /// </summary>
        public const int AUTH_CANNOT_BE_DONE = 0x6983;

        /// <summary>
        /// La llave seleccionada no es valida para hacer la operacion o no se ha realizado previamente 
        /// una autenticacion.
        /// </summary>
        public const int SECURITY_STATUS_NOT_SATISFIED = 0x6982;
        /// <summary>
        /// Falla en la escritura de datos en la memoria. En mifare puede ser ocasionado por una selección
        /// erronea del tipo de llave o por no haber hecho previamente la autenticacion. Revisar los bits de configuración. 
        /// </summary>
        public const int UNSUCCESSFUL_WRITING = 0x6581;


        public const int INVALID_BLOCK_ADDRESS = 0x6A82;


        #region GetMifareStatusCodeInfo
        /// <summary>
        /// Obtiene en cadena de texto la descripcion del MifareStatusCode
        /// </summary>
        /// <param name="code">MifareStatusCode</param>
        /// <returns>Cadena con la descripcion del MifareStatusCode</returns>
        public static string GetMifareStatusCodeInfo(int code)
        {
            string strInfo = code.ToString("X4");

            switch (code)
            {
                case SUCCESS:
                    strInfo = "SUCCESS" ;
                    break;
                case INVALID_CARD:
                    strInfo = "INVALID_CARD";
                    break;
                case CARD_NOT_PRESENT:
                    strInfo = "CARD_NOT_PRESENT";
                    break;
                case NOT_ENOUGH_MEMORY:
                    strInfo = "NOT_ENOUGH_MEMORY";
                    break;
                case INVALID_VALUE_BLOCK:
                    strInfo = "INVALID_VALUE_BLOCK";
                    break;
                case VALUE_LIMIT:
                    strInfo = "VALUE_LIMIT";
                    break;
                case AUTH_CANNOT_BE_DONE:
                    strInfo = "AUTH_CANNOT_BE_DONE";
                    break;
                case SECURITY_STATUS_NOT_SATISFIED:
                    strInfo = "SECURITY_STATUS_NOT_SATISFIED";
                    break;
                case UNSUCCESSFUL_WRITING:
                    strInfo = "UNSUCCESSFUL_WRITING";
                    break;
                case INVALID_BLOCK_ADDRESS:
                    strInfo = "INVALID_BLOCK_ADDRESS";
                    break;             
                default: break;
            }
            return strInfo;


        }
        #endregion
    }
}
