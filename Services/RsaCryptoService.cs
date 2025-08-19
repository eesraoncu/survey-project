using System.Security.Cryptography;
using System.Text;

namespace SurveyApp.Services;

public class RsaCryptoService : IRsaCryptoService
{
	private readonly RSA _rsa;

	public RsaCryptoService()
	{
		_rsa = RSA.Create(2048);
	}

	public string GetPublicKeyPem()
	{
		var publicKey = _rsa.ExportSubjectPublicKeyInfoPem();
		return publicKey;
	}

	public string DecryptBase64(string base64CipherText)
	{
		var cipherBytes = Convert.FromBase64String(base64CipherText);
		var plainBytes = _rsa.Decrypt(cipherBytes, RSAEncryptionPadding.OaepSHA256);
		return Encoding.UTF8.GetString(plainBytes);
	}
}


