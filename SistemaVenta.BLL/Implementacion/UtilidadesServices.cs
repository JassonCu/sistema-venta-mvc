using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using SistemaVenta.BLL.Interfaces;

namespace SistemaVenta.BLL.Implementacion
{
    public class UtilidadesServices : IUtilidades
    {
        public string GenerarClave()
        {
            // Generar una clave aleatoria de 6 caractere
            string clave = Guid.NewGuid().ToString("N").Substring(0, 6);

            return clave;
        }
        public string ConvertirSha256(string texto)
        {
            // Convertir el texto a Sha256 y retornar el resultado en una cadena de texto. 
            // Ejemplo: "Hola mundo" -> "b109f3bbbc244eb82441917ed06d618b9008dd09b3befd1b5e07394c706a8bb980b1d7785e5976ec049b46df5f1326af5a2ea6d103fd07c95385ffab0cacbc86"
            StringBuilder Sb = new StringBuilder();

            using(SHA256 hash = SHA256Managed.Create()) 
            {
                // Convertir el texto a Sha256 y retornar el resultado en una cadena de texto.
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));
                
                foreach(byte b in result)
                {
                    Sb.Append(b.ToString("x2"));
                }
            }

            return Sb.ToString();
        }

    }
}
