using System;
using System.Text;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace AdministracionPersonal.WebService.LogicaNegocio
{
    /// <summary>
    /// Implementacion de AES-GCM para .NET Framework 4.8.
    /// La clase System.Security.Cryptography.AesGcm solo existe desde .NET Core 3.0,
    /// por lo que aqui se usa BouncyCastle para obtener exactamente el mismo
    /// resultado que el Alcance 1 (AES-256-GCM, nonce de 12 bytes, tag de 128 bits).
    /// </summary>
    public class CriptografiaServicio : ICriptografiaServicio
    {
        private const string Prefijo = "AESGCM";
        private const int TamanoNonce = 12;  // bytes
        private const int TamanoTag = 16;    // bytes
        private const int BitsTag = TamanoTag * 8;

        private readonly byte[] _clave;

        public CriptografiaServicio(string claveAes)
        {
            if (string.IsNullOrEmpty(claveAes))
            {
                throw new ArgumentException(
                    "Security:AesKey no configurado. Debe ser la misma clave que Encryption:Key del Alcance 1.",
                    nameof(claveAes));
            }

            if (claveAes.Length != 32)
            {
                throw new ArgumentException(
                    string.Format(
                        "Security:AesKey mide {0} caracteres; debe medir exactamente 32 " +
                        "(la misma clave de 32 caracteres que Encryption:Key en el Alcance 1).",
                        claveAes.Length),
                    nameof(claveAes));
            }

            _clave = Encoding.UTF8.GetBytes(claveAes);
        }

        public bool Verificar(string passwordPlano, string passwordHash)
        {
            if (string.IsNullOrEmpty(passwordPlano) || string.IsNullOrWhiteSpace(passwordHash))
            {
                return false;
            }

            try
            {
                var partes = passwordHash.Split(':');
                if (partes.Length != 4 || partes[0] != Prefijo)
                {
                    return false;
                }

                var nonce = Convert.FromBase64String(partes[1]);
                var tag = Convert.FromBase64String(partes[2]);
                var cifrado = Convert.FromBase64String(partes[3]);

                if (nonce.Length != TamanoNonce || tag.Length != TamanoTag)
                {
                    return false;
                }

                var plano = Descifrar(nonce, tag, cifrado);
                var esperado = Encoding.UTF8.GetBytes(passwordPlano);

                return ComparacionSegura(plano, esperado);
            }
            catch
            {
                // Password hash con formato invalido, tag que no valida, etc.
                return false;
            }
        }

        private byte[] Descifrar(byte[] nonce, byte[] tag, byte[] cifrado)
        {
            // BouncyCastle espera el ciphertext y el tag concatenados.
            var entrada = new byte[cifrado.Length + tag.Length];
            Buffer.BlockCopy(cifrado, 0, entrada, 0, cifrado.Length);
            Buffer.BlockCopy(tag, 0, entrada, cifrado.Length, tag.Length);

            var cipher = new GcmBlockCipher(new AesEngine());
            cipher.Init(false, new AeadParameters(new KeyParameter(_clave), BitsTag, nonce));

            var salida = new byte[cipher.GetOutputSize(entrada.Length)];
            var escritos = cipher.ProcessBytes(entrada, 0, entrada.Length, salida, 0);
            escritos += cipher.DoFinal(salida, escritos);

            if (escritos == salida.Length)
            {
                return salida;
            }

            var resultado = new byte[escritos];
            Buffer.BlockCopy(salida, 0, resultado, 0, escritos);
            return resultado;
        }

        /// <summary>Comparacion en tiempo constante, para no filtrar informacion por el tiempo de respuesta.</summary>
        private static bool ComparacionSegura(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }

            var diferencia = 0;
            for (var i = 0; i < a.Length; i++)
            {
                diferencia |= a[i] ^ b[i];
            }

            return diferencia == 0;
        }
    }
}
