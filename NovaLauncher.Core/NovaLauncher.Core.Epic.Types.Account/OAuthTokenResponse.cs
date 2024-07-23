using System;
using System.Text.Json.Serialization;

namespace NovaLauncher.Core.Epic.Types.Account;

public class OAuthTokenResponse
{
	[JsonPropertyName("access_token")]
	public required string AccessToken { get; set; }

	[JsonPropertyName("expires_in")]
	public int ExpiresIn => (int)ExpiresAt.Subtract(DateTime.Now).TotalSeconds;

	[JsonPropertyName("expires_at")]
	public DateTime ExpiresAt { get; set; }

	[JsonPropertyName("token_type")]
	public required string TokenType { get; set; }

	[JsonPropertyName("refresh_token")]
	public string? RefreshToken { get; set; }

	[JsonPropertyName("refresh_expires")]
	public int? RefreshExpires => (int)RefreshExpiresAt.Value.Subtract(DateTime.Now).TotalSeconds;

	[JsonPropertyName("refresh_expires_at")]
	public DateTime? RefreshExpiresAt { get; set; }

	[JsonPropertyName("account_id")]
	public required Guid AccountId { get; set; }

	[JsonPropertyName("client_id")]
	public required string ClientId { get; set; }

	[JsonPropertyName("internal_client")]
	public bool InternalClient { get; set; }

	[JsonPropertyName("client_service")]
	public required string ClientService { get; set; }

	[JsonPropertyName("displayName")]
	public required string DisplayName { get; set; }

	[JsonPropertyName("app")]
	public required string App { get; set; }

	[JsonPropertyName("in_app_id")]
	public required string InAppId { get; set; }

	[JsonPropertyName("scope")]
	public required string[]? Scope { get; set; } = Array.Empty<string>();

}
