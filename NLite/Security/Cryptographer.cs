/*
 * Created by SharpDevelop.
 * User: issuser
 * Date: 2010-10-12
 * Time: 10:35
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;

using NLite.Internal;

namespace NLite.Security
{
	 /// <summary>
    /// 对称算法类型
    /// </summary>
    public enum SymmetricAlgorithmType
    {
        DES,
        RC2,
        RC4,
        Rijindael,
        TripleDES,
    }

    /// <summary>
    /// 对称加密
    /// </summary>
    public class Cryptographer
    {
        public Cryptographer(){}
        public Cryptographer(string key):this(key,Cryptographer.DEFAULT_IV,SymmetricAlgorithmType.Rijindael){}
        public Cryptographer(string key,string iv):this(key,iv,SymmetricAlgorithmType.Rijindael){}
        public Cryptographer(string key,string iv,SymmetricAlgorithmType symmetricAlgorithmType)
        {
            this.key = key;
            this.iv = iv;
            this.symmetricAlgorithmType = symmetricAlgorithmType;
        }

        /// <summary>
        /// 缺省密钥键
        /// </summary>
        private const string DEFAULT_KEY = "aslkjkljlsajsuaggasfklrjuisdhaie3084068406{*&^%$wslfdjsodfji";
        /// <summary>
        /// 缺省初始化向量IV
        /// </summary>
        private const string DEFAULT_IV  = "E4ghj*Ghg7!rNIfb&95GUY86GfghUb#er57HBh(u%g6HJ($jhWk7&!hg4ui%";

        private string key =DEFAULT_KEY;
        /// <summary>
        /// 密钥键
        /// </summary>
        public string Key
        {
            get { return key;}
            set { key = value;}
        }

        private string iv = DEFAULT_IV;
        /// <summary>
        /// 初始化向量IV
        /// </summary>
        public string IV 
        {
            get { return iv;}
            set { iv = value;}
        }

        private SymmetricAlgorithmType symmetricAlgorithmType = SymmetricAlgorithmType.Rijindael;
        /// <summary>
        /// 对称算法类型
        /// </summary>
        public SymmetricAlgorithmType SymmetricAlgorithmType
        {
            get { return symmetricAlgorithmType;}
            set { symmetricAlgorithmType = value;}
        }

        #region Private Members
        private byte[] GetLegalKey(SymmetricAlgorithm cryptoObj)
        {
            if (string.IsNullOrEmpty(key))
                key = DEFAULT_KEY;

            string sTemp = key;
            cryptoObj.GenerateKey();
            byte[] bytTemp = cryptoObj.Key;
            int KeyLength = bytTemp.Length;
            if (sTemp.Length > KeyLength)
                sTemp = sTemp.Substring(0, KeyLength);
            else if (sTemp.Length < KeyLength)
                sTemp = sTemp.PadRight(KeyLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }

        private byte[] GetLegalIV(SymmetricAlgorithm cryptoObj)
        {
            if (string.IsNullOrEmpty(iv))
                return null;//iv = DEFAULT_IV;

            string sTemp = iv;
            cryptoObj.GenerateIV();
            byte[] bytTemp = cryptoObj.IV;
            int IVLength = bytTemp.Length;
            if (sTemp.Length > IVLength)
                sTemp = sTemp.Substring(0, IVLength);
            else if (sTemp.Length < IVLength)
                sTemp = sTemp.PadRight(IVLength, ' ');
            return ASCIIEncoding.ASCII.GetBytes(sTemp);
        }

        private static SymmetricAlgorithm GetSymmetricAlgorithm(SymmetricAlgorithmType type)
        {
            switch(type)
            {
                case SymmetricAlgorithmType.DES:return DES.Create();
                case SymmetricAlgorithmType.RC2:return RC2.Create();
                case SymmetricAlgorithmType.Rijindael:return Rijndael.Create();
                case SymmetricAlgorithmType.TripleDES:return TripleDES.Create();
                    //case SymmetricAlgorithmType.RC4: return Org.Mentalis.SecurityServices.Cryptography.RC4.Create();
                default: return Rijndael.Create();
            }
        }
        #endregion

        #region public Members

        /// <summary>
        /// 对称加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string SymmetricEncrpyt(string str)
        {
            Guard.NotNullOrEmpty(str,"str");
			
            SymmetricAlgorithm cryptoObj = GetSymmetricAlgorithm(this.symmetricAlgorithmType);

            byte[] bytIn = UTF8Encoding.UTF8.GetBytes(str);
            using (MemoryStream ms = new MemoryStream())
            {

                cryptoObj.Key = GetLegalKey(cryptoObj);
                cryptoObj.IV = GetLegalIV(cryptoObj);

                ICryptoTransform encrypto = cryptoObj.CreateEncryptor();
                using (CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write))
                {
                    cs.Write(bytIn, 0, bytIn.Length);
                    cs.FlushFinalBlock();
                    ms.Close();
                    byte[] bytOut = ms.ToArray();
                    return Convert.ToBase64String(bytOut);
                }
            }
        }

        /// <summary>
        /// 对称解密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string SymmetricDecrpyt(string str)
        {
            Guard.NotNullOrEmpty(str, "str");

            using (SymmetricAlgorithm cryptoObj = GetSymmetricAlgorithm(this.symmetricAlgorithmType))
            {

                byte[] bytIn = Convert.FromBase64String(str);
                using (MemoryStream ms = new MemoryStream(bytIn, 0, bytIn.Length))
                {

                    cryptoObj.Key = GetLegalKey(cryptoObj);
                    cryptoObj.IV = GetLegalIV(cryptoObj);

                    ICryptoTransform encrypto = cryptoObj.CreateDecryptor();
                    using (CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// MD5加密类,注意经MD5加密过的信息是不能转换回原始数据的
        /// ,请不要在用户敏感的信息中使用此加密技术,比如用户的密码,
        /// 并且需要找会密码的功能请尽量使用对称加密
        /// </summary>
        public string MD5Encrpyt(string str)
        {
            byte[] toCompute=Encoding.Unicode.GetBytes(str);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] hashed = md5.ComputeHash(toCompute, 0, toCompute.Length);
                return Convert.ToBase64String(hashed);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string SHA1Encrpyt(string str)
        {
            Guard.NotNullOrEmpty(str, "str");

            byte[] bytIn = UTF8Encoding.UTF8.GetBytes(str);
            using (HashAlgorithm sha = new SHA1CryptoServiceProvider())
            {
                byte[] hashed = sha.ComputeHash(bytIn);
                return Convert.ToBase64String(hashed);
            }
        }

        #endregion
    }
}
