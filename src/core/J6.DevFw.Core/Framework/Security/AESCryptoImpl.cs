using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace JR.DevFw.Framework.Security
{  
    
    /// <summary>
    /// AES�ԳƼ����㷨
    /// </summary>
    public class AesCryptoImpl
    {
        /// <summary>
        /// Ĭ��TOKEN,16λ
        /// </summary>
        private const string DEFAULT_TOKEN = "aescodefork3fnet";

        //Ĭ����Կ����   
        private static byte[] ivKeys = { 0x12,0x34, 0x56, 0x78, 0x90, 0xAB,
            0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78,
            0x90, 0xAB, 0xCD, 0xEF };

        #region ���ܺͽ����߼�


        /// <summary>  
        /// AES���� 
        /// </summary>  
        /// <param name="data">Ҫ���ܵ�����</param>  
        /// <param name="token">��Կ(Ĭ��128λ,16����ĸ)</param>  
        /// <param name="encoding">����</param>
        /// <returns>���ؼ��ܺ�������ֽ�����</returns>  
        public static byte[] Encrypt(byte[] data, string token)
        {
            //��������㷨  
            SymmetricAlgorithm des = Rijndael.Create();

            byte[] _keys = Encoding.UTF8.GetBytes(token);

            //������Կ����Կ����  
            des.Key = _keys;
            des.IV = ivKeys;
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(data, 0, data.Length);
            cs.FlushFinalBlock();
            byte[] cipherBytes = ms.ToArray();//�õ����ܺ���ֽ�����  
            cs.Dispose();
            ms.Dispose();
            return cipherBytes;
        }

        /// <summary>  
        /// AES����  
        /// </summary>  
        /// <param name="decData">�����ֽ�����</param>  
        /// <param name="token">��Կ</param>  
        /// <returns>���ؽ��ܺ���ַ���</returns>  
        public static byte[] Decrypt(byte[] decData, string token)
        {
            // if (decData.Length != ivKeys.Length)
            //{
            //    throw new CryptographicException("keys length is not valid!");
            //}

            SymmetricAlgorithm des = Rijndael.Create();
            des.Key = Encoding.UTF8.GetBytes(token);
            des.IV = ivKeys;
            byte[] decryptBytes = new byte[decData.Length];
            MemoryStream ms = new MemoryStream(decData);
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Read);
            cs.Read(decryptBytes, 0, decryptBytes.Length);
            cs.Dispose();
            ms.Dispose();
            return decryptBytes;
        }

        /// <summary>
        /// ���ܵ�Base64�ַ���
        /// </summary>
        /// <param name="data"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string EncryptToBase64(byte[] data, string token)
        {
            byte[] encData = Encrypt(data, token);
            return Convert.ToBase64String(encData);
        }

        /// <summary>
        /// ��Base64�ַ����н���
        /// </summary>
        /// <param name="encyptStr"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static byte[] DecryptFromBase64(string encyptStr, string token)
        {
            byte[] data = Convert.FromBase64String(encyptStr);
            return Decrypt(data, token);
        }


        public static string EncryptToBase64(string str, string token)
        {
            return EncryptToBase64(Encoding.UTF8.GetBytes(str), token);
        }

        #endregion




        #region ��Token����


        public static string Decrypt(string encyptBase64Str, string token)
        {
            return Encoding.UTF8.GetString(DecryptFromBase64(encyptBase64Str, token));
        }

        public static byte[] Encrypt(byte[] data)
        {
            return Encrypt(data, DEFAULT_TOKEN);
        }

        public static byte[] Decrypt(byte[] decData)
        {
            return Decrypt(decData, DEFAULT_TOKEN);
        }

        public static string EncryptToBase64(byte[] data)
        {
            return EncryptToBase64(data, DEFAULT_TOKEN);
        }

        public static byte[] DecryptFromBase64(string encyptStr)
        {
            return DecryptFromBase64(encyptStr, DEFAULT_TOKEN);
        }

        public static string EncryptToBase64(string str)
        {
            return EncryptToBase64(str, DEFAULT_TOKEN);
        }

        public static string Decrypt(string encyptBase64Str)
        {
            return Decrypt(encyptBase64Str, DEFAULT_TOKEN);
        }
        #endregion
    }
}