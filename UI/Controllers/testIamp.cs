using System;
using System.Text;
using System.IO;
using System.Net;
using System.Globalization;
using MailKit.Net.Imap;
using MailKit;
using Newtonsoft.Json;
using MailKit.Security;
using Microsoft.Identity.Client;

class testIamp
{
	/*Client id
	8033c947-1302-4264-9e65-31d65a7312bd
	8033c947-1302-4264-9e65-31d65a7312bd

	Directory
	5acad0c6-2f8d-4d05-bd1e-692eb87e707b

	Object ID 
	e60440ba-42c0-444d-ba9f-2ee362e6b880

	client_secret
	8zX8Q~6Qo2cX9uE60OQMMbaAuXpD_SX9c6OoKatl

	Secret ID
	ffdde047-f25e-4029-a01b-0aafead055ab*/
	public string token_type { get; set; }

	public string Bearer { get; set; }

	public string expires_in { get; set; }

	public string ext_expires_in { get; set; }

	public string access_token { get; set; }

	public static async Task RequestTokenAsync()
	{

		string client_id = "8033c947-1302-4264-9e65-31d65a7312bd";
		string client_secret = "8zX8Q~6Qo2cX9uE60OQMMbaAuXpD_SX9c6OoKatl";
		string tenant = "5acad0c6-2f8d-4d05-bd1e-692eb87e707b";
		string scope = "https://outlook.office365.com/.default";
		string requestData = string.Format("client_id={0}&client_secret={1}&scope={2}&grant_type=client_credentials",
		client_id, client_secret, scope);
		string tokenUri =string.Format("https://login.microsoftonline.com/{0}/oauth2/v2.0/token", tenant);
		string accountEmailAddress = "mesaayudamineduc@mineduc.gob.gt";
		HttpWebRequest httpRequest = WebRequest.Create(tokenUri) as HttpWebRequest;
		httpRequest.Method = "POST";
		httpRequest.ContentType = "application/x-www-form-urlencoded";
		using (Stream requestStream = httpRequest.GetRequestStream())
		{
			byte[] requestBuffer = Encoding.UTF8.GetBytes(requestData);
			requestStream.Write(requestBuffer, 0, requestBuffer.Length);
			requestStream.Close();
		}
		try
		{
			HttpWebResponse httpResponse = httpRequest.GetResponse() as HttpWebResponse;
			using (StreamReader reader = new StreamReader(httpResponse.GetResponseStream()))
			{
				// reads response body
				string responseText = reader.ReadToEnd();
				// output the response text which includes the access token.
				Console.WriteLine(responseText);
				var responseAuth = JsonConvert.DeserializeObject<testIamp>(responseText);
				Console.WriteLine(responseAuth.access_token);

				//var oauth2 = new SaslMechanismOAuth2(authToken.Account.Username, authToken.AccessToken);
				var options = new PublicClientApplicationOptions
				{
					ClientId = client_id,
					TenantId = tenant,

					// Use "https://login.microsoftonline.com/common/oauth2/nativeclient" for apps using
					// embedded browsers or "http://localhost" for apps that use system browsers.
					RedirectUri = "https://login.microsoftonline.com/common/oauth2/nativeclient"
				};

				var confidentialClientApplication = ConfidentialClientApplicationBuilder.Create(client_id)
					.WithAuthority($"https://login.microsoftonline.com/{tenant}/v2.0")
					.WithClientSecret (client_secret)
					.Build();

				var scopes = new string[] {
					// For IMAP and POP3, use the following scope
					"https://ps.outlook.com/.default"

					// For SMTP, use the following scope
					// "https://outlook.office365.com/.default"
				};

				var authToken = await confidentialClientApplication.AcquireTokenForClient(scopes).ExecuteAsync();
				var oauth2 = new SaslMechanismOAuth2(accountEmailAddress, authToken.AccessToken);

				//var authToken = await publicClientApplication.AcquireTokenInteractive(scopes).ExecuteAsync();
				//var oauth2 = new SaslMechanismOAuth2(authToken.Account.Username, authToken.AccessToken);
				//var oauth2 = new SaslMechanismOAuth2("mesaayudamineduc@mineduc.gob.gt",responseAuth.access_token);

				//var authToken = await publicClientApplication.AcquireTokenInteractive(scopes).ExecuteAsync ();
				//var oauth2 = new SaslMechanismOAuth2 (authToken.Account.Username, authToken.AccessToken);
				using (var client = new ImapClient())
				{
					//client.Connect("outlook.office365.com", 993, true);

					//client.Authenticate("mesaayudamineduc@mineduc.gob.gt", responseAuth.access_token);
					 await client.ConnectAsync ("outlook.office365.com", 993, SecureSocketOptions.SslOnConnect);
					await client.AuthenticateAsync(oauth2);
					await client.DisconnectAsync (true);


					// The Inbox folder is always available on all IMAP servers...
					var inbox = client.Inbox;
					inbox.Open(FolderAccess.ReadOnly);

					Console.WriteLine("Total messages: {0}", inbox.Count);
					Console.WriteLine("Recent messages: {0}", inbox.Recent);

					for (int i = 0; i < inbox.Count; i++)
					{
						var message = inbox.GetMessage(i);
						Console.WriteLine("Subject: {0}", message.Subject);
					}

					client.Disconnect(true);
				}
			}
		}
		catch (WebException ex)
		{
			Console.WriteLine(ex.Message);
			if (ex.Status == WebExceptionStatus.ProtocolError)
			{
				// get more detail from the response stream.
				var response = ex.Response as HttpWebResponse;
				if (response != null)
				{
					Console.WriteLine("HTTP: " + response.StatusCode);
					using (StreamReader reader = new StreamReader(response.GetResponseStream()))
					{
						// reads response body

					}
				}
			}
		}
	}
}


