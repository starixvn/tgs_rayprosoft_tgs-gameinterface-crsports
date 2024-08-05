namespace Tgs.GameInterface.CRSports.Core.Exceptions;

public sealed class GameInterfaceException : Exception
{
	public GameInterfaceException(string respcode) : base(string.Concat(ConvertErrorMesaage(respcode), $"({respcode})"))
	{
	}

	private static string ConvertErrorMesaage(string respcode) => respcode switch
	{
		"0000" => "成功",
		"0001" => "參數錯誤",
		"0002" => "Token 驗證錯誤",
		"0003" => "查無資料",
		"0004" => "信用額度用戶不支持存提款功能",
		"0005" => "上層餘額不足，無法存入",
		"0006" => "餘額不足，無法提出",
		"0007" => "不可新增會員帳號",
		"0008" => "輸入的帳號已經有人使用",
		"0011" => "找不到 AID 的資料物件",
		"0012" => "编解码發生錯誤 (解 Request)",
		"0013" => "编解码發生錯誤 (编 Response)",
		"0014" => "未被定義的方法",
		"0015" => "人數已滿",
		"0016" => "系統維護中",
		"0017" => "系統流量較高 請重新再試",
		"0018" => "新增會員幣別錯誤",
		"0019" => "系統優化中,功能暫停服務/中斷期間 XX:XX:XX(time) to XX:XX:XX(time) (GMT-4)",
		"0102" => "會員登入異常",
		"0103" => "金額達到 8 位數限制",
		"0201" => "無效的帳號或密碼",
		"0202" => "帳號被上鎖",
		"0203" => "密碼錯誤多次,請聯繫您的上線解除封鎖",
		"0204" => "您的帳戶已關閉。請聯繫您的上線開啟您的帳戶",
		"0205" => "操作過於頻繁",
		"0206" => "不開放此服務",
		"6666" => "測試使用",
		"9998" => "網站維護中",
		"9999" => "系統異常",
		_ => $"遊戲供應商未提供錯誤代碼{respcode}"
	};
}
