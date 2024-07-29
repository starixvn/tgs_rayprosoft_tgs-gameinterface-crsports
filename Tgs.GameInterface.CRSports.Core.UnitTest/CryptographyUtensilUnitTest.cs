using System.Text.Json.Nodes;

namespace Tgs.GameInterface.CRSports.Core.UnitTest;

public class CryptographyUtensilUnitTest
{
	[Fact]
	public void Encrypt()
	{
		var source = new JsonObject
		{
			["username"] = "RT706test1",
			["password"] = "aaa123",
			["timestamp"] = 1721703786958,
		};
		string key = "FnzrUTi5fcsdrws7";

		var sut = new CryptographyUtensil();

		var actual = sut.Encrypt(source, key);

		Assert.NotNull(actual);
		Assert.Equal("sOqfBdCQ8YfW2jE1olq1Z6/XbHpo9uIJZydpqr0whLaJ4PWyjDkDW+hZuWNW4hjMMxZEk7eIxSexUJWfwgsklpAFysgzaIXOQhBg0m37fyo=", actual);
	}

	[Fact]
	public void Decrypt()
	{
		var source = "uOhEGcIYiIIx7Hwp8y8aQITd8Bz3X4IMbnrWajh4YtV9xp4dT3oxCWa49zqAXGGk5yEI6xMhmHudowdzajdwYe4V9W6EoBTgCrZQRLRfvQt7Mv5dtXgNXepE1GHbao4aXJUd0w14yNBo2G2yJUFme4uMfaPmKysmNnx8VQ7p4On4LVae4Lt0XN8ZieZbdRsgfU4rh1pbJbWtuzjVR4wBt2CZGRuM8hyPFOgLlOEE9QJn8MnhMKfPFuuDUQbjL+0A";
		string key = "FnzrUTi5fcsdrws7";

		var sut = new CryptographyUtensil();

		var actual = sut.Decrypt(source, key);

		Assert.NotNull(actual);
		Assert.Equal("AGLogin", actual["method"]!.ToString());
		Assert.Equal("19834706515", actual["responeid"]!.ToString());
		Assert.Equal("0000", actual["respcode"]!.ToString());
		Assert.Equal("3238", actual["aid"]!.ToString());
		Assert.Equal("RT706test1", actual["username"]!.ToString());
		Assert.Equal("139af53bm1bdeld336bfxw", actual["token"]!.ToString());
		Assert.Equal("success", actual["status"]!.ToString());
		Assert.Equal("1721703822345", actual["timestamp"]!.ToString());
	}
}
