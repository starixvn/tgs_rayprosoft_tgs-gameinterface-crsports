using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;

namespace Tgs.GameInterface.CRSports.Core;

internal class CryptographyUtensil : ICryptographyUtensil
{
	public JsonNode Decrypt(string content, string key)
	{
		if (string.IsNullOrWhiteSpace(content))
		{
			throw new ArgumentNullException(content);
		}

		byte[] cipherText = Convert.FromBase64String(content);

		using var aesAlg = Aes.Create();
		aesAlg.Key = Encoding.UTF8.GetBytes(key);
		aesAlg.Mode = CipherMode.ECB; // Set the cipher mode explicitly if needed
		aesAlg.Padding = PaddingMode.PKCS7; // Set the padding mode explicitly

		var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

		var decryptedBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);

		return JsonNode.Parse(Encoding.UTF8.GetString(decryptedBytes))!;
	}

	public string Encrypt(JsonNode source, string key)
	{
		if (string.IsNullOrWhiteSpace(source.ToJsonString()))
		{
			throw new ArgumentNullException(nameof(source));
		}

		var toEncryptArray = Encoding.UTF8.GetBytes(source.ToJsonString());

		using var aesAlg = Aes.Create();
		aesAlg.Key = Encoding.UTF8.GetBytes(key);
		aesAlg.Mode = CipherMode.ECB; // Set the cipher mode explicitly if needed
		aesAlg.Padding = PaddingMode.PKCS7; // Set the padding mode explicitly

		var crypto = aesAlg.CreateEncryptor();

		var resultArray = crypto.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

		return Convert.ToBase64String(resultArray, 0, resultArray.Length);
	}
}
