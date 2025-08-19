using System;

namespace SurveyApp.Services;

public interface IRsaCryptoService
{
	string GetPublicKeyPem();
	string DecryptBase64(string base64CipherText);
}


