using System.Text.Json.Nodes;

namespace Tgs.GameInterface.CRSports.Core;

public interface ICryptographyUtensil
{
	public string Encrypt(JsonNode source, string key);

	public JsonNode Decrypt(string content, string key);
}