class testIamp2
{
	/*Client id
	8033c947-1302-4264-9e65-31d65a7312bd
	8033c947-1302-4264-9e65-31d65a7312bd

	Directory
	5acad0c6-2f8d-4d05-bd1e-692eb87e707b

	Object ID 
	e60440ba-42c0-444d-ba9f-2ee362e6b880

	client_secret
	8zX8Q~6Qo2cX9uE60OQMMbaAuXpD_SX9c6OoKatl

	Secret ID
	ffdde047-f25e-4029-a01b-0aafead055ab*/
	public string token_type { get; set; }

	public string Bearer { get; set; }

	public string expires_in { get; set; }

	public string ext_expires_in { get; set; }

	public string access_token { get; set; }

	static string client_id = "8033c947-1302-4264-9e65-31d65a7312bd";
	static string client_secret = "8zX8Q~6Qo2cX9uE60OQMMbaAuXpD_SX9c6OoKatl";
	static string tenant = "5acad0c6-2f8d-4d05-bd1e-692eb87e707b";
	string scope = "https://outlook.office365.com/.default";

	static string ExchangeAccount = "mesaayudamineduc@mineduc.gob.gt";

	public static async Task RequestTokenAsync()
	{

		string client_id = "8033c947-1302-4264-9e65-31d65a7312bd";
		string client_secret = "8zX8Q~6Qo2cX9uE60OQMMbaAuXpD_SX9c6OoKatl";
		string tenant = "5acad0c6-2f8d-4d05-bd1e-692eb87e707b";
		string scope = "https://outlook.office365.com/.default";
		string requestData = string.Format("client_id={0}&client_secret={1}&scope={2}&grant_type=client_credentials",
		client_id, client_secret, scope);
		string tokenUri = string.Format("https://login.microsoftonline.com/{0}/oauth2/v2.0/token", tenant);


		HttpWebRequest httpRequest = WebRequest.Create(tokenUri) as HttpWebRequest;
		httpRequest.Method = "POST";
		httpRequest.ContentType = "application/x-www-form-urlencoded";
		using (Stream requestStream = httpRequest.GetRequestStream())
		{
			byte[] requestBuffer = Encoding.UTF8.GetBytes(requestData);
			requestStream.Write(requestBuffer, 0, requestBuffer.Length);
			requestStream.Close();
		}

		HttpWebResponse httpResponse = httpRequest.GetResponse() as HttpWebResponse;
		testIamp responseAuth= null;
		using (StreamReader reader = new StreamReader(httpResponse.GetResponseStream()))
		{
			// reads response body
			string responseText = reader.ReadToEnd();
			// output the response text which includes the access token.
			Console.WriteLine(responseText);
			responseAuth = JsonConvert.DeserializeObject<testIamp>(responseText);
			Console.WriteLine(responseAuth.access_token);
			var oauth2 = new SaslMechanismOAuthBearer(ExchangeAccount, responseAuth.access_token);
		}


		using (var client = new ImapClient())
		{
			client.Connect("outlook.office365.com", 993, SecureSocketOptions.SslOnConnect);

			if (client.AuthenticationMechanisms.Contains("OAUTHBEARER") || client.AuthenticationMechanisms.Contains("XOAUTH2"))
				AuthenticateAsync(client, responseAuth).GetAwaiter().GetResult();

			// The Inbox folder is always available on all IMAP servers...
			var inbox = client.Inbox;
			inbox.Open(FolderAccess.ReadOnly);

			Console.WriteLine("Total messages: {0}", inbox.Count);
			Console.WriteLine("Recent messages: {0}", inbox.Recent);

			for (int i = 0; i < inbox.Count; i++)
			{
				var message = inbox.GetMessage(i);
				Console.WriteLine("Subject: {0}", message.Subject);
			}

			client.Disconnect(true);
		}

	}
	static async Task AuthenticateAsync(ImapClient client, testIamp responseAuth)
	{
		var options = new PublicClientApplicationOptions
		{
			ClientId = client_id,
			TenantId = tenant,
			RedirectUri = "https://login.microsoftonline.com/common/oauth2/nativeclient"
		};

		var publicClientApplication = PublicClientApplicationBuilder
			.CreateWithApplicationOptions(options)
			.Build();

		var scopes = new string[] {
					"email",
					"offline_access",
					"https://outlook.office.com/IMAP.AccessAsUser.All", // Only needed for IMAP
					//"https://outlook.office.com/POP.AccessAsUser.All",  // Only needed for POP
					//"https://outlook.office.com/SMTP.AccessAsUser.All", // Only needed for SMTP
				};


		//var authToken = await publicClientApplication.AcquireTokenInteractive(scopes).WithLoginHint(ExchangeAccount).ExecuteAsync();
		//await publicClientApplication.AcquireTokenSilent(scopes, authToken.Account).ExecuteAsync();

		// Note: We use authToken.Account.Username here instead of ExchangeAccount because the user *may* have chosen a
		// different Microsoft Exchange account when presented with the browser window during the authentication process.
		//SaslMechanism oauth2;

		// if (client.AuthenticationMechanisms.Contains("OAUTHBEARER"))
		// 	oauth2 = new SaslMechanismOAuthBearer(authToken.Account.Username, authToken.AccessToken);
		// else
		// 	oauth2 = new SaslMechanismOAuth2(authToken.Account.Username, authToken.AccessToken);

		var oauth2 = new SaslMechanismOAuthBearer(ExchangeAccount, responseAuth.access_token);

		await client.AuthenticateAsync(oauth2);
	}
}

